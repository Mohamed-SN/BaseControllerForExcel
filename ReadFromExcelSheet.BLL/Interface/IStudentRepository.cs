using ReadFromExcelSheet.BLL.Helper;
using ReadFromExcelSheet.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadFromExcelSheet.BLL.Interface
{
    public interface IStudentRepository : IBaseRepo<Student , EntitySC, int>
    {
       
    }
    
}
