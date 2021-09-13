using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vomsProject.Data;

namespace vomsProject.Helpers
{
    /// <summary>
    /// A central collection of all voms api actions.
    /// </summary>
    public class OperationsService
    {
        public class ForbittenException : Exception
        {

        }
        public class ConflictException : Exception
        {

        }
        private readonly ApplicationDbContext _context;
        private readonly RepositoryService Repository;
        private readonly UserManager<User> UserManager;
        public OperationsService(ApplicationDbContext context, RepositoryService repository, UserManager<User> userManager)
        {
            _context = context;
            Repository = repository;
            UserManager = userManager;
        }

        /// <summary>
        /// Save a new version and set it as the latest version.
        /// </summary>
        /// <param name="user">The user who is creating the page</param>
        /// <param name="solution">The solution the page should belong to</param>
        /// <param name="id">The id of the page</param>
        /// <param name="content">The content of the new version</param>
        /// <param name="isPublished">Whether to publish the page</param>
        /// <returns>The newly created PageContent. If the Page could not be found null will be returned.</returns>
        /// <exception cref="ForbittenException"/>
        public async Task<PageContent> UpdatePageContent(User user, IQueryable<Solution> solution, int id, string content, bool isPublished = false)
        {
            var theSolution = await solution.SingleOrDefaultAsync();
            if (theSolution == null || !await Repository.IsUserOnSolution(theSolution, user))
            {
                throw new ForbittenException();
            }

            var page = await Repository.Pages(solution)
                .Include(page => page.Versions)
                .FirstOrDefaultAsync(page => page.Id == id);

            if (page == null)
            {
                return null;
            }

            var blockContent = JsonConvert.DeserializeObject<IEnumerable<Editor.TransferBlock>>(content);
            var mainContent = blockContent.FirstOrDefault(block => block.tagType == "main");
            var pageContent = new PageContent()
            {
                Content = JsonConvert.SerializeObject(mainContent.blocks),
                SavedBy = user,
                SaveDate = DateTime.UtcNow,
                Page = page
            };
            page.Versions.Add(pageContent);
            page.LastSavedVersion = pageContent;

            if (isPublished)
            {
                page.PublishedVersion = pageContent;
                page.HtmlRenderContent = mainContent.ToHtml();
                page.IsPublished = true;
            }

            await _context.SaveChangesAsync();

            return pageContent;
        }

        /// <summary>
        /// Publishes a page. Same UpdatePageContent but will always publish the page.
        /// </summary>
        /// <param name="user">The user who is creating the page</param>
        /// <param name="solution">The solution the page should belong to</param>
        /// <param name="id">The id of the page</param>
        /// <param name="content">The content of the new version</param>
        /// <returns>The newly created PageContent. If the Page could not be found null will be returned.</returns>
        /// <exception cref="ForbittenException"/>
        public async Task<PageContent> PublishPage(User user, IQueryable<Solution> solution, int id, string content)
        {
            return await UpdatePageContent(user, solution, id, content, true);
        }

        /// <summary>
        /// Set the publish status as false.
        /// </summary>
        /// <param name="user">The user who is creating the page</param>
        /// <param name="solution">The solution the page should belong to</param>
        /// <param name="id">The id of the page</param>
        /// <returns>The unpublished Page. If the Page could not be found null will be returned.</returns>
        /// <exception cref="ForbittenException"/>
        public async Task<Page> UnpublishPage(User user, IQueryable<Solution> solution, int id)
        {
            var theSolution = await solution.SingleOrDefaultAsync();
            if (theSolution == null || !await Repository.IsUserOnSolution(theSolution, user))
            {
                throw new ForbittenException();
            }

            var page = await Repository.Pages(solution)
                .FirstOrDefaultAsync(page => page.Id == id);
            if (page == null)
            {
                return null;
            }

            page.PublishedVersion = null;
            page.HtmlRenderContent = "";
            page.IsPublished = false;

            await _context.SaveChangesAsync();

            return page;
        }

        /// <summary>
        /// Update the layout belonging the a page.
        /// </summary>
        /// <param name="user">The user who is updateing the layout</param>
        /// <param name="solution">The solution the layout belong to</param>
        /// <param name="pageId">The id of the page with the layout to update<param>
        /// <param name="content">The updated content of the layout</param>
        /// <returns>The updated Layout. If the Layout could not be found null will be returned.</returns>
        /// <exception cref="ForbittenException"/>
        public async Task<Layout> UpdatePageLayout(User user, IQueryable<Solution> solution, int pageId, string content)
        {
            var theSolution = await solution.SingleOrDefaultAsync();
            if (theSolution == null || !await Repository.IsUserOnSolution(theSolution, user))
            {
                throw new ForbittenException();
            }

            var layout = await Repository.Pages(solution)
                    .Where(page => page.Id == pageId)
                    .Select(page => page.Layout)
                    .FirstOrDefaultAsync();

            if (layout == null)
            {
                return null;
            }

            var blockContent = JsonConvert.DeserializeObject<IEnumerable<Editor.TransferBlock>>(content);
            var header = blockContent.FirstOrDefault(block => block.tagType == "header");
            var footer = blockContent.FirstOrDefault(block => block.tagType == "footer");

            layout.HeaderContent = header.ToHtml();
            layout.HeaderEditableContent = JsonConvert.SerializeObject(header.blocks);

            layout.FooterContent = footer.ToHtml();
            layout.FooterEditableContent = JsonConvert.SerializeObject(footer.blocks);

            layout.SavedBy = user;
            layout.SaveDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return layout;
        }

