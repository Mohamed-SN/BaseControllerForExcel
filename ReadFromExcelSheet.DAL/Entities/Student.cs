using ReadFromExcelSheet.DAL.Extends;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadFromExcelSheet.DAL.Entities
{
    public class Student : BaseEntity<int>
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }
        [Column(TypeName = "nvarchar(max)")]
        public string ProfilePicture { get; set; }
    }
}
