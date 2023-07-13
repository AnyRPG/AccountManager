using System.Security.Permissions;

namespace AccountManager.Models
{
    public class AuthenticationRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }

        public AuthenticationRequest(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }

        public AuthenticationRequest(IFormCollection collection)
        {
            // put form into variables
            UserName = collection["username"];
            Password = collection["password"];
        }

    }
}
