using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReadFromExcelSheet.DAL.Entities;

namespace ReadFromExcelSheet.DAL.Extends
{
    public class BaseEntity<IdType> : IMustHaveCompany
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public IdType Id { get; set; } = default!;
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDelete { get; set; } = false;
        public virtual short? CompanyId { get; set; }
        public virtual Company? Company { get; set; }
    }
}
