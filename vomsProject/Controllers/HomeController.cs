using Microsoft.AspNetCore.Mvc;
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
        private readonly DatabaseHelper _databaseHelper;
        private readonly ApplicationDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, DatabaseHelper databaseHelper, ApplicationDbContext dbContext, IConfiguration configuration)
        {
            _logger = logger;
            _databaseHelper = databaseHelper;
            _dbContext = dbContext;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            var model = new HomePageViewModel 
            { 
                Solutions = _databaseHelper.GetSolutions(_dbContext), 
                DestinationUrl = $".{_configuration.GetValue<string>("RootDomain")}:5001/" 
            };

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
