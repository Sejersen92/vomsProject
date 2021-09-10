using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace vomsProject.SolutionPages
{
    public class EditablePageModel
    {
        public class Version
        {
            public int id;
            public string saveDate;
        }
        public int id;
        public string content;
        public string title;
        public string header;
        public string footer;
        public string isPublished;
        public string publishedVersion;
        public string publishedDate;
        public string savedVersion;
        public string savedDate;
        public IEnumerable<Version> versions;
        public string styleVariables { get; set; }
    }
}
