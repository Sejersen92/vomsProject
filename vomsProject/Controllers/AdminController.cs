using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using vomsProject.Data;
using vomsProject.Helpers;
using vomsProject.Models;

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

        [Authorize]
        public IActionResult Index()
        {
            var model = Solutions();

            return View(model);
        }

        [Authorize]
        public IActionResult Pages([FromRoute] int id)
        {
            var model = new PageOverview();
            model.Pages = _dbContext.Pages.Where(x => x.Solution.Id == id).ToList();
            model.SolutionId = id;

            return View(model);
        }

        [Authorize]
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

        [Authorize]
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

        [Authorize]
        [HttpPost]
        public async Task<bool> CreatePageAsync (string title, int solutionId)
        {
            try
            {
                var page = new Page()
                {
                    PageName = title,
                    Solution = await _dbContext.Solutions.FindAsync(solutionId)
                };

                _dbContext.Pages.Add(page);
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
    }
}
