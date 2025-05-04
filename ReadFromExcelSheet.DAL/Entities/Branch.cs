using ReadFromExcelSheet.DAL.Extends;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadFromExcelSheet.DAL.Entities
{
    public class Branch : BaseEntity<int>
    {

        [StringLength(60, MinimumLength = 2)]
        public string Name { get; set; } = null!;
        [StringLength(60, MinimumLength = 2)]
        public string NameAr { get; set; } = null!;
        public virtual ICollection<Employee> Employees { get; set; } = new HashSet<Employee>();




    }
}
