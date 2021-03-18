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

namespace vomsProject.Controllers
{
    public class PageController : Controller
    {
        private readonly ILogger<PageController> _logger;
        private readonly IConfiguration Configuration;
        private readonly ApplicationDbContext Context;
        private readonly UserManager<User> UserManager;
        private readonly string RootDomain;
        public PageController(ILogger<PageController> logger, IConfiguration configuration, ApplicationDbContext context, UserManager<User> userManager)
        {
            _logger = logger;
            Configuration = configuration;
            Context = context;
            UserManager = userManager;
            RootDomain = Configuration["RootDomain"];
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

        // Get the page by name belonging to the solution. Returns null if the page dosen't exist
        private async Task<Page> GetPage(IQueryable<Solution> solution, string pageName)
        {
            try
            {
                return await solution.SelectMany((solution) => solution.Pages)
                    .SingleAsync((page) => page.PageName == pageName);
            }
            catch (InvalidOperationException ex)
            {
                return null;
            }
        }

        private async Task<bool> IsUserOnSoulution(Solution solution, User user)
        {
            return await Context.Permissions.AnyAsync((permission) =>
                solution == permission.Solution
                && user == permission.User);
        }

        public async Task<IActionResult> Index(string pageName)
        {
            var userTask = UserManager.GetUserAsync(HttpContext.User);
            var solution = GetSolution(Request.Host.Host);
            var page = await GetPage(solution, pageName);
            var user = await userTask;
            if (user != null)
            {
                var theSolution = await solution.SingleOrDefaultAsync();
                if (theSolution != null && await IsUserOnSoulution(theSolution, user))
                {
                    if (page == null)
                    {
                        // We will store the page if it gets saved
                        page = new Page { Solution = theSolution };
                    }

                    // return editable page
                    return View(page);
                }
            }

            if (page != null)
            {
                // TODO: we should return an non editable page
                return View(page);
            }
            return NotFound();
        }
    }
}
