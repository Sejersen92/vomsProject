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
using static vomsProject.Helpers.OperationsService;

/*
 This controller is the api used to manage pages.
 */
namespace vomsProject.Controllers.Api
{
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

    /// <summary>
    /// Result of uploading a image
    /// </summary>
    public class ImageResult
    {
        public int Id { get; set; }
        public int PageId { get; set; }
    }
    [Route("api/[controller]")]
    [ApiController]
    public class PageController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly RepositoryService Repository;
        private readonly UserManager<User> UserManager;
        private readonly OperationsService Operations;
        public PageController(ApplicationDbContext context, RepositoryService repository, UserManager<User> userManager, OperationsService operations)
        {
            _context = context;
            Repository = repository;
            UserManager = userManager;
            Operations = operations;
        }

        /// <summary>
        /// Save a new version and set it as the latest version.
        /// </summary>
        /// <param name="id">The id of the page</param>
        /// <param name="body">The content from the editor</param>
        /// <returns></returns>
        [Authorize]
        [Route("{id}/update")]
        [HttpPost]
        public async Task<IActionResult> Update(int id, [FromBody] object body)
        {
            try
            {
                var user = await UserManager.GetUserAsync(HttpContext.User);
                var solution = Repository.GetSolutionByDomainName(Request.Host.Host);

                var content = await Operations.UpdatePageContent(user, solution, id, body.ToString(), false);

                if (content != null)
                {
                    return Ok(new SaveResult
                    {
                        LatestVersion = content.Id,
                        SaveDate = content.SaveDate.ToString("yyyy-MM-dd HH:mm")
                    });
                }
                return NotFound();
            }
            catch (ForbittenException)
            {
                return Forbid();
            }
        }

        /// <summary>
        /// Publish and Save a new version. Also set this version as the the latest version.
        /// </summary>
        /// <param name="id">The id of the page</param>
        /// <param name="body">The content from the editor</param>
        /// <returns></returns>
        [Authorize]
        [Route("{id}/publish")]
        [HttpPost]
        public async Task<IActionResult> Publish(int id, [FromBody] object body)
        {
            try
            {
                var user = await UserManager.GetUserAsync(HttpContext.User);
                var solution = Repository.GetSolutionByDomainName(Request.Host.Host);

                var content = await Operations.PublishPage(user, solution, id, body.ToString());

                if (content != null)
                {
                    return Ok(new SaveResult
                    {
                        LatestVersion = content.Id,
                        SaveDate = content.SaveDate.ToString("yyyy-MM-dd HH:mm")
                    });
                }
                return NotFound();
            }
            catch (ForbittenException)
            {
                return Forbid();
            }
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
            try
            {
                var user = await UserManager.GetUserAsync(HttpContext.User);
                var solution = Repository.GetSolutionByDomainName(Request.Host.Host);

                var page = await Operations.UnpublishPage(user, solution, id);

                if (page != null)
                {
                    return Ok();
                }
                return NotFound();
            }
            catch (ForbittenException)
            {
                return Forbid();
            }
        }

        /// <summary>
        /// Set the publish status as false.
        /// </summary>
        /// <param name="id">The id of the page with the layout to update</param>
        /// <param name="body">The content from the editor</param>
        /// <returns></returns>
        [Authorize]
        [Route("{id}/layout/update")]
        [HttpPost]
        public async Task<IActionResult> UpdatePageLayout(int id, [FromBody] object body)
        {
            try
            {
                var user = await UserManager.GetUserAsync(HttpContext.User);
                var solution = Repository.GetSolutionByDomainName(Request.Host.Host);
                
                var layout = await Operations.UpdatePageLayout(user, solution, id, body.ToString());
                
                if (layout != null)
                {
                    return Ok(new SaveResult
                    {
                        LatestVersion = layout.Id,
                        SaveDate = layout.SaveDate.ToString("yyyy-MM-dd HH:mm")
                    });
                }
                return NotFound();
            }
            catch (ForbittenException)
            {
                return Forbid();
            }
        }

        /// <summary>
        /// Get the editor content for a specified version.
        /// </summary>
        /// <param name="id">The id of the page</param>
        /// <param name="versionId">The id of the version</param>
        /// <returns></returns>
        [Authorize]
        [Route("{id}/Version/{versionId}")]
        [HttpGet]
        public async Task<IActionResult> GetVersion(int id, int versionId)
        {
            try
            {
                var user = await UserManager.GetUserAsync(HttpContext.User);
                var solution = Repository.GetSolutionByDomainName(Request.Host.Host);

                var content = await Operations.GetPageContentVersion(user, solution, id, versionId);

                if (content != null)
                {
                    return Ok(content);
                }
                return NotFound();
            }
            catch (ForbittenException)
            {
                return Forbid();
            }            
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

        [Authorize]
        [Route("{id}/Recover")]
        [HttpPost]
        public async Task<IActionResult> RecoverPage(int id)
        {
            var user = await UserManager.GetUserAsync(HttpContext.User);
            var solution = Repository.GetSolutionByDomainName(Request.Host.Host);
            var theSolution = await solution.SingleOrDefaultAsync();
            if (theSolution == null || !await Repository.IsUserOnSolution(theSolution, user))
            {
                return Forbid();
            }

            var page = await Repository.DeletedPages(solution)
                .Where(page => page.Id == id).FirstOrDefaultAsync();

            if (page == null)
            {
                return NotFound();
            }
            var replaceingPage = await Repository.PageQuery(solution, page.PageName).AnyAsync();
            if (replaceingPage)
            {
                return Conflict();
            }
            page.IsDeleted = false;
            await _context.SaveChangesAsync();
            return Ok();
        }

        /// <summary>
        /// Upload a list of images
        /// </summary>
        /// <param name="id">The id of the page the images belong to</param>
        /// <param name="files">The images to upload</param>
        /// <returns></returns>
        [Authorize]
        [Route("{id}/Upload")]
        [HttpPost]
        public async Task<IActionResult> Upload(int id, List<IFormFile> files)
        {
            try
            {
                var user = await UserManager.GetUserAsync(HttpContext.User);
                var solution = Repository.GetSolutionByDomainName(Request.Host.Host);
                var upload = files.Select(file => new UploadImage()
                {
                    ContentType = file.ContentType,
                    FileName = file.FileName,
                    Content = file.OpenReadStream()
                });
                var images = await Operations.UploadImages(user, solution, id, upload);
                if (images == null)
                {
                    return NotFound();
                }

                return Ok(images.Select((image) => new ImageResult() 
                {
                    Id = image.Id,
                    PageId = image.Page.Id
                }));
            }
            catch (ForbittenException)
            {
                return Forbid();
            }
        }
    }
}
