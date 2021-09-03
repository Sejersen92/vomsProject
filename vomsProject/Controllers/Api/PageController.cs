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

/*
 This controller is the api used to manage pages.
 */
namespace vomsProject.Controllers.Api
{
    /// <summary>
    /// Dto used to publish a page.
    /// </summary>
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

    /// <summary>
    /// Dto used to change the which version is the latest.
    /// </summary>
    public class SetLastVersionDto
    {
        public int VersionId { get; set; }
    }
    /// <summary>
    /// Dto used when saveing the properties.
    /// </summary>
    public class PropertiesDto
    {
        public string Title { get; set; }
    }
    /// <summary>
    /// Result of saveing a page. This is used as the result for both save and publish.
    /// </summary>
    public class SaveResult
    {
        public int LatestVersion { get; set; }
        public string SaveDate { get; set; }
    }
    [Route("api/[controller]")]
    [ApiController]
    public class PageController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly RepositoryService Repository;
        private readonly UserManager<User> UserManager;
        public PageController(ApplicationDbContext context, RepositoryService repository, UserManager<User> userManager)
        {
            _context = context;
            Repository = repository;
            UserManager = userManager;
        }

        /// <summary>
        /// Save a new version and set it as the latest version.
        /// </summary>
        /// <param name="id">The id of the page</param>
        /// <param name="body">The quill delta</param>
        /// <returns></returns>
        [Authorize]
        [Route("{id}/update")]
        [HttpPost]
        public async Task<IActionResult> Update(int id, [FromBody] object body)
        {
            var user = await UserManager.GetUserAsync(HttpContext.User);
            var solution = Repository.GetSolutionByDomainName(Request.Host.Host);
            var theSolution = await solution.SingleOrDefaultAsync();
            if (theSolution == null || !await Repository.IsUserOnSolution(theSolution, user))
            {
                return Forbid();
            }

            var page = await Repository.Pages(solution)
                .Include(page => page.Versions)
                .FirstOrDefaultAsync(page => page.Id == id);
            if (page == null)
            {
                return NotFound();
            }
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
            return Ok(new SaveResult 
            { 
                LatestVersion = content.Id,
                SaveDate = content.SaveDate.ToString("yyyy-MM-dd HH:mm")
            });
        }

        /// <summary>
        /// Publish and Save a new version. Also set this version as the the latest version.
        /// </summary>
        /// <param name="id">The id of the page</param>
        /// <param name="body">The published version including The quill delta and, the rendered html</param>
        /// <returns></returns>
        [Authorize]
        [Route("{id}/publish")]
        [HttpPost]
        public async Task<IActionResult> Publish(int id, [FromBody] PublishDto body)
        {
            var user = await UserManager.GetUserAsync(HttpContext.User);
            var solution = Repository.GetSolutionByDomainName(Request.Host.Host);
            var theSolution = await solution.SingleOrDefaultAsync();
            if (theSolution == null || !await Repository.IsUserOnSolution(theSolution, user))
            {
                return Forbid();
            }

            var page = await Repository.Pages(solution)
                .Include(page => page.Versions)
                .FirstOrDefaultAsync(page => page.Id == id);
            if (page == null)
            {
                return NotFound();
            }
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
            return Ok(new SaveResult 
            { 
                LatestVersion = content.Id,
                SaveDate = content.SaveDate.ToString("yyyy-MM-dd HH:mm")
            });
        }

        /// <summary>
        /// Set the publish status as false.
        /// </summary>
        /// <param name="id">The id of the page</param>
        /// <returns></returns>
        [Authorize]
        [Route("{id}/unpublish")]
        [HttpPost]
        public async Task<IActionResult> UnPublish(int id)
        {
            var user = await UserManager.GetUserAsync(HttpContext.User);
            var solution = Repository.GetSolutionByDomainName(Request.Host.Host);
            var theSolution = await solution.SingleOrDefaultAsync();
            if (theSolution == null || !await Repository.IsUserOnSolution(theSolution, user))
            {
                return Forbid();
            }

            var page = await Repository.Pages(solution)
                .FirstOrDefaultAsync(page => page.Id == id);
            if (page == null)
            {
                return NotFound();
            }

            page.PublishedVersion = null;
            page.HtmlRenderContent = "";
            page.IsPublished = false;

            await _context.SaveChangesAsync();
            return Ok();
        }

