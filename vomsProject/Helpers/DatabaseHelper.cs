using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using vomsProject.Data;

namespace vomsProject.Helpers
{
    public class DatabaseHelper
    {
        public IEnumerable<Solution> GetSolutions(ApplicationDbContext dbContext)
        {
            try
            {
                return dbContext.Solutions.Include(x => x.Permissions).ThenInclude(x => x.User).ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine("(GetSolutionsByUser) caused an error: " + e);
                return new List<Solution>();
            }
        }
        public static IEnumerable<Solution> GetSolutionsByUser(string userId, ApplicationDbContext dbContext)
        {
            return dbContext.Solutions.Include(x => x.Permissions).ThenInclude(perm => perm.User).Where(x => x.Permissions.Any(perm => perm.User.Id == userId)).ToList();
        }
        public Solution GetSolutionById(int id, ApplicationDbContext dbContext)
        {
             return dbContext.Solutions.Include(x => x.Permissions).ThenInclude(perm => perm.User).Include(x => x.Pages).FirstOrDefault(x => x.Id == id);
        }
    }
}
