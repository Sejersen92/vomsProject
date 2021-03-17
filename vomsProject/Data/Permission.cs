using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace vomsProject.Data
{
    public class Permission
    {
        public int Id { get; set; }
        public PermissionLevel PermissionLevel { get; set; }
        public Solution Solution { get; set; }
        public User User { get; set; }
    }
}
