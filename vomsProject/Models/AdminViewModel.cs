using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vomsProject.Data;

namespace vomsProject.Models
{
    public class AdminViewModel
    {
        public IEnumerable<Solution> Solutions { get; set; }
        public bool? HasReachedProductLimit { get; set; }
    }
}
