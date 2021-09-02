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
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace vomsProject.Controllers
{
    public class PreviewController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly RepositoryService _repository;
        private readonly ApplicationDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public PreviewController(ILogger<HomeController> logger, RepositoryService repository, ApplicationDbContext dbContext, IConfiguration configuration)
        {
            _logger = logger;
            _repository = repository;
            _dbContext = dbContext;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Solution(int id)
        {
            var model = new SolutionViewModel { Solution = _repository.GetSolutionById(id) };

            return View(model);
        }

        [HttpGet]
        public IActionResult Page(int id)
        {
            var model = _dbContext.Pages.FirstOrDefault(x => x.Id == id);
            if (model == null) return View((Page)null);

            return View(model);
        }
    }
}
