using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vomsProject.Data;

namespace vomsProject.Models
{
    public class PageOverview
    {
        public int SolutionId { get; set; }
        public Solution Solution { get; set; }
        public IEnumerable<Page> Pages { get; set; }
        public IEnumerable<Option> StyleSheets { get; set; }
        public int? SelectedStyleId { get; set; }
    }
}
