using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadFromExcelSheet.BLL.Interface
{
    public interface IUnitOfWork : IDisposable
    {


        public IStudentRepository Students { get;  set; }



        Task<bool> CompleteAsync();
    }
}
