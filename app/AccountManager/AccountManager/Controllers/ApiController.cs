using AccountManager.Database;
using AccountManager.Models;
using AccountManager.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace AccountManager.Controllers
{
    [ApiController]
    [Route("api")]
    public class ApiController : Controller
    {
        private readonly GameDbContext dbContext;
        private readonly PlayerCharacterService playerCharacterService;
        private readonly ILogger<ApiController> logger;
        private readonly AccountManager.Services.IAuthenticationService authenticationService;
        private readonly AccountManagerSettings accountManagerSettings;

        public ApiController(ILogger<ApiController> logger, GameDbContext dbContext, AccountManagerSettings accountManagerSettings, AccountManager.Services.IAuthenticationService authenticationService)
        {
            this.dbContext = dbContext;
            this.accountManagerSettings = accountManagerSettings;
            playerCharacterService = new PlayerCharacterService(dbContext, logger);
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
                logger.LogError(e.ToString());
                return BadRequest("Error occured on server.  See server logs for more details.");
            }
        }

        [HttpPost("createplayercharacter")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ActionResult CreatePlayerCharacter(CreateCharacterRequest createCharacterRequest)
        {
            try
            {
                //logger.LogInformation("Logging in user");
                // determine userId from JWT
                var userIdString = User.FindFirst("id")?.Value;
                if (userIdString == null) {
                    return BadRequest("Could not determine User Id");
                }
                var userId = int.Parse(userIdString);

                // for example - find user in database, then perform some validation
                // var user = dbContext.Users.Include(u=>u.PlayerCharacters).First(u => u.Id == userId);
                
                // add new character
                var success = playerCharacterService.AddPlayerCharacter(userId, createCharacterRequest);
                if (!success)
                {
                    return BadRequest();
                }
                return Ok();
            } catch (Exception e)
            {
                logger.LogError(e.ToString());
                return BadRequest("Error occured on server.  See server logs for more details.");
            }
        }

        [HttpPost("getplayercharacters")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ActionResult GetPlayerCharacters()
        {
            try
            {
                //logger.LogInformation("Logging in user");
                // determine userId from JWT
                var userIdString = User.FindFirst("id")?.Value;
                if (userIdString == null)
                {
                    return BadRequest("Could not determine User Id");
                }
                var userId = int.Parse(userIdString);

                // for example - find user in database, then perform some validation
                // var user = dbContext.Users.Include(u=>u.PlayerCharacters).First(u => u.Id == userId);

                // add new character
                var playerCharacterList = playerCharacterService.GetPlayerCharacters(userId);
                /*
                if (!success)
                {
                    return BadRequest();
                }
                */
                return Ok(playerCharacterList);
            } catch (Exception e)
            {
                logger.LogError(e.ToString());
                return BadRequest("Error occured on server.  See server logs for more details.");
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
