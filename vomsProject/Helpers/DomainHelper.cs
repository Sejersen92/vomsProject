using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vomsProject.Controllers;
using vomsProject.Data;

namespace vomsProject.Helpers
{
    public class DomainHelper
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DatabaseHelper _databaseHelper;
        private readonly ApplicationDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public DomainHelper(ILogger<HomeController> logger, DatabaseHelper databaseHelper, ApplicationDbContext dbContext, IConfiguration configuration)
        {
            _logger = logger;
            _databaseHelper = databaseHelper;
            _dbContext = dbContext;
            _configuration = configuration;
        }

        /// <summary>
        /// Get a Link to a page.
        /// </summary>
        /// <param name="page">The page to get a link to. The Solution property has to be included.</param>
        /// <returns>Returns a url</returns>
        public string GetDestinationUrl(Page page)
        {
            return $"{GetIndexPageUrl(page.Solution)}{page.PageName}";
        }

        /// <summary>
        /// Get a Link to the index page of the solution.
        /// </summary>
        /// <param name="solution">The solution to get a link to.</param>
        /// <returns>Returns a url, with a slash at the end</returns>
        public string GetIndexPageUrl(Solution solution)
        {
            if (solution != null)
            {
                var port = _configuration.GetValue<int?>("Port");
                var domain = solution.Domain != null ? solution.Domain : $"{solution.Subdomain}.{_configuration.GetValue<string>("RootDomain")}";
                return $"https://{domain}{(port.HasValue ? ":" + port.Value : "")}/";
            }
            else
            {
                throw new ArgumentException("Solution cannot be null");
            }
        }
    }
}
