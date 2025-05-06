using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadFromExcelSheet.DTO
{
    public class CompanyDto : BaseDto<short>
    {

        [StringLength(50, MinimumLength = 4)]
        public string Name { get; set; } = null!;

        [StringLength(50, MinimumLength = 4)]
        public string NameAr { get; set; } = null!;

        [EmailAddress]
        [StringLength(50)]
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public byte[] Logo { get; set; }
    }
}
