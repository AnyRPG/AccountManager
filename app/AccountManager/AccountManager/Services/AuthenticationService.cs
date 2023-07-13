using AccountManager.Database;
using AccountManager.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AccountManager.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly AccountManagerSettings accountManagerSettings;
        private readonly GameDbContext gameDbContext;
        private readonly ILogger<AuthenticationService> logger;

        public AuthenticationService(AccountManagerSettings accountManagerSettings, GameDbContext gameDbContext, ILogger<AuthenticationService> logger)
        {
            this.accountManagerSettings = accountManagerSettings;
            this.gameDbContext = gameDbContext;
            this.logger = logger;
        }

        public (bool success, string content) AddUserFromForm(IFormCollection collection)
        {

            // put form into variables
            string userName = collection["UserName"];
            string password = collection["Password"];
            string email = collection["Email"];
            string phone = collection["Phone"];

            // format a user object and return the value of the registration attempt
            User user = new User()
            {
                UserName = userName,
                Email = email,
                Phone = phone,
                PasswordHash = password
            };

            return Register(user);
        }

        public (bool success, string content) Register(User user)
        {
            // check if username is not taken
            if (gameDbContext.Users.Any(u => u.UserName == user.UserName))
            {
                logger.LogInformation($"Username {user.UserName} was not available during registration attempt");
                return (false, "Username not available");
            }
            
            // populate salt and hash values
            user.ProvideSaltAndHash();

            gameDbContext.Add(user);
            gameDbContext.SaveChanges();
            logger.LogInformation($"Registered new user {user.UserName}");

            return (true, "");

        }

        /*
        public (bool success, string content) LoginUserFromForm(IFormCollection collection)
        {

            // put form into variables
            string userName = collection["username"];
            string password = collection["password"];

            return Login(userName, password);
        }
        */

        public (bool sucess, string token) Login(AuthenticationRequest authenticationRequest, HttpContext httpContext)
        {
            var user = gameDbContext.Users.SingleOrDefault(u => u.UserName == authenticationRequest.UserName);
            if (user == null)
            {
                logger.LogInformation($"[LOGIN] invalid username {authenticationRequest.UserName}");
                return (false, "Invalid username");
            }

            if (user.PasswordHash != AuthenticationHelpers.ComputeHash(authenticationRequest.Password, user.Salt))
            {
                logger.LogInformation($"[LOGIN] invalid password for user {authenticationRequest.UserName}");
                return (false, "Invalid password");
            }

            var task = CookieLogin(user, httpContext);
            task.Wait();

            logger.LogInformation($"[LOGIN] Successfully logged in user {authenticationRequest.UserName}");

            return (true, GenerateJwtToken(AssembleClaimsIdentity(user)));
        }

        public void Logout(HttpContext httpContext)
        {
            var task = httpContext.SignOutAsync();
            task.Wait();
            logger.LogInformation($"[LOGOUT] Logged out user");
        }

        private async Task CookieLogin(User user, HttpContext httpContext)
        {
            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim("Id", user.Id.ToString())
                };
            var claimsIdentity = new ClaimsIdentity(claims, "Login");

            await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
        }

        private ClaimsIdentity AssembleClaimsIdentity(User user)
        {
            var subject = new ClaimsIdentity(new[]{
                new Claim("id", user.Id.ToString()),
            });

            return subject;
        }

        private string GenerateJwtToken(ClaimsIdentity subject)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(accountManagerSettings.BearerKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = subject,
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

    }



    public interface IAuthenticationService
    {
        (bool success, string content) Register(User user);
        //(bool sucess, string token) Login(string userName, string password);
        (bool sucess, string token) Login(AuthenticationRequest authenticationRequest, HttpContext httpContext);
        (bool success, string content) AddUserFromForm(IFormCollection collection);
        void Logout(HttpContext httpContext);
        //(bool success, string content) LoginUserFromForm(IFormCollection collection);
    }

    public static class AuthenticationHelpers
    {
        public static void ProvideSaltAndHash(this User user)
        {
            var salt = GenerateSalt();
            user.Salt = Convert.ToBase64String(salt);
            user.PasswordHash = ComputeHash(user.PasswordHash, user.Salt);
        }

        private static byte[] GenerateSalt()
        {
            var rng = RandomNumberGenerator.Create();
            var salt = new byte[24];
            rng.GetBytes(salt);
            return salt;
        }

        public static string ComputeHash(string password, string saltString)
        {
            var salt = Convert.FromBase64String(saltString);

            using var hashGenerator = new Rfc2898DeriveBytes(password, salt);
            hashGenerator.IterationCount = 10101;
            var bytes = hashGenerator.GetBytes(24);
            return Convert.ToBase64String(bytes);
        }
    }
}
