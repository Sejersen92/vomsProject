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

        public async Task<IActionResult> Solution(int id)
        {
            var solution = await _repository.GetSolutionById(id)
                .Include(x => x.Permissions).ThenInclude(perm => perm.User)
                .Include(x => x.Pages)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (solution == null)
            {
                return NotFound();
            }
            var model = new SolutionViewModel { Solution = solution };

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