        /// <summary>
        /// Get the editor content for a specified version.
        /// </summary>
        /// <param name="user">The user who is getting the content</param>
        /// <param name="solution">The solution the content belong to</param>
        /// <param name="pageId">The id of the page with the content</param>
        /// <param name="versionId">The id of the version</param>
        /// <returns>The page content. If the content could not be found null will be returned.</returns>
        /// <exception cref="ForbittenException"/>
        public async Task<PageContent> GetPageContentVersion(User user, IQueryable<Solution> solution, int pageId, int versionId)
        {
            var theSolution = await solution.SingleOrDefaultAsync();
            if (theSolution == null || !await Repository.IsUserOnSolution(theSolution, user))
            {
                throw new ForbittenException();
            }

            return await Repository.Pages(solution)
                .Where(page => page.Id == pageId)
                .SelectMany(page => page.Versions)
                .Where(version => version.Id == versionId)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Removes a page.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="solution"></param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public async Task RemovePage(User user, IQueryable<Solution> solution, int pageId)
        {
            var theSolution = await solution.SingleOrDefaultAsync();
            if (theSolution == null || !await Repository.IsUserOnSolution(theSolution, user))
            {
                throw new ForbittenException();
            }

            try
            {
                var page = await Repository.Pages(solution).FirstOrDefaultAsync(page => page.Id == pageId);
                if (page != null)
                {
                    page.IsDeleted = true;
                    page.DeletedDate = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Updates a solution.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="solution"></param>
        /// <param name="friendlyName"></param>
        /// <param name="domainName"></param>
        /// <param name="stylesheet"></param>
        /// <param name="favicon"></param>
        /// <returns></returns>
        public async Task UpdateSolution(User user, IQueryable<Solution> solution, string friendlyName, string domainName, int stylesheet
            , string faviconMimeType = null, byte[] favicon = null)
        {
            var theSolution = await solution.SingleOrDefaultAsync();

            if (theSolution == null || !await Repository.DoUserHavePermissionOnSolution(user, theSolution, PermissionLevel.Admin))
            {
                throw new ForbittenException();
            }

            theSolution.Domain = domainName;
            theSolution.FriendlyName = friendlyName;
            if (favicon != null && faviconMimeType != null)
            {
                theSolution.FaviconMimeType = faviconMimeType;
                theSolution.Favicon = favicon;
            }

            theSolution.StyleId = stylesheet;
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes a solution.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="solution"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteSolution(User user, IQueryable<Solution> solution, int id)
        {
            var theSolution = await solution.SingleOrDefaultAsync();
            if (theSolution == null || !await Repository.DoUserHavePermissionOnSolution(user, theSolution, PermissionLevel.Admin))
            {
                throw new ForbittenException();
            }

            try
            {
                _context.Solutions.Remove(theSolution);

                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Adds a user.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="solution"></param>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        public async Task AddUser(User user, IQueryable<Solution> solution, string userEmail)
        {
            var theSolution = await solution.SingleOrDefaultAsync();
            if (theSolution == null || !await Repository.DoUserHavePermissionOnSolution(user, theSolution, PermissionLevel.Admin))
            {
                throw new ForbittenException();
            }

            try
            {
                var addedUser = _context.Users.FirstOrDefault(x => x.Email == userEmail);

                _context.Permissions.Add(new Permission()
                {
                    PermissionLevel = PermissionLevel.Editor,
                    User = addedUser,
                    Solution = theSolution
                });

                await _context.SaveChangesAsync();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        
        /// <summary>
        /// Removes a user.
        /// </summary>
        /// <param name="user">The user who is doing the task.</param>
        /// <param name="solution"></param>
        /// <param name="removeUserId">The user Id on the user whom is being removed.</param>
        /// <returns></returns>
        public async Task RemoveUser(User user, IQueryable<Solution> solution, string removeUserId)
        {
            var theSolution = await solution.SingleOrDefaultAsync();
            if (theSolution == null || !await Repository.DoUserHavePermissionOnSolution(user, theSolution, PermissionLevel.Admin))
            {
                throw new ForbittenException();
            }

            try
            {
                _context.Permissions.RemoveRange(solution.SelectMany(solution => solution.Permissions).Where(perm => perm.User.Id == removeUserId));

                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
