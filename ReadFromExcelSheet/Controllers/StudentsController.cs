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
public class StudentsController : BaseController<Student,EntitySC ,int,ReturnStudentDto, ReturnStudentDto, StudentDto,EditStudentDto, StudentUploadDto>
{
    private readonly IFileService _fileService;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IStringLocalizer<SharedResources> _localizer;

    public StudentsController(IFileService fileService, IMapper mapper, IUnitOfWork unitOfWork, IStringLocalizer<SharedResources> localizer) : base(unitOfWork, mapper, fileService , localizer)
    {
        _fileService = fileService;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _localizer = localizer;
    }

    protected override async Task<List<Student>> ParseExcelToEntities(StudentUploadDto dto, string filePath)
    {
        var result = new List<Student>();

        using var package = new ExcelPackage(new FileInfo(filePath));
        var worksheet = package.Workbook.Worksheets.First();
        var rowCount = worksheet.Dimension.Rows;

        for (int row = 2; row <= rowCount; row++)
        {
            var name = worksheet.Cells[row, 1].Text;
            //int age =int.TryParse worksheet.Cells[row, 2].Text;
            var email = worksheet.Cells[row, 3].Text;
            var profilePic = worksheet.Cells[row, 4].ToString();
            if (string.IsNullOrWhiteSpace(name)) continue;

            result.Add(new Student
            {
                Name = name,
                BranchId = dto.BranchId,
                //Age = age,
                Email = email,
                ProfilePicture = profilePic

            });
        }

        return result;
    }
}




   