        /// <summary>
        /// Get the quill delta for a specified version.
        /// </summary>
        /// <param name="id">The id of the page</param>
        /// <param name="versionId">The id of the version</param>
        /// <returns></returns>
        [Authorize]
        [Route("{id}/Version/{versionId}")]
        [HttpGet]
        public async Task<IActionResult> GetVersion(int id, int versionId)
        {
            var user = await UserManager.GetUserAsync(HttpContext.User);
            var solution = Repository.GetSolutionByDomainName(Request.Host.Host);
            var theSolution = await solution.SingleOrDefaultAsync();
            if (theSolution == null || !await Repository.IsUserOnSolution(theSolution, user))
            {
                return Forbid();
            }

            var content = await Repository.Pages(solution)
                .Where(page => page.Id == id)
                .SelectMany(page => page.Versions)
                .Where(version => version.Id == versionId)
                .FirstOrDefaultAsync();

            if (content != null)
            {
                return Ok(content);
            }
            return NotFound();
        }

        /// <summary>
        /// Used to change wich version is the latest version.
        /// </summary>
        /// <param name="id">The id of the page</param>
        /// <param name="body">Contains the version id to set as the latest</param>
        /// <returns></returns>
        [Authorize]
        [Route("{id}/SetAsLastVersion")]
        [HttpPost]
        public async Task<IActionResult> SetAsLastVersion(int id, [FromBody] SetLastVersionDto body)
        {
            var user = await UserManager.GetUserAsync(HttpContext.User);
            var solution = Repository.GetSolutionByDomainName(Request.Host.Host);
            var theSolution = await solution.SingleOrDefaultAsync();
            if (theSolution == null || !await Repository.IsUserOnSolution(theSolution, user))
            {
                return Forbid();
            }

            var page = Repository.Pages(solution)
                .Where(page => page.Id == id);

            var content = await page.SelectMany(page => page.Versions)
                .Where(version => version.Id == body.VersionId)
                .FirstOrDefaultAsync();
            var thePage = await page.FirstOrDefaultAsync();
            if (thePage != null && content != null)
            {
                thePage.LastSavedVersion = content;
                await _context.SaveChangesAsync();
                return Ok(new SaveResult 
                { 
                    LatestVersion = content.Id,
                    SaveDate = content.SaveDate.ToString("yyyy-MM-dd HH:mm")
                });
            }
            return NotFound();
        }

        [Authorize]
        [Route("{id}/properties/update")]
        [HttpPost]
        public async Task<IActionResult> UpdateProperties(int id, [FromBody] PropertiesDto body)
        {
            var user = await UserManager.GetUserAsync(HttpContext.User);
            var solution = Repository.GetSolutionByDomainName(Request.Host.Host);
            var theSolution = await solution.SingleOrDefaultAsync();
            if (theSolution == null || !await Repository.IsUserOnSolution(theSolution, user))
            {
                return Forbid();
            }

            var page = await Repository.Pages(solution)
                .Where(page => page.Id == id).FirstOrDefaultAsync();
            if (page != null)
            {
                page.Title = body.Title;
                await _context.SaveChangesAsync();
                return Ok();
            }
            return NotFound();
        }

        [Authorize]
        [Route("{id}/Delete")]
        [HttpPost]
        public async Task<IActionResult> DeletePage(int id)
        {
            var user = await UserManager.GetUserAsync(HttpContext.User);
            var solution = Repository.GetSolutionByDomainName(Request.Host.Host);
            var theSolution = await solution.SingleOrDefaultAsync();
            if (theSolution == null || !await Repository.IsUserOnSolution(theSolution, user))
            {
                return Forbid();
            }

            var page = await Repository.Pages(solution)
                .Where(page => page.Id == id).FirstOrDefaultAsync();
            if (page != null)
            {
                page.IsDeleted = true;
                page.DeletedDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return Ok();
            }
            return NotFound();
        }
    }
}
