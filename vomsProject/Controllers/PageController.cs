using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly string RootDomain;
        private readonly SolutionHelper _solutionHelper;
        private readonly DomainHelper _domainHelper;

        public PageController(ILogger<PageController> logger, IConfiguration configuration, 
            ApplicationDbContext context, UserManager<User> userManager, SolutionHelper solutionHelper, DomainHelper domainHelper)
        {
            _logger = logger;
            Configuration = configuration;
            Context = context;
            UserManager = userManager;
            RootDomain = Configuration["RootDomain"];
            _solutionHelper = solutionHelper;
            _domainHelper = domainHelper;
        }

        // Get the solution from a domain. This function returns a set of one or zero solutions.
        private IQueryable<Solution> GetSolution(string domain)
        {
            if (domain.EndsWith(RootDomain))
            {
                var subdomain = domain.Substring(0, domain.Length - (RootDomain.Length + 1)); // + 1 for extra dot
                return Context.Solutions.Where((solution) => solution.Subdomain == subdomain);
            }
            else
            {
                return Context.Solutions.Where((solution) => solution.Domain == domain);
            }
        }

        public async Task<IActionResult> Index(string pageName)
        {
            if (pageName == null)
            {
                pageName = "";
            }
            var userTask = UserManager.GetUserAsync(HttpContext.User);
            var solution = GetSolution(Request.Host.Host);
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
    }
}
