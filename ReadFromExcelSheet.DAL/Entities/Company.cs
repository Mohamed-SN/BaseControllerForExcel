using ReadFromExcelSheet.DAL.Extends;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadFromExcelSheet.DAL.Entities
{
    public class Company : BaseEntity<short>
    {
        [StringLength(50, MinimumLength = 4)]
        public string Name { get; set; } = null!;

        [StringLength(50, MinimumLength = 4)]
        public string NameAr { get; set; } = null!;
        
        [EmailAddress]
        [StringLength(50)]
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string Logo { get; set; }


        public virtual ICollection<Branch> Branches { get; set; } = new HashSet<Branch>();

    }
}
