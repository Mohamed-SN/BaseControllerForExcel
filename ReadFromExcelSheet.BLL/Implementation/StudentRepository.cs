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
    public class StudentRepository : BaseRepo<Student, EntitySC, int>, IStudentRepository
    {
        private readonly ApplicationDbContext _context;

        public StudentRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
