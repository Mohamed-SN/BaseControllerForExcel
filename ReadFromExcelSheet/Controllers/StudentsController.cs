using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using ReadFromExcelSheet.BLL.Helper;
using ReadFromExcelSheet.BLL.Interface;
using ReadFromExcelSheet.Controllers;
using ReadFromExcelSheet.DAL.Entities;
using ReadFromExcelSheet.DTO;
using ReadFromExcelSheet.Resources;
using ReadFromExcelSheet.Utilites;
using ReadFromExcelSheet.Utiltes;
using System.ComponentModel.DataAnnotations;
using System.Text;


[ApiController]
[Route("api/[controller]")]
public class StudentsController : BaseController<Student,EntitySC ,int,ReturnStudentDto, ReturnStudentDto, StudentDto,EditStudentDto>
{
    private readonly IFileService _fileService;
    private readonly IExcelImportService _excelImportService;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IStringLocalizer<SharedResources> _localizer;

    public StudentsController(IFileService fileService, IExcelImportService excelImportService , IMapper mapper, IUnitOfWork unitOfWork, IStringLocalizer<SharedResources> localizer) : base(unitOfWork, mapper, fileService , localizer)
    {
        _fileService = fileService;
        _excelImportService = excelImportService;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _localizer = localizer;
    }

    //[HttpPost("upload-Student")]
    //public async Task<IActionResult> UploadStudentExcel(IFormFile file, int branchId)
    //{
    //    try
    //    {
    //        var (entities, errors) = await _excelImportService
    //            .ProcessExcelImport<StudentDto, Student>(file, branchId, "BranchId");

    //        if (!entities.Any())
    //            return BadRequest("No valid data found to import.");

    //        dynamic repo = GetRepository();
    //        await repo.SaveRange(entities);
    //        await _unitOfWork.CompleteAsync();

    //        if (errors.Any())
    //            return Ok(new { Message = "Imported with warnings", Errors = errors });

    //        return Ok(new { Message = "All companies imported successfully", Data = entities });
    //    }
    //    catch (ArgumentException ex)
    //    {
    //        return BadRequest(ex.Message);
    //    }
    //    catch (Exception ex)
    //    {
    //        return StatusCode(500, $"An error occurred: {ex.Message}");
    //    }
    //}



    [HttpPost("upload-Student")]
    public async Task<IActionResult> UploadStudentExcel([FromForm] StudentFileDto dto)
    {
        try
        {
            if (dto == null || dto.File == null || dto.File.Length == 0)
                return BadRequest("Invalid file.");
            var file = dto.File;
            var branchId = dto.BranchId;
            var (entities, errors) = await _excelImportService
                .ProcessExcelImport<StudentDto, Student>(file, branchId);

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