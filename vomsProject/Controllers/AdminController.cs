using Microsoft.AspNetCore.Authorization;
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
        private readonly DomainHelper DomainHelper;
        private readonly SolutionHelper SolutionHelper;

        public AdminController(StorageHelper storageHelper, ApplicationDbContext dbContext, UserManager<User> userManager, IConfiguration configuration, JwtService jwtService, DomainHelper domainHelper, SolutionHelper solutionHelper)
        {
            _storageHelper = storageHelper;
            _dbContext = dbContext;
            UserManager = userManager;
            Configuration = configuration;
            JwtService = jwtService;
            DomainHelper = domainHelper;
            SolutionHelper = solutionHelper;
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
                .AsSplitQuery()
                .Include(x => x.Permissions).ThenInclude(x => x.User)
                .Include(x => x.Pages).FirstOrDefault(x => x.Id == id);

            var stylesheets = _dbContext.Styles.Select(style => new Option() { Id = style.Id, Text = style.Name }).ToList();
            if (solution != null)
            {
                var model = new PageOverview
                {
                    Pages = solution.Pages,
                    SolutionId = id,
                    Solution = solution,
                    StyleSheets = stylesheets,
                    SelectedStyleId = solution.StyleId.HasValue ? solution.StyleId.Value : null
                };

                return View(model);
            }
            return NotFound();
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
            var style = await _dbContext.Styles.FirstOrDefaultAsync();
            var project = new Solution()
            {
                Subdomain = title,
                Style = style
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
        public async Task<IActionResult> UpdateSolution(int stylesheet, int solutionId)
        {
            var user = await UserManager.GetUserAsync(HttpContext.User);
            var solution = _dbContext.Solutions.Find(solutionId);
            if (solution == null || !await SolutionHelper.DoUserHavePermissionOnSolution(user, solution, PermissionLevel.Admin))
            {
                return Forbid();
            }

            solution.StyleId = stylesheet;
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Index");
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
                var permission = _dbContext.Permissions.Include(perm => perm.User).Include(perm => perm.Solution).Where(perm => perm.User.Id == id && perm.Solution.Id == solutionId);
                _dbContext.Permissions.RemoveRange(permission);

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
                var solution = await _dbContext.Solutions.Include(solution => solution.Permissions).FirstOrDefaultAsync(x => x.Id == solutionId);
                var user = _dbContext.Users.FirstOrDefault(x => x.Email == userEmail);

                if (user == null) return RedirectToAction("SolutionOverview", new { id = solutionId });

                solution.Permissions.Add(new Permission() 
                {
                    PermissionLevel = PermissionLevel.Editor,
                    User = user
                });

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
                var currentSolution = await _dbContext.Solutions.Include(x => x.Permissions).Include(y => y.Pages).FirstOrDefaultAsync(x => x.Id == id);

                if (currentSolution == null) return BadRequest("Something went wrong.");
                if (currentSolution.Permissions == null) return BadRequest("Something went wrong. (NPF)");

                currentSolution.Permissions.Clear();
                currentSolution.Pages.Clear();
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
                var currentSolution = await _dbContext.Solutions.Include(x => x.Permissions).Include(y => y.Pages).FirstOrDefaultAsync(x => x.Id == id);

                if (currentSolution == null) return BadRequest("Something went wrong.");
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
        /// <param name="pageName">Page to redirect to after login. This paramter is optional and defaults to the index page.</param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> LoginToSolution(int id, string pageName)
        {
            // It is important that we use the user that is logged in.
            var user = await UserManager.GetUserAsync(HttpContext.User);
            var solution = _dbContext.Solutions.Find(id);
            var token = new JwtSecurityTokenHandler().WriteToken(JwtService.CreateOneTimeToken(user));
            if (pageName == null)
            {
                return Redirect($"{DomainHelper.GetIndexPageUrl(solution)}Login?token={token}");
            }
            else
            {
                return Redirect($"{DomainHelper.GetIndexPageUrl(solution)}Login?token={token}&pageName={pageName}");
            }
        }
    }
}
