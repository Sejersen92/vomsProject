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
        public User User { get; set; }
        public byte[] Favicon { get; set; }
        public List<StylesheetCustomizations> StylesheetCustomizations { get; set; }
        public List<string> Fonts { get; set; }
    }
}
