using AccountManager.Database;
using AccountManager.Models;
using AccountManager.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace AccountManager.Controllers
{
    [ApiController]
    [Route("api")]
    public class ApiController : Controller
    {
        private readonly GameDbContext dbContext;
        private readonly UserAccountService accountService;
        private readonly ILogger<AccountController> logger;
        private readonly AccountManager.Services.IAuthenticationService authenticationService;
        private readonly AccountManagerSettings accountManagerSettings;

        public ApiController(ILogger<AccountController> logger, GameDbContext dbContext, AccountManagerSettings accountManagerSettings, AccountManager.Services.IAuthenticationService authenticationService)
        {
            this.dbContext = dbContext;
            this.accountManagerSettings = accountManagerSettings;
            accountService = new UserAccountService(dbContext);
            this.authenticationService = authenticationService;
            //authenticationService = new AccountManager.Services.AuthenticationService(accountManagerSettings, dbContext, logger);
            this.logger = logger;
        }

        [HttpPost("login")]
        public ActionResult Login(AuthenticationRequest authenticationRequest)
        {
            try
            {
                //logger.LogInformation("Logging in user");
                //AuthenticationRequest authenticationRequest = new AuthenticationRequest(collection);
                var (success, content) = authenticationService.Login(authenticationRequest, HttpContext);
                if (!success)
                {
                    return BadRequest(content);
                }
                return Ok(new AuthenticationResponse() { Token = content });
            } catch (Exception e)
            {
                logger.LogError(e.Message);
                return BadRequest($"Error Occured: {e.Message}");
            }
        }

        // GET: Account/logout
        /*
        public ActionResult Logout()
        {
            authenticationService.Logout(HttpContext);
            return RedirectToAction(nameof(Index));
        }
        */

    }
}
