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
    public class HtmlHelper
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DatabaseHelper _databaseHelper;
        private readonly ApplicationDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public HtmlHelper(ILogger<HomeController> logger, DatabaseHelper databaseHelper, ApplicationDbContext dbContext, IConfiguration configuration)
        {
            _logger = logger;
            _databaseHelper = databaseHelper;
            _dbContext = dbContext;
            _configuration = configuration;
        }

        public string GetDestinationUrl(Page page)
        {
            var solution = page.Solution;
            var destinationUrl = $"{solution.Domain}.{_configuration.GetValue<string>("RootDomain")}/{page.PageName}";

            return destinationUrl;
        }

        public string GetIndexPageUrl(Solution solution)
        {
            if (solution != null)
            {
                return $"{solution.Domain}.{_configuration.GetValue<string>("RootDomain")}/";
            }
            else
            {
                return null;
            }
        }
    }
}
