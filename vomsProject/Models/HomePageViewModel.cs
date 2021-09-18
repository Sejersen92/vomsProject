using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vomsProject.Data;

namespace vomsProject.Models
{
    public class HomePageViewModel
    {
        public IEnumerable<Solution> Solutions { get; set; }
        public User User { get; set; }
    }
}
