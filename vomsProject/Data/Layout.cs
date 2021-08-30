using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace vomsProject.Data
{
    public class Layout
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string HeaderEditableContent { get; set; }
        public string HeaderContent { get; set; }
        public string FooterEditableContent { get; set; }
        public string FooterContent { get; set; }
        public DateTime SaveDate { get; set; }
        public User SavedBy { get; set; }
    }
}
