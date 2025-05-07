using AutoMapper;
using DocumentFormat.OpenXml.Vml.Office;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using OfficeOpenXml;
using ReadFromExcelSheet.BLL.Helper;
using ReadFromExcelSheet.BLL.Implementation;
using ReadFromExcelSheet.BLL.Interface;
using ReadFromExcelSheet.DAL.Entities;
using ReadFromExcelSheet.DTO;
using ReadFromExcelSheet.Resources;
using ReadFromExcelSheet.Utilites;
using ReadFromExcelSheet.Utiltes;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ReadFromExcelSheet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesController : BaseController<Company, EntitySC, short, ReturnCompanyDto, ReturnCompanyDto, CompanyDto, EditCompanyDto>
    {
        private readonly IFileService _fileService;
        private readonly IExcelImportService _excelImportService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStringLocalizer<SharedResources> _localizer;

        public CompaniesController(IFileService fileService, IExcelImportService excelImportService, IMapper mapper, IUnitOfWork unitOfWork, IStringLocalizer<SharedResources> localizer) : base(unitOfWork, mapper, fileService , localizer)
        {
            _fileService = fileService;
            _excelImportService = excelImportService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _localizer = localizer;
            
        }

        [HttpPost("upload-company")]
        public async Task<IActionResult> UploadCompanyExcel([FromForm] CompanyFileDto dto)
        {
            try
            {

                if (dto == null || dto.File == null || dto.File.Length == 0)
                    return BadRequest("Invalid file.");
                var file = dto.File;
                var branchId = dto.BranchId;

                var (entities, errors) = await _excelImportService
                    .ProcessExcelImport<CompanyDto, Company>(file, branchId);

                if (!entities.Any())
                    return BadRequest("No valid data found to import.");

                dynamic repo = GetRepository();
                await repo.SaveRange(entities);
                await _unitOfWork.CompleteAsync();

                if (errors.Any())
                    return Ok(new { Message = "Imported with warnings", Errors = errors });

                return Ok(new { Message = "All companies imported successfully", Data = entities });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

    }
}

    
    
