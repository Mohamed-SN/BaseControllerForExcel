using ReadFromExcelSheet.DAL.Extends;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadFromExcelSheet.DAL.Entities
{
    public class Employee : BaseEntity<int>
    {
        public string? ReferancePhoto { get; set; }

        public decimal? NetSalary { get; set; }
        public decimal? GrossSalary { get; set; }

        public int BranchId { get; set; }
        public virtual Branch Branch { get; set; }




    }
}
