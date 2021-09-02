using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

/*
 A Solution is the basis for a website. It controls this domain and subdomain that can be used to get to the website.
 It also had a collection of the Pages one the website. Permissions is wich users have access to the Solution and how mutch access they have.
 To get the users of a Solution you have to use the Permissions collection.
 */
namespace vomsProject.Data
{
    public class Solution
    {
        public int Id { get; set; }
        public string Subdomain { get; set; }
        public string Domain { get; set; }
        public ICollection<Page> Pages { get; set; }
        public ICollection<Permission> Permissions { get; set; }
        public ICollection<Image> Images { get; set; }
        public ICollection<Layout> Layouts { get; set; }
        public int? StyleId { get; set; }
        public Style Style { get; set; }

        [NotMapped]
        public string DestinationUrl { get; set; }
    }
}
