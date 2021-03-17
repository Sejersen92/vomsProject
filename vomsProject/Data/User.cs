using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace vomsProject.Data
{
    public class User : IdentityUser
    {
        public ICollection<Solution> Solutions { get; set; }
    }
}
