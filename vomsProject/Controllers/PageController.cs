using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vomsProject.Data;
using vomsProject.Helpers;

namespace vomsProject.Controllers
{
    public class PageController : Controller
    {
        private readonly ILogger<PageController> _logger;
        private readonly IConfiguration Configuration;
        private readonly ApplicationDbContext Context;
        private readonly UserManager<User> UserManager;
        private readonly SignInManager<User> SignInManager;
        private readonly string RootDomain;
        private readonly SolutionHelper _solutionHelper;
        private readonly DomainHelper _domainHelper;
        private readonly JwtService JwtService;

        public PageController(ILogger<PageController> logger, IConfiguration configuration,
            ApplicationDbContext context, UserManager<User> userManager, SolutionHelper solutionHelper, DomainHelper domainHelper, SignInManager<User> signInManager, JwtService jwtService)
        {
            _logger = logger;
            Configuration = configuration;
            Context = context;
            UserManager = userManager;
            SignInManager = signInManager;
            RootDomain = Configuration["RootDomain"];
            _solutionHelper = solutionHelper;
            _domainHelper = domainHelper;
            JwtService = jwtService;
        }

        /// <summary>
        /// This is the page dispatcher. Each request for a page on a soultion is looked up by this handler.
        /// </summary>
        /// <param name="pageName">The name of the page</param>
        /// <returns></returns>
        public async Task<IActionResult> Index(string pageName)
        {
            if (pageName == null)
            {
                pageName = "";
            }
            var userTask = UserManager.GetUserAsync(HttpContext.User);
            var solution = _solutionHelper.GetSolutionByDomainName(Request.Host.Host);
            var user = await userTask;
            if (user != null)
            {
                var theSolution = await solution.SingleOrDefaultAsync();
                if (theSolution != null && await _solutionHelper.IsUserOnSolution(theSolution, user))
                {
                    var page = await _solutionHelper.GetPage(solution, pageName);
                    if (page == null)
                    {
                        // We will store the page if it gets saved
                        page = new Page { Solution = theSolution };
                    }

                    // return editable page
                    return View(page);
                }
            }
            var publishedPage = await _solutionHelper.GetPageIfPublished(solution, pageName);
            if (publishedPage != null)
            {
                // TODO: we should return an non editable page
                return View(publishedPage);
            }
            return NotFound();
        }
        /// <summary>
        /// Log in a user on the solution. On success redirect to index page. On failure redirect to the CMS page.
        /// </summary>
        /// <param name="token">Jwt token used to authenticate. The token subject is the id for the user.</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Login(string token)
        {
            try
            {
                SecurityToken loginToken;
                new JwtSecurityTokenHandler().ValidateToken(token, JwtService.TokenValidationParamters, out loginToken);
                var jwtToken = (JwtSecurityToken)loginToken;
                var userId = jwtToken.Subject;
                var user = await UserManager.FindByIdAsync(userId);
                await SignInManager.SignInAsync(user, true);
                var solution = _solutionHelper.GetSolutionByDomainName(Request.Host.Host).FirstOrDefault();
                return Redirect(_domainHelper.GetIndexPageUrl(solution));
            }
            catch
            {
                // Faild to authenticate
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Log out a user from the solution. Then redirect to index page.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await SignInManager.SignOutAsync();
            var solution = _solutionHelper.GetSolutionByDomainName(Request.Host.Host).FirstOrDefault();
            return Redirect(_domainHelper.GetIndexPageUrl(solution));
        }
    }
}
