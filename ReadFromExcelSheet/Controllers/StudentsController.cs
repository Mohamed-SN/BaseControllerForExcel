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




   

}