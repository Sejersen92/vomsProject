using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
        /// <returns>The newly created PageContent. If the Page could not be found null will be returned.</returns>
        /// <exception cref="ForbittenException"/>
        public async Task<PageContent> UpdatePageContent(User user, IQueryable<Solution> solution, int id, string content, bool isPublished = false, string publishedHtml = null)
        {

            var theSolution = await solution.SingleOrDefaultAsync();
            if (solution == null || !await Repository.IsUserOnSolution(theSolution, user))
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

            var pageContent = new PageContent()
            {
                Content = content,
                SavedBy = user,
                SaveDate = DateTime.UtcNow,
                Page = page
            };
            page.Versions.Add(pageContent);
            page.LastSavedVersion = pageContent;

            if (isPublished)
            {
                page.PublishedVersion = pageContent;
                page.HtmlRenderContent = publishedHtml;
                page.IsPublished = true;
            }

            await _context.SaveChangesAsync();

            return pageContent;
        }

        public async Task<PageContent> PublishPage(User user, IQueryable<Solution> solution, int id, string content, string publishedHtml)
        {
            return await UpdatePageContent(user, solution, id, content, true, publishedHtml);
        }
    }
}
