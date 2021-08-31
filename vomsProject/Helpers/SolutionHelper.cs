using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vomsProject.Controllers;
using vomsProject.Data;

namespace vomsProject.Helpers
{
    public class SolutionHelper
    {
        private readonly ILogger<PageController> _logger;
        private readonly IConfiguration Configuration;
        private readonly ApplicationDbContext Context;
        private readonly UserManager<User> UserManager;
        private readonly string RootDomain;


        public SolutionHelper(ILogger<PageController> logger, IConfiguration configuration, ApplicationDbContext context, UserManager<User> userManager)
        {
            _logger = logger;
            Configuration = configuration;
            Context = context;
            UserManager = userManager;
            RootDomain = Configuration["RootDomain"];
        }

        /// <summary>
        /// Get the page by name belonging to the solution.
        /// </summary>
        /// <param name="solution">A query for a single solution</param>
        /// <param name="pageName">The url path for the page</param>
        /// <returns>Returns a set of one or zero pages.</returns>
        public IQueryable<Page> PageQuery(IQueryable<Solution> solution, string pageName)
        {
            return solution.SelectMany((solution) => solution.Pages)
                .Where((page) => page.PageName == pageName);
        }

        /// <summary>
        /// Check if a User has access to a Solution. This funcion will make a lookup in the permissions table.
        /// </summary>
        /// <param name="solution">The Solution</param>
        /// <param name="user">The User</param>
        /// <returns>Returns true if a user has access.</returns>
        public async Task<bool> IsUserOnSolution(Solution solution, User user)
        {
            return await Context.Permissions.AnyAsync((permission) =>
                solution == permission.Solution
                && user == permission.User);
        }

        /// <summary>
        /// Check if a User has the specified permission level on a Solution. This funcion will make a lookup in the permissions table.
        /// </summary>
        /// <param name="user">The User</param>
        /// <param name="solution">The Solution</param>
        /// <param name="permissionLevel">The required permission level</param>
        /// <returns>Returns true if a user has access.</returns>
        public async Task<bool> DoUserHavePermissionOnSolution(User user, Solution solution, PermissionLevel permissionLevel)
        {
            return await Context.Permissions.AnyAsync((permission) =>
                solution == permission.Solution
                && user == permission.User
                && permission.PermissionLevel == permissionLevel);
        }

        /// <summary>
        /// Get the solution from a domain.
        /// </summary>
        /// <param name="domain">The domain name a solution was requested for.</param>
        /// <returns>Returns a set of one or zero solutions.</returns>
        public IQueryable<Solution> GetSolutionByDomainName(string domain)
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
    }
}
