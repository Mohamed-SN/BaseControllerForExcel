using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using ReadFromExcelSheet.BLL.Interface;
using ReadFromExcelSheet.DAL.Entities;
using ReadFromExcelSheet.DTO;
using ReadFromExcelSheet.Utilites;
using ReadFromExcelSheet.Utiltes;
using System.ComponentModel.DataAnnotations;
using System.Text;


[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly IFileService _fileService;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public StudentsController(IFileService fileService, IMapper mapper, IUnitOfWork unitOfWork)
    {
        _fileService = fileService;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    //[HttpPost("upload")]
    //public async Task<IActionResult> UploadExcel(IFormFile file)
    //{
    //    if (file == null || file.Length == 0)
    //        return BadRequest("Invalid file.");

    //    List<string> bugs = new List<string>();
    //    var students = new List<StudentDto>();
    //    var studentToAdd = new List<Student>();

    //    // Save the uploaded file temporarily to disk to allow OpenXML to open it
    //    var tempFilePath = Path.GetTempFileName();
    //    await using (var fs = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write))
    //    {
    //        await file.CopyToAsync(fs);
    //    }

    //    // Extract images in order using OpenXML
    //    var images = ExcelImageExtractor.ExtractImagesByOrder(tempFilePath);

    //    using (var stream = new MemoryStream(System.IO.File.ReadAllBytes(tempFilePath)))
    //    using (var package = new ExcelPackage(stream))
    //    {
    //        var worksheet = package.Workbook.Worksheets[0];
    //        var rowCount = worksheet.Dimension.Rows;

    //        for (int row = 2; row <= rowCount; row++)
    //        {
    //            var student = Utilites.MapRowToDto<StudentDto>(worksheet, row, images, _fileService);


    //            var context = new ValidationContext(student);
    //            var validationResults = new List<ValidationResult>();
    //            Validator.TryValidateObject(student, context, validationResults, true);

    //            if (validationResults.Any())
    //            {
    //                var bug = new StringBuilder($"row[{row}]");
    //                foreach (var error in validationResults.Select(v => v.ErrorMessage))
    //                    bug.Append(", " + error);
    //                bugs.Add(bug.ToString());
    //            }

    //            students.Add(student);

    //             studentToAdd = _mapper.Map<List<Student>>(students);

    //            var result = await _unitOfWork.Students.SaveRange(studentToAdd);
    //            await _unitOfWork.CompleteAsync();
    //            if (result == null)
    //            {
    //                bugs.Add($"Failed to save student at row {row}");
    //            }

    //        }
    //    }

    //    System.IO.File.Delete(tempFilePath); // Clean up temp file

    //    if (bugs.Any())
    //        return Ok(bugs);

    //    //return Ok(new { students });
    //    return Ok(studentToAdd);
    //}


    [HttpPost("upload")]
    public async Task<IActionResult> UploadExcel(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Invalid file.");

        var bugs = new List<string>();
        var studentsDto = new List<StudentDto>();

        // Save uploaded Excel file temporarily
        var tempFilePath = Path.GetTempFileName();
        await using (var fs = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write))
        {
            await file.CopyToAsync(fs);
        }

        // Extract images from Excel using OpenXML
        var images = ExcelImageExtractor.ExtractImagesByOrder(tempFilePath);

        using (var stream = new MemoryStream(System.IO.File.ReadAllBytes(tempFilePath)))
        using (var package = new ExcelPackage(stream))
        {
            var worksheet = package.Workbook.Worksheets[0];
            var rowCount = worksheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++)
            {
                var studentDto = Utilites.MapRowToDto<StudentDto>(worksheet, row, images, _fileService);

                // Validate student data
                var context = new ValidationContext(studentDto);
                var validationResults = new List<ValidationResult>();
                Validator.TryValidateObject(studentDto, context, validationResults, true);

                if (validationResults.Any())
                {
                    var bug = new StringBuilder($"row[{row}]");
                    foreach (var error in validationResults.Select(v => v.ErrorMessage))
                        bug.Append(", " + error);
                    bugs.Add(bug.ToString());
                    continue;
                }

                studentsDto.Add(studentDto);
            }
        }

        System.IO.File.Delete(tempFilePath); // Cleanup temp file

        if (studentsDto.Count == 0)
            return BadRequest("No valid data found to import.");

        // Convert DTOs to Student entities and save images to disk
        var studentsToAdd = new List<Student>();

        foreach (var dto in studentsDto)
        {
            string fileName = null;

            if (dto.ProfilePicture != null && dto.ProfilePicture.Length > 0)
            {
                fileName = $"{Guid.NewGuid()}.jpg";
                var imagePath = Path.Combine("wwwroot", "Students", fileName);
                Directory.CreateDirectory(Path.GetDirectoryName(imagePath));
                await System.IO.File.WriteAllBytesAsync(imagePath, dto.ProfilePicture);
            }

            studentsToAdd.Add(new Student
            {
                Name = dto.Name,
                Age = dto.Age,
                Email = dto.Email,
                ProfilePicture = fileName 
            });
        }

        var result = await _unitOfWork.Students.SaveRange(studentsToAdd);
        await _unitOfWork.CompleteAsync();

        if (result == null)
            return StatusCode(500, "Failed to save students.");

        if (bugs.Any())
            return Ok(new { Message = "Imported with warnings", Errors = bugs });

        return Ok(new { Message = "All students imported successfully", Students = studentsToAdd });
    }



    [HttpGet("template")]
    public IActionResult GetStudentTemplate()
    {

        using (var package = new ExcelPackage())
        {
            var worksheet = package.Workbook.Worksheets.Add("Students");

            worksheet.Cells[1, 1].Value = "Name";
            worksheet.Cells[1, 2].Value = "Age";
            worksheet.Cells[1, 3].Value = "Email";
            worksheet.Cells[1, 4].Value = "ProfilePicture";

            //worksheet.Cells[2, 1].Value = "Jane Doe";
            //worksheet.Cells[2, 2].Value = 22;
            //worksheet.Cells[2, 3].Value = "jane.doe@example.com";
            //worksheet.Cells[2, 4].Value = "ImageHere";

            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            var excelBytes = package.GetAsByteArray();
            var fileName = $"StudentTemplate_{DateTime.Now:yyyyMMddHHmmss}.xlsx";

            return File(excelBytes,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        fileName);
        }
    }

}