using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vomsProject.Data;
using vomsProject.Helpers;
using vomsProject.SolutionPages;

/*
 This controller is the main handler for request to a solution.
 */
namespace vomsProject.Controllers
{
    public class PageController : Controller
    {
        private readonly ILogger<PageController> _logger;
        private readonly IConfiguration Configuration;
        private readonly ApplicationDbContext Context;
        private readonly UserManager<User> UserManager;
        private readonly SignInManager<User> SignInManager;
        private readonly string RootDomain;
        private readonly RepositoryService _solutionHelper;
        private readonly DomainHelper _domainHelper;
        private readonly JwtService JwtService;

        public PageController(ILogger<PageController> logger, IConfiguration configuration,
            ApplicationDbContext context, UserManager<User> userManager, RepositoryService solutionHelper, DomainHelper domainHelper, SignInManager<User> signInManager, JwtService jwtService)
        {
            _logger = logger;
            Configuration = configuration;
            Context = context;
            UserManager = userManager;
            SignInManager = signInManager;
            RootDomain = Configuration["RootDomain"];
            _solutionHelper = solutionHelper;
            _domainHelper = domainHelper;
            JwtService = jwtService;
        }

        /// <summary>
        /// This is the page dispatcher. Each request for a page on a solution is looked up by this handler.
        /// </summary>
        /// <param name="pageName">The name of the page</param>
        /// <returns></returns>
        public async Task<IActionResult> Index(string pageName)
        {
            pageName ??= "";
            var user = await UserManager.GetUserAsync(HttpContext.User);
            var solution = _solutionHelper.GetSolutionByDomainName(Request.Host.Host);
            var theSolution = await solution.SingleOrDefaultAsync();
            if (user != null)
            {
                if (theSolution != null && await _solutionHelper.IsUserOnSolution(theSolution, user))
                {
                    var page = await _solutionHelper.PageQuery(solution, pageName)
                        .Include((page) => page.LastSavedVersion)
                        .Include((page) => page.PublishedVersion)
                        .Include((page) => page.Layout)
                        .SingleOrDefaultAsync();
                    if (page == null)
                    {
                        page = new Page()
                        {
                            PageName = pageName,
                            HtmlRenderContent = "",
                            IsPublished = false,
                            Solution = theSolution,
                            IsDeleted = false,
                            Layout = await solution.Select(solution => solution.DefaultLayout).FirstOrDefaultAsync()
                        };
                        Context.Add(page);
                        await Context.SaveChangesAsync();
                    }
                    var versions = await Context.PageContents.Where(content => content.Page == page).Select(content => new EditablePageModel.Version()
                    {
                        id = content.Id,
                        saveDate = content.SaveDate.ToString("yyyy-MM-dd HH:mm")
                    }).ToListAsync();

                    var content = PageContentUtil.ConstructPageContent(page);

                    return new EditablePageResult(new EditablePageModel()
                    {
                        id = page.Id,
                        content = JsonConvert.SerializeObject(content),
                        title = page.Title,
                        isPublished = page.IsPublished ? "true" : "false",
                        publishedVersion = page.PublishedVersion != null ? page.PublishedVersion.Id.ToString() : "null",
                        publishedDate = page.PublishedVersion != null ? page.PublishedVersion.SaveDate.ToString("yyyy-MM-dd HH:mm") : "",
                        savedVersion = page.LastSavedVersion != null ? page.LastSavedVersion.Id.ToString() : "null",
                        savedDate = page.LastSavedVersion != null ? page.LastSavedVersion.SaveDate.ToString("yyyy-MM-dd HH:mm") : "",
                        versions = versions,
                        styleVariables = theSolution.SerializedStylesheet,
                        layoutSaveDate = page.Layout != null ? page.Layout.SaveDate.ToString("yyyy-MM-dd HH:mm") : ""
                    });
                }
            }
            var publishedPage = await _solutionHelper.PageQuery(solution, pageName)
                .Where((page) => page.IsPublished)
                .Include((page) => page.Layout)
                .SingleOrDefaultAsync();
            if (publishedPage != null)
            {
                return new PageResult(new PageModel()
                {
                    content = publishedPage.HtmlRenderContent,
                    title = publishedPage.Title,
                    header = publishedPage.Layout != null ? publishedPage.Layout.HeaderContent : "",
                    footer = publishedPage.Layout != null ? publishedPage.Layout.FooterContent : "",
                    styleVariables = theSolution.SerializedStylesheet
                });
            }
            if (theSolution != null)
            {
                return new Status404PageResult(new Page404Model()
                {
                    // TODO: Could we get a better name?
                    solutionName = theSolution.Subdomain,
                    message = "Make sure you typed the url correctly."
                });
            }
            else
            {
                return new Status404PageResult(new Page404Model()
                {
                    solutionName = "Non existing website",
                    message = $"There is no website on the requested domain: {Request.Host.Host}."
                });
            }
        }

