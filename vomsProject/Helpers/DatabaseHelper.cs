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
                return dbContext.Solutions.Include(x => x.Users).ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine("(GetSolutionsByUser) caused an error: " + e);
                return new List<Solution>();
            }
        }
        public static IEnumerable<Solution> GetSolutionsByUser(string userId, ApplicationDbContext dbContext)
        {
            try
            {
                var solutions = dbContext.Solutions.Include(x => x.Users).Where(x => x.Users.FirstOrDefault().Id == userId).ToList();
                return solutions.Any() ? solutions : null;
            }
            catch (Exception e)
            {
                Console.WriteLine("(GetSolutionsByUser) caused an error: " + e);
                return null;
            }

        }
        public Solution GetSolutionById(int id, ApplicationDbContext dbContext)
        {
            try
            {
                var solution = dbContext.Solutions.Include(x => x.Users).Include(x => x.Pages).FirstOrDefault(x => x.Id == id);
                return solution;
            }
            catch (Exception e)
            {
                Console.WriteLine("(GetSolutionById) caused an error: " + e);
                return null;
            }

        }
    }
}
