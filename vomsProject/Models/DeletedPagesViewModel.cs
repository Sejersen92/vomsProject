using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vomsProject.Data;

namespace vomsProject.Models
{
    public class DeletedPagesViewModel
    {
        public IEnumerable<Page> Pages { get; set; }
        public int? FaildToDeletePageId { get; set; }
    }
}
