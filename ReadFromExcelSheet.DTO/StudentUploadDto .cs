using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadFromExcelSheet.DTO
{
    public class StudentUploadDto : BaseFileDto
    {
        [Required]
        public int BranchId { get; set; }
    }
}
