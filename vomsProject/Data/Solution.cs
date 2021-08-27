using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace vomsProject.Data
{
    public class Solution
    {
        public int Id { get; set; }
        public string Subdomain { get; set; }
        public string Domain { get; set; }
        public ICollection<User> Users { get; set; }
        public ICollection<Page> Pages { get; set; }
        public ICollection<Permission> Permissions { get; set; }
        public ICollection<Image> Images { get; set; }

        [NotMapped]
        public string DestinationUrl { get; set; }
    }
}
