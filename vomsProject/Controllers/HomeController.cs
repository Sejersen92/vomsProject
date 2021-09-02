﻿using Microsoft.AspNetCore.Mvc;
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

        public HomeController(ILogger<HomeController> logger, RepositoryService repository, 
            ApplicationDbContext dbContext, IConfiguration configuration, DomainHelper domainHelper)
        {
            _logger = logger;
            _repository = repository;
            _dbContext = dbContext;
            _configuration = configuration;
            _domainHelper = domainHelper;
        }

        public IActionResult Index()
        {
            var model = new HomePageViewModel 
            { 
                Solutions = _dbContext.Solutions.Include(x => x.Permissions).ThenInclude(x => x.User).ToList()
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
