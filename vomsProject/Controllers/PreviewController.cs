using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using vomsProject.Data;
using vomsProject.Helpers;
using vomsProject.Models;

namespace vomsProject.Controllers
{
    public class PreviewController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DatabaseHelper _databaseHelper;
        private readonly ApplicationDbContext _dbContext;

        public PreviewController(ILogger<HomeController> logger, DatabaseHelper databaseHelper, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _databaseHelper = databaseHelper;
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Solution(int id)
        {
            var model = new SolutionViewModel { Solution = _databaseHelper.GetSolutionById(id, _dbContext) };

            return View(model);
        }

        public IActionResult Page(int id)
        {
            var model = _dbContext.Pages.Where(x => x.Id == id).ToList();

            //Utilize the QUill editor to renderHtml in a non-editable way.
            return View(model);
        }
    }
}
