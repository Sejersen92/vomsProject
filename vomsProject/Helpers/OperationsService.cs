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

        public async Task UpdateSolution(User user, IQueryable<Solution> solution, string friendlyName, string domainName, int stylesheet
            , byte[] favicon = null)
        {
            var theSolution = await solution.SingleOrDefaultAsync();

            if (theSolution == null || !await Repository.DoUserHavePermissionOnSolution(user, theSolution, PermissionLevel.Admin))
            {
                throw new ForbittenException();
            }

            theSolution.Domain = domainName;
            theSolution.FriendlyName = friendlyName;
            theSolution.Favicon = favicon;

            theSolution.StyleId = stylesheet;
            await _context.SaveChangesAsync();
        }

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
        /// Remove a user from a specific solution.
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
