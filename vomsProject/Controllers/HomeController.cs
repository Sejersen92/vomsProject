using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using vomsProject.Data;
using vomsProject.Helpers;
using vomsProject.Models;

namespace vomsProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly RepositoryService _repository;
        private readonly ApplicationDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly DomainHelper _domainHelper;
        private readonly UserManager<User> UserManager;

        public HomeController(ILogger<HomeController> logger, RepositoryService repository, 
            ApplicationDbContext dbContext, IConfiguration configuration, DomainHelper domainHelper, UserManager<User> userManager)
        {
            _logger = logger;
            _repository = repository;
            _dbContext = dbContext;
            _configuration = configuration;
            _domainHelper = domainHelper; 
            UserManager = userManager;
        }

        public async Task<IActionResult> Index(string searchString, int? pageNumber = 1)
        {
            //HomePageViewModel
            var user = await UserManager.GetUserAsync(HttpContext.User);
            var solutions = _repository.GetSolutionsByUser(user);
            var pageSize = 3;

            if (!string.IsNullOrEmpty(searchString) && searchString != "All")
            {
                solutions = solutions.Where(s => s.Subdomain == searchString);
            }

            var model = new HomePageViewModel()
            {
                Solutions = await PaginatedList<Solution>.CreateAsync(solutions, pageNumber ?? 1, pageSize),
                User = user
            };

            foreach (var solution in model.Solutions)
            {
                solution.DestinationUrl = _domainHelper.GetSolutionIndexPageUrl(solution);
            }

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
