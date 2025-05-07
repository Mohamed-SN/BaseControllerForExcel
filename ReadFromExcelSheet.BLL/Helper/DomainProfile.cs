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
            CreateMap<Student, ReturnStudentDto>()
    .ForMember(dest => dest.ProfilePicture,
        opt => opt.MapFrom(src => Convert.FromBase64String(src.ProfilePicture)));

            CreateMap<ReturnStudentDto, Student>()
                .ReverseMap();

            CreateMap<StudentDto, Student>()
                .ForMember(dest => dest.ProfilePicture,
                           opt => opt.MapFrom(src => Convert.ToBase64String(src.ProfilePicture)));



            CreateMap<Company, ReturnCompanyDto>()
    .ForMember(dest => dest.Logo,
        opt => opt.MapFrom(src => (src.Logo)));

            CreateMap<ReturnCompanyDto, Company>()
                .ReverseMap();

            CreateMap<CompanyDto, Company>()
                .ForMember(dest => dest.Logo,
                           opt => opt.MapFrom(src => Convert.ToBase64String(src.Logo)));




        }

    }
}
