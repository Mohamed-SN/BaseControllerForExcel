using ReadFromExcelSheet.BLL.Interface;
using ReadFromExcelSheet.DAL.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;

namespace ReadFromExcelSheet.BLL.Implementation
{
    public class UnitOfWork : IUnitOfWork
    {

        private readonly ApplicationDbContext _context;
        public IStudentRepository Students { get; set; }

        public UnitOfWork(ApplicationDbContext context, IFileService fileService)
        {
            _context = context;
            Students = new StudentRepository(_context);
            

        }

        public async Task<bool> CompleteAsync()
        {
            try
            {
                return await _context.SaveChangesAsync() > 0 ? true : false;

            }
            catch (Exception e)
            {

                throw new Exception(e.InnerException?.Message ?? e.Message);
            }

        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
