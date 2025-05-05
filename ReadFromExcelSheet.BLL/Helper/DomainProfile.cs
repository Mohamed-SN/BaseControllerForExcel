using AutoMapper;
using ReadFromExcelSheet.DAL.Entities;
using ReadFromExcelSheet.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadFromExcelSheet.BLL.Helper
{
    public class DomainProfile : Profile
    {
        public DomainProfile() 
        {
            CreateMap<StudentDto, Student>();
        }

    }
}
