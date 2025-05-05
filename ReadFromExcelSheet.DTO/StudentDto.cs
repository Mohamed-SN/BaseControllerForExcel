using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadFromExcelSheet.DTO
{
    public class StudentDto :BaseDto<int>
    {
        [Required]
        public string Name { get; set; }

        [Range(18, 60)]
        public int Age { get; set; }

        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [MinLength(1)]
        public byte[] ProfilePicture { get; set; }
    }
}
