using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vomsProject.Data;
using vomsProject.Helpers;

namespace vomsProject.Controllers.Api
{
    public class PublishDto
    {
        public PublishDto(string html, object content)
        {
            Html = html;
            Content = content;
        }

        public object Content { get; private set; }
        public string Html { get; private set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class PageController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly SolutionHelper SolutionHelper;
        private readonly UserManager<User> UserManager;
        public PageController(ApplicationDbContext context, SolutionHelper solutionHelper, UserManager<User> userManager)
        {
            _context = context;
            SolutionHelper = solutionHelper;
            UserManager = userManager;
        }

        [Authorize]
        [Route("{id}/update")]
        [HttpPost]
        public async Task<IActionResult> Update(int id, [FromBody] object body)
        {
            var user = await UserManager.GetUserAsync(HttpContext.User);
            var solution = SolutionHelper.GetSolutionByDomainName(Request.Host.Host);
            var theSolution = await solution.SingleOrDefaultAsync();
            if (theSolution == null || !await SolutionHelper.IsUserOnSolution(theSolution, user))
            {
                return Forbid();
            }

            var page = await _context.Pages.Include(page => page.Versions).FirstOrDefaultAsync(page => page.Id == id);
            var content = new PageContent()
            {
                Content = body.ToString(),
                SavedBy = user,
                SaveDate = DateTime.UtcNow,
                Page = page
            };
            page.Versions.Add(content);
            page.LastSavedVersion = content;

            await _context.SaveChangesAsync();
            return Ok();
        }

        [Authorize]
        [Route("{id}/publish")]
        [HttpPost]
        public async Task<IActionResult> Publish(int id, [FromBody] PublishDto body)
        {
            var user = await UserManager.GetUserAsync(HttpContext.User);
            var solution = SolutionHelper.GetSolutionByDomainName(Request.Host.Host);
            var theSolution = await solution.SingleOrDefaultAsync();
            if (theSolution == null || !await SolutionHelper.IsUserOnSolution(theSolution, user))
            {
                return Forbid();
            }

            var page = await _context.Pages.Include(page => page.Versions).FirstOrDefaultAsync(page => page.Id == id);
            var content = new PageContent()
            {
                Content = body.Content.ToString(),
                SavedBy = user,
                SaveDate = DateTime.UtcNow,
                Page = page
            };
            page.Versions.Add(content);
            page.LastSavedVersion = content;
            page.PublishedVersion = content;
            page.HtmlRenderContent = body.Html;
            page.IsPublished = true;

            await _context.SaveChangesAsync();
            return Ok();
        }

        [Authorize]
        [Route("{id}/unpublish")]
        [HttpPost]
        public async Task<IActionResult> UnPublish(int id)
        {
            var user = await UserManager.GetUserAsync(HttpContext.User);
            var solution = SolutionHelper.GetSolutionByDomainName(Request.Host.Host);
            var theSolution = await solution.SingleOrDefaultAsync();
            if (theSolution == null || !await SolutionHelper.IsUserOnSolution(theSolution, user))
            {
                return Forbid();
            }

            var page = await _context.Pages.FirstOrDefaultAsync(page => page.Id == id);

            page.PublishedVersion = null;
            page.HtmlRenderContent = "";
            page.IsPublished = false;

            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
