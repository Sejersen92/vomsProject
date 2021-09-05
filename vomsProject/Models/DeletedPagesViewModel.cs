using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vomsProject.Data;

namespace vomsProject.Models
{
    public enum RecoverFailureReason
    {
        None,
        IsNolongerInTrash,
        HasBeenReplaced
    }
    public class DeletedPagesViewModel
    {
        public Solution Solution { get; set; }
        public IEnumerable<Page> Pages { get; set; }
        public RecoverFailureReason RecoverFailureReason { get; set; }
        public int? FailedToRecoverPageId { get; set; }
    }
}
