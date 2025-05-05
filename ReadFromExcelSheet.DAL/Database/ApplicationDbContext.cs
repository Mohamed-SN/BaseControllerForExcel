using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ReadFromExcelSheet.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ReadFromExcelSheet.DAL.Database
{
    public class ApplicationDbContext :DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
        {
        
        
         
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {


            modelBuilder.Entity<Branch>()
           .HasMany(c => c.Employees)
           .WithOne(e => e.Branch)
           .HasForeignKey(e => e.BranchId)
           .IsRequired(true)
           .OnDelete(DeleteBehavior.Restrict);
        }

        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<Branch> Branches { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<Student> Students { get; set; }




    }
}
