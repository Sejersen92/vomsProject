using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using vomsProject.Data;
using vomsProject.Helpers;

namespace vomsProject.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly StorageHelper _storageHelper;

        public AdminController(StorageHelper storageHelper, ApplicationDbContext dbContext)
        {
            _storageHelper = storageHelper;
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            var model = Solutions();

            return View(model);
        }

        private IEnumerable<Solution> Solutions()
        {
            var result = new List<Solution>();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!string.IsNullOrWhiteSpace(userId))
            {
                result.AddRange(_storageHelper.GetSolutions(userId, _dbContext));
            }
            
            return result;
        }
    }
}
