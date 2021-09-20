using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vomsProject.Data;
using vomsProject.Helpers;

namespace vomsProject.Models
{
    public class HomePageViewModel
    {
        public PaginatedList<Solution> Solutions { get; set; }
        public User User { get; set; }
    }
}
