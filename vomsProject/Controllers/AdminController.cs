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

        public IActionResult Pages(int id)
        {

            var databasePages = _dbContext.Pages.Where(x => x.Solution.Id == id).ToList();

            return View(databasePages);
        }

        private IEnumerable<Solution> Solutions()
        {
            var result = new List<Solution>();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!string.IsNullOrWhiteSpace(userId))
            {
                try
                {
                    result.AddRange(_storageHelper.GetSolutions(userId, _dbContext));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return result;
                }
            }
            return result;
        }

        [HttpPost]
        public IEnumerable<string> CreateProject(string title, string users)
        {
            var userlist = users.Split(",");
            var databaseUsers = _dbContext.Users.Where(x => userlist.Contains(x.UserName));

            var project = new Solution()
            {
                Subdomain = title,
                Users = databaseUsers.ToList()
            };

            _dbContext.Solutions.Add(project);
            _dbContext.SaveChangesAsync();

            return userlist;
        }
    }
}
