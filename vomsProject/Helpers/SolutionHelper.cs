﻿using Microsoft.AspNetCore.Identity;
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

        // Get the page by name belonging to the solution. Returns null if the page dosen't exist
        public async Task<Page> GetPage(IQueryable<Solution> solution, string pageName)
        {
            try
            {
                return await solution.SelectMany((solution) => solution.Pages)
                    .Include((page) => page.LastSavedVersion)
                    .Include((page) => page.Layout)
                    .SingleAsync((page) => page.PageName == pageName);
            }
            catch (InvalidOperationException ex)
            {
                return null;
            }
        }

        // Get the page by name belonging to the solution. Returns null if the page dosen't exist or it is not published
        public async Task<Page> GetPageIfPublished(IQueryable<Solution> solution, string pageName)
        {
            try
            {
                var page = await GetPage(solution, pageName);
                if (page.IsPublished)
                {
                    return page;
                }
                return null;
            }
            catch (InvalidOperationException ex)
            {
                return null;
            }
        }

        public async Task<bool> IsUserOnSolution(Solution solution, User user)
        {
            return await Context.Permissions.AnyAsync((permission) =>
                solution == permission.Solution
                && user == permission.User);
        }
        // Get the solution from a domain. This function returns a set of one or zero solutions.
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
