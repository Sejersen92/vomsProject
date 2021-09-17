using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace vomsProject.Data
{
    public class User : IdentityUser
    {
        public ICollection<Permission> Permissions { get; set; }

        public ProductType ProductVersion { get; set; }

        public string Nickname { get; set; }

        public bool DisableTutorials { get; set; }

    }
}
