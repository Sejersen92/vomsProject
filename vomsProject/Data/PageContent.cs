using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace vomsProject.Data
{
    public class PageContent
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime SaveDate { get; set; }
        public User SavedBy { get; set; }
        public Page Page { get; set; }
    }
}
