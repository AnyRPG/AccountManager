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
    
    public class AccountController : Controller
    {
        private readonly GameDbContext dbContext;
        private readonly UserAccountService accountService;
        private readonly ILogger logger;
        private readonly AccountManager.Services.IAuthenticationService authenticationService;
        private readonly AccountManagerSettings accountManagerSettings;

        public AccountController(ILogger<AccountController> logger, GameDbContext dbContext, AccountManagerSettings accountManagerSettings)
        {
            this.dbContext = dbContext;
            this.accountManagerSettings = accountManagerSettings;
            accountService = new UserAccountService(dbContext);
            authenticationService = new AccountManager.Services.AuthenticationService(accountManagerSettings, dbContext, logger);
            this.logger = logger;
        }

        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        // GET: Account/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Account/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Account/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                //logger.LogInformation("Adding new user");

                var (success, content) = authenticationService.AddUserFromForm(collection);
                if (!success)
                {
                    return BadRequest(content);
                }
                return RedirectToAction(nameof(RegisterSuccess));
            }
            catch
            {
                return View();
            }
        }

        // GET: Account/RegisterSuccess
        public ActionResult RegisterSuccess() {
            return View();
        }

        // GET: Account/Login
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(IFormCollection collection)
        {
            try
            {
                //logger.LogInformation("Logging in user");
                AuthenticationRequest authenticationRequest = new AuthenticationRequest();
                authenticationRequest.GetRequestFromCollection(collection);
                var (success, content) = authenticationService.Login(authenticationRequest, HttpContext);
                if (!success)
                {
                    TempData["ErrorMessage"] = content;
                    return View();
                }
                return Redirect(HttpContext.Request.Query["page"].ToString() == string.Empty ? "/Account/Dashboard" : collection["ReturnUrl"]);
            } catch
            {
                return View();
            }
        }

        // GET: Account/logout
        public ActionResult Logout()
        {
            authenticationService.Logout(HttpContext);
            return RedirectToAction(nameof(Index));
        }

        // GET: Account/Dashboard
        [Authorize]
        public ActionResult Dashboard()
        {
            ViewData["UserName"] = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.Name).Value;
            ViewData["UserId"] = HttpContext.User.Claims.First(c => c.Type == "Id").Value;
            return View();
        }

        // GET: AccountController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AccountController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AccountController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AccountController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
