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
using System.Text;
using System.IO;
using Microsoft.AspNetCore.Http;

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
        private readonly RepositoryService Repository;
        private readonly OperationsService _operationsService;

        public AdminController(StorageHelper storageHelper, ApplicationDbContext dbContext, UserManager<User> userManager,
            IConfiguration configuration, JwtService jwtService, DomainHelper domainHelper, RepositoryService repository, OperationsService operationsService)
        {
            _storageHelper = storageHelper;
            _dbContext = dbContext;
            UserManager = userManager;
            Configuration = configuration;
            JwtService = jwtService;
            DomainHelper = domainHelper;
            Repository = repository;
            _operationsService = operationsService;
        }

        /// <summary>
        /// The default admin view containing a list over all "permitted" solutions, i.e. the solutions a logged in user has access to.
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public async Task<IActionResult> IndexAsync()
        {
            var user = await UserManager.GetUserAsync(HttpContext.User);
            var model = new AdminViewModel()
            {
                Solutions = await Repository.GetSolutionsByUser(user)
            };

            return View(model);
        }

        /// <summary>
        /// Creates a solution based on the logged in user. If a users productVersions solution-limitation is exceeded, a user is not permitted to create a new solution.
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Index(string title)
        {
            var maxSolutions = 0;
            var user = await UserManager.GetUserAsync(HttpContext.User);
            var productVersion = user.ProductVersion;
            var style = await _dbContext.Styles.FirstOrDefaultAsync();

            switch (productVersion)
            {
                case ProductType.Community:
                    maxSolutions = 1;
                    break;
                case ProductType.Professional:
                    maxSolutions = 5;
                    break;
                case ProductType.Enterprise:
                    maxSolutions = 20;
                    break;
                default:
                    break;
            }

            await using (var dbContextTransaction = await _dbContext.Database.BeginTransactionAsync())
            {
                var query = _dbContext.Permissions.Where(x => x.User.Id == user.Id && x.PermissionLevel == PermissionLevel.Admin);
                var numberOfSolutions = query.Count();

                if (numberOfSolutions < maxSolutions)
                {
                    var project = new Solution()
                    {
                        Subdomain = title,
                        Style = style
                    };

                    _dbContext.Permissions.Add(new Permission
                    {
                        PermissionLevel = PermissionLevel.Admin,
                        User = user,
                        Solution = project
                    });

                    await _dbContext.SaveChangesAsync();
                    await dbContextTransaction.CommitAsync();

                    return RedirectToAction("Index");
                }
                else
                {
                    var model = new AdminViewModel()
                    {
                        Solutions = await Repository.GetSolutionsByUser(user),
                        HasReachedProductLimit = true
                    };

                    return View(model);
                }
            }
        }

        /// <summary>
        /// The view which contains all data about and involving a given solution.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public async Task<IActionResult> SolutionOverview([FromRoute] int id)
        {
            var user = await UserManager.GetUserAsync(HttpContext.User);
            var solution = Repository.GetSolutionById(id);
            var theSolution = await solution
                .Include(x => x.Permissions).ThenInclude(x => x.User)
                .Include(x => x.Style)
                .SingleOrDefaultAsync();
            if (theSolution == null || !await Repository.IsUserOnSolution(theSolution, user))
            {
                return Forbid();
            }

            var options = new List<StylesheetCustomizations>();
            var stylesheetKeyValue = new Dictionary<string, string>();
            var customizations = theSolution.StylesheetCustomization.Split(';');

            foreach (var customization in customizations)
            {
                var keyValue = customization.Split(':');
                if (keyValue.Length == 2)
                {
                    stylesheetKeyValue.Add(keyValue[0], keyValue[1]);
                }
            }

            foreach (var option in theSolution.Style.StylesheetOptions.Split(';'))
            {
                var settings = option.Split(',');
                if (settings.Length == 4)
                {
                    options.Add(new StylesheetCustomizations
                    {
                        VariableName = settings[0],
                        Value = stylesheetKeyValue[settings[1]],
                        FriendlyName = settings[2],
                        Type = settings[3]
                    });
                }
            }

            var stylesheets = await _dbContext.Styles.Select(style => new Option() { Id = style.Id, Text = style.Name }).ToListAsync();
            var pages = await Repository.Pages(solution).
                Include(x => x.PublishedVersion).
                ToListAsync();

            var model = new PageOverview
            {
                Pages = pages,
                SolutionId = id,
                Solution = theSolution,
                StyleSheets = stylesheets,
                User = user,
                SelectedStyleId = theSolution.StyleId,
                Favicon = theSolution.Favicon,
                Fonts = new List<string>()
                {
                    "Calibri",
                    "Arial",
                    "Times New Roman",
                    "Comic sans"
                },
                StylesheetCustomizations = options
            };

            return View(model);
        }

        /// <summary>
        /// Update a solution with the following params: 
        /// </summary>
        /// <param name="stylesheet"></param>
        /// <param name="solutionId"></param>
        /// <param name="friendlyName"></param>
        /// <param name="domainName"></param>
        /// <param name="favicon"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateSolution(int stylesheet, int solutionId, string friendlyName = null, string domainName = null,
            List<IFormFile> favicon = null)
        {
            var user = await UserManager.GetUserAsync(HttpContext.User);
            var theSolution = Repository.GetSolutionById(solutionId);
            byte[] fileBytes;

            foreach (var file in favicon)
            {
                if (file.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        await file.CopyToAsync(ms);
                        fileBytes = ms.ToArray();
                    }
                    await _operationsService.UpdateSolution(user, theSolution, friendlyName, domainName, stylesheet, fileBytes);
                }
                else
                {
                    await _operationsService.UpdateSolution(user, theSolution, friendlyName, domainName, stylesheet);
                }
            }



            return RedirectToAction("Index");
        }

        /// <summary>
        /// Create a page in a solution.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreatePage(string title, int id)
        {
            var solution = Repository.GetSolutionById(id);
            var theSolution = await solution.SingleOrDefaultAsync();

            title ??= "";
            return RedirectToAction("LoginToSolution", "Admin", new { id = theSolution.Id, title });
        }

        /// <summary>
        /// Remove a page from a solution.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="solutionId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> RemovePage(int id, int solutionId)
        {
            var user = await UserManager.GetUserAsync(HttpContext.User);
            var theSolution = Repository.GetSolutionById(solutionId);
            try
            {
                await _operationsService.RemovePage(user, theSolution, id);

                return RedirectToAction("SolutionOverview", new { id = solutionId });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return RedirectToAction("SolutionOverview");
            }
        }

        /// <summary>
        /// Remove a user from a solution.
        /// </summary>
        /// <param name="id">The user id</param>
        /// <param name="solutionId">The solution id</param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> RemoveUser(string id, int solutionId)
        {
            var user = await UserManager.GetUserAsync(HttpContext.User);
            var theSolution = Repository.GetSolutionById(solutionId);

            try
            {
                await _operationsService.RemoveUser(user, theSolution, id);

                return RedirectToAction("SolutionOverview", new { id = solutionId });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return RedirectToAction("SolutionOverview", e);
            }
        }

        /// <summary>
        /// Add a user to solution.
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="solutionId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddUser(string userEmail, int solutionId)
        {
            var user = await UserManager.GetUserAsync(HttpContext.User);
            var theSolution = Repository.GetSolutionById(solutionId);

            try
            {
                await _operationsService.AddUser(user, theSolution, userEmail);

                return RedirectToAction("SolutionOverview", new { id = solutionId });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return RedirectToAction("SolutionOverview");
            }
        }

        /// <summary>
        /// Deletes a selected solution and related entities.
        /// </summary>
        /// <param name="id">The solution Id</param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> DeleteSolution(int id)
        {
            var user = await UserManager.GetUserAsync(HttpContext.User);
            var theSolution = Repository.GetSolutionById(id);

            try
            {
                await _operationsService.DeleteSolution(user, theSolution, id);

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return RedirectToAction("index");
            }
        }

        /// <summary>
        /// This handler redirects to the login handler on the solution.
        /// </summary>
        /// <param name="id">The id for the solution to be logged in on.</param>
        /// <param name="pageName">Page to redirect to after login. This parameter is optional and defaults to the index page.</param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> LoginToSolution(int id, string pageName)
        {
            var user = await UserManager.GetUserAsync(HttpContext.User);
            var solution = await _dbContext.Solutions.FindAsync(id);
            var token = new JwtSecurityTokenHandler().WriteToken(JwtService.CreateOneTimeToken(user));
            if (pageName == null)
            {
                return Redirect($"{DomainHelper.GetSolutionIndexPageUrl(solution)}Login?token={token}");
            }
            else
            {
                return Redirect($"{DomainHelper.GetSolutionIndexPageUrl(solution)}Login?token={token}&pageName={Uri.EscapeDataString(pageName)}");
            }
        }

        /// <summary>
        /// The Page Recycling Bin, where you can see the deleted pages on a solution.
        /// </summary>
        /// <param name="id">The id of the solution</param>
        /// <returns></returns>
        [Authorize]
        public async Task<IActionResult> DeletedPages(int id)
        {
            var user = await UserManager.GetUserAsync(HttpContext.User);
            var solution = Repository.GetSolutionById(id);
            var theSolution = await solution.SingleOrDefaultAsync();
            if (theSolution == null || !await Repository.IsUserOnSolution(theSolution, user))
            {
                return Forbid();
            }

            var pages = await Repository.DeletedPages(solution).ToListAsync();
            var model = new DeletedPagesViewModel()
            {
                Solution = theSolution,
                Pages = pages
            };

            return View(model);
        }

        /// <summary>
        /// Recover a deleted page to a solution.
        /// </summary>
        /// <param name="id">The id of the solution</param>
        /// <param name="pageId">The id of the page</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeletedPages(int id, int pageId)
        {
            var user = await UserManager.GetUserAsync(HttpContext.User);
            var solution = Repository.GetSolutionById(id);
            var theSolution = await solution.SingleOrDefaultAsync();
            if (theSolution == null || !await Repository.IsUserOnSolution(theSolution, user))
            {
                return Forbid();
            }

            var pages = await Repository.DeletedPages(solution).ToListAsync();
            var page = pages.FirstOrDefault(page => page.Id == pageId);
            if (page == null)
            {
                var model = new DeletedPagesViewModel()
                {
                    Pages = pages,
                    RecoverFailureReason = RecoverFailureReason.IsNolongerInTrash,
                    FailedToRecoverPageId = pageId
                };
                return View(model);
            }
            var replacingPage = await Repository.PageQuery(solution, page.PageName).AnyAsync();
            if (replacingPage)
            {
                var model = new DeletedPagesViewModel()
                {
                    Pages = pages,
                    RecoverFailureReason = RecoverFailureReason.HasBeenReplaced,
                    FailedToRecoverPageId = pageId
                };
                return View(model);
            }
            page.IsDeleted = false;
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("DeletedPages");
        }

        /// <summary>
        /// Just for testing - Should be moved into the pageController.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="pageId"></param>
        /// <param name="solutionId"></param>
        /// <returns></returns>
        public async Task<IActionResult> UploadImage(List<IFormFile> image, int pageId, int solutionId)
        {
            var solution = Repository.GetSolutionById(solutionId);
            var theSolution = await solution.SingleOrDefaultAsync();
            var pages = Repository.Pages(solution);
            var page = await pages.FirstOrDefaultAsync(x => x.Id == pageId);

            try
            {
                var imageToUpload = image.FirstOrDefault();

                var d = await _storageHelper.UploadToBlob(imageToUpload.OpenReadStream(), _dbContext, page, imageToUpload.FileName);

                return RedirectToAction("SolutionOverview", new { id = theSolution.Id });
            }
            catch (Exception e)
            {
                return RedirectToAction("SolutionOverview", new { id = theSolution.Id });
            }
        }

        /// <summary>
        /// Retrieves the solutionsFavicon - This is currently used as a preview of what Favicon is selected.
        /// </summary>
        /// <param name="solutionId"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetSolutionFavicon(int solutionId)
        {
            var solution = await Repository.GetSolutionById(solutionId).FirstOrDefaultAsync();
            if (solution.Favicon == null)
                return NotFound();

            return new FileContentResult(solution.Favicon, "image/png");
        }


        public async Task<IActionResult> SetStyleVariables([FromBody] Dictionary<string, string> variables, int solutionId)
        {
            var solution = await Repository.GetSolutionById(solutionId).FirstOrDefaultAsync();
            var sb = new StringBuilder();
            foreach (var variable in variables)
            {
                sb.Append($"{variable.Key}:{variable.Value};");
            }
            solution.StylesheetCustomization = sb.ToString();
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("SolutionOverview", solutionId);
        }
    }
}
