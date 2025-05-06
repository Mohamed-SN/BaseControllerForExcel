using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using ReadFromExcelSheet.BLL.Helper;
using ReadFromExcelSheet.BLL.Interface;
using ReadFromExcelSheet.DAL.Entities;
using ReadFromExcelSheet.DTO;
using ReadFromExcelSheet.Resources;

namespace ReadFromExcelSheet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesController : BaseController<Company, EntitySC, short, ReturnCompanyDto, ReturnCompanyDto, CompanyDto, EditCompanyDto>
    {
        private readonly IFileService _fileService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStringLocalizer<SharedResources> _localizer;

        public CompaniesController(IFileService fileService, IMapper mapper, IUnitOfWork unitOfWork, IStringLocalizer<SharedResources> localizer) : base(unitOfWork, mapper, fileService, localizer)
        {
            _fileService = fileService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _localizer = localizer;
        }
    }
}
