using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadFromExcelSheet.DTO
{
    public class BaseDto<IdType>
    {
        public IdType Id { get; set; } = default!;
        public short? CompanyId { get; set; }
    }
}
