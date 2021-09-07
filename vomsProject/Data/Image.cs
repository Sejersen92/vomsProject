using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace vomsProject.Data
{
    public class Image
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public Solution Solution { get; set; }
        public Page Page { get; set; }

        public string FriendlyName { get; set; }
    }
}