        /// <summary>
        /// Return the stylesheet for a solution.
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Style()
        {
            var solution = _solutionHelper.GetSolutionByDomainName(Request.Host.Host);
            var style = await solution.Include(solution => solution.Style).Select(solution => solution.Style).SingleOrDefaultAsync();

            return new FileContentResult(Encoding.UTF8.GetBytes(style != null ? style.Css : ""), "text/css");
        }
        /// <summary>
        /// Log in a user on the solution. On success redirect to index page. On failure redirect to the CMS page.
        /// </summary>
        /// <param name="token">Jwt token used to authenticate. The token subject is the id for the user.</param>
        /// <param name="pageName">Page to redirect to after login. This parameter is optional and defaults to the index page.</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Login(string token, string pageName)
        {
            var solution = _solutionHelper.GetSolutionByDomainName(Request.Host.Host);
            try
            {
                new JwtSecurityTokenHandler().ValidateToken(token, JwtService.TokenValidationParamters, out var loginToken);
                var jwtToken = (JwtSecurityToken)loginToken;
                var userId = jwtToken.Subject;
                var user = await UserManager.FindByIdAsync(userId);
                await SignInManager.SignInAsync(user, true);

                if (pageName != null)
                {
                    var page = await _solutionHelper.PageQuery(solution, pageName).Include((page) => page.Solution).SingleOrDefaultAsync();
                    if (page != null)
                    {
                        return Redirect(_domainHelper.GetPageUrl(page));
                    }
                }
                var theSolution = await solution.SingleOrDefaultAsync();
                return Redirect(_domainHelper.GetSolutionIndexPageUrl(theSolution));
            }
            catch
            {
                // TODO: The redirects below might benefit from some abstraction. Like we have with DomainHelper.

                // Failed to authenticate
                var theSolution = await solution.SingleOrDefaultAsync();
                if (theSolution == null)
                {
                    // For normal users this would be if a solution has been deleted, or the domain has been changed.
                    // In that case they might like to see the over view of solutions they have access to.
                    return Redirect($"https://{RootDomain}:5001/Admin/Index");
                }
                // If it is just an old token that got resend somehow. The user might like to get back to where they can generate a new token.
                if (pageName != null)
                {
                    return Redirect($"https://{RootDomain}:5001/Admin/LoginToSolution/{theSolution.Id}?pageName={pageName}");
                }
                else
                {
                    return Redirect($"https://{RootDomain}:5001/Admin/LoginToSolution/{theSolution.Id}");
                }
            }
        }

        /// <summary>
        /// Log out a user from the solution. Then redirect to index page.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await SignInManager.SignOutAsync();
            var solution = _solutionHelper.GetSolutionByDomainName(Request.Host.Host).FirstOrDefault();
            return Redirect(_domainHelper.GetSolutionIndexPageUrl(solution));
        }
    }
}
