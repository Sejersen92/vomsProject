using Microsoft.AspNetCore.Mvc;
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
        private readonly StorageHelper _storageHelper;
        private readonly ApplicationDbContext _dbContext;

        public HomeController(ILogger<HomeController> logger, StorageHelper storageHelper, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _storageHelper = storageHelper;
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            var model = new Image();
            model.ImageUrl = _storageHelper.GetImageUrl(1, _dbContext);

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
