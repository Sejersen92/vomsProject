﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using vomsProject.Data;
using vomsProject.Helpers;
using vomsProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;

namespace vomsProject.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly StorageHelper _storageHelper;
        private readonly UserManager<User> UserManager;
        private readonly IConfiguration Configuration;
        private readonly JwtService JwtService;

        public AdminController(StorageHelper storageHelper, ApplicationDbContext dbContext, UserManager<User> userManager, IConfiguration configuration, JwtService jwtService)
        {
            _storageHelper = storageHelper;
            _dbContext = dbContext;
            UserManager = userManager;
            Configuration = configuration;
            JwtService = jwtService;
        }

        [Authorize]
        public IActionResult Index()
        {
            var model = Solutions();

            return View(model);
        }

        [Authorize]
        public IActionResult SolutionOverview([FromRoute] int id)
        {
            var solution = _dbContext.Solutions
                .Include(x => x.Users)
                .Include(x => x.Pages).FirstOrDefault(x => x.Id == id);
            var model = new PageOverview
            {
                Pages = solution?.Pages,
                SolutionId = id,
                Solution = solution
            };

            return View(model);
        }

        [Authorize]
        private IEnumerable<Solution> Solutions()
        {
            var result = new List<Solution>();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId)) return result;

            try
            {
                result.AddRange(DatabaseHelper.GetSolutionsByUser(userId, _dbContext));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return result;
            }
            return result;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Index(string title, string users)
        {
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userList = !string.IsNullOrEmpty(users) ? users.Split(",") : new string[0];
            var databaseUsers = _dbContext.Users.Where(user => userList.Contains(user.UserName) || user.Id == userid);
            var project = new Solution()
            {
                Subdomain = title,
                Users = databaseUsers.ToList(),
            };
            _dbContext.Solutions.Add(project);
            _dbContext.Permissions.AddRange(databaseUsers.Select(user => new Permission()
            {
                PermissionLevel = user.Id == userid ? PermissionLevel.Admin : PermissionLevel.Editor,
                User = user,
                Solution = project
            }));
            await _dbContext.SaveChangesAsync();
            return Index();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreatePage(string title, int id)
        {
            try
            {
                if (title == null)
                {
                    title = "";
                }
                var page = new Page()
                {
                    PageName = title,
                    Solution = await _dbContext.Solutions.FindAsync(id)
                };

                _dbContext.Pages.Add(page);
                await _dbContext.SaveChangesAsync();

                return RedirectToAction("SolutionOverview", new { id = id });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return RedirectToAction("SolutionOverview", e);
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> RemovePage(int id, int solutionId)
        {
            try
            {
                var page = await _dbContext.Pages.FindAsync(id);
                if (page != null)
                {
                    _dbContext.Pages.Remove(page);
                }

                await _dbContext.SaveChangesAsync();
                return RedirectToAction("SolutionOverview", new { id = solutionId });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return RedirectToAction("SolutionOverview", e);
            }
        }

        /// <summary>
        /// SDE = Solution doesn't exist
        /// UDE = User doesn't exist
        /// </summary>
        /// <param name="id"></param>
        /// <param name="solutionId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> RemoveUser(string id, int solutionId)
        {
            try
            {
                var solution = await _dbContext.Solutions.Include(s => s.Users).FirstOrDefaultAsync(x => x.Id == solutionId);
                var user = await _dbContext.Users.Include(y => y.Solutions).FirstOrDefaultAsync(x => x.Id == id);

                if (user == null || !user.Solutions.Contains(solution)) return BadRequest("Something went wrong. (SDE/UDE)");

                solution.Users.Remove(user);
                await _dbContext.SaveChangesAsync();

                return RedirectToAction("SolutionOverview", new { id = solutionId });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return RedirectToAction("SolutionOverview", e);
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddUser(string userEmail, int solutionId)
        {
            try
            {
                var solution = await _dbContext.Solutions.Include(solution => solution.Users).FirstOrDefaultAsync(x => x.Id == solutionId);
                var user = _dbContext.Users.FirstOrDefault(x => x.Email == userEmail);

                if (user == null) return RedirectToAction("SolutionOverview", new { id = solutionId });

                solution.Users.Add(user);

                await _dbContext.SaveChangesAsync();

                return RedirectToAction("SolutionOverview", new { id = solutionId });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return RedirectToAction("SolutionOverview", e);
            }
        }

        /// <summary>
        /// Deletes a selected solution. No Cascade-deletion --> Include Permissions, Page and users.
        /// .Clear() removes all table-entities in the given context.
        /// UPDE = UserPermission doesn't exist.
        /// NUF = No users found.
        /// NPF = No permissions found.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> DeleteSolution(int id)
        {
            try
            {
                var currentSolution = await _dbContext.Solutions.Include(x => x.Permissions).Include(y => y.Pages).Include(x => x.Users).FirstOrDefaultAsync(x => x.Id == id);

                if (currentSolution == null) return BadRequest("Something went wrong.");
                if (currentSolution.Users == null) return BadRequest("Something went wrong. (NUF)");
                if (currentSolution.Permissions == null) return BadRequest("Something went wrong. (NPF)");

                currentSolution.Permissions.Clear();
                currentSolution.Users.Clear();
                _dbContext.Solutions.Remove(currentSolution);

                await _dbContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return RedirectToAction("index", e);
            }
        }

        /// <summary>
        /// Edits a selected solution.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> EditSolution(int id)
        {
            try
            {
                var currentSolution = await _dbContext.Solutions.Include(x => x.Permissions).Include(y => y.Pages).Include(x => x.Users).FirstOrDefaultAsync(x => x.Id == id);

                if (currentSolution == null) return BadRequest("Something went wrong.");
                if (currentSolution.Users == null) return BadRequest("Something went wrong. (NUF)");
                if (currentSolution.Permissions == null) return BadRequest("Something went wrong. (NPF)");

                currentSolution.Domain = "";

                await _dbContext.SaveChangesAsync();
                return RedirectToAction("index");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return RedirectToAction("index", e);
            }
        }

        /// <summary>
        /// This handler redirects to the login handler on the solution.
        /// </summary>
        /// <param name="id">The id for the solution to be logged in on.</param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> LoginToSolution(int id)
        {
            // It is important that we use the user that is logged in.
            var user = await UserManager.GetUserAsync(HttpContext.User);
            var solution = _dbContext.Solutions.Find(id);
            var token = new JwtSecurityTokenHandler().WriteToken(JwtService.CreateOneTimeToken(user));
            // TODO: use solution helper
            return Redirect($"https://{solution.Subdomain}.{Configuration["RootDomain"]}:5001/Login/?token={token}");
        }
    }
}
