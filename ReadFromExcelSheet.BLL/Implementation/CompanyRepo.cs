using ReadFromExcelSheet.BLL.Helper;
using ReadFromExcelSheet.BLL.Interface;
using ReadFromExcelSheet.DAL.Database;
using ReadFromExcelSheet.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadFromExcelSheet.BLL.Implementation
{
    public class CompanyRepo : BaseRepo<Company, EntitySC, short>, ICompanyRepo
    {
        private readonly ApplicationDbContext _context;
        public CompanyRepo(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
