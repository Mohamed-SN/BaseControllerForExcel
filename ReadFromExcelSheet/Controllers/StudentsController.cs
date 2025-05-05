using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using ReadFromExcelSheet.BLL.Interface;
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

    public StudentsController(IFileService fileService)
    {
        _fileService = fileService;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadExcel(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Invalid file.");

        List<string> bugs = new List<string>();
        var students = new List<StudentDto>();

        // Save the uploaded file temporarily to disk to allow OpenXML to open it
        var tempFilePath = Path.GetTempFileName();
        await using (var fs = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write))
        {
            await file.CopyToAsync(fs);
        }

        // Extract images in order using OpenXML
        var images = ExcelImageExtractor.ExtractImagesByOrder(tempFilePath);

        using (var stream = new MemoryStream(System.IO.File.ReadAllBytes(tempFilePath)))
        using (var package = new ExcelPackage(stream))
        {
            var worksheet = package.Workbook.Worksheets[0];
            var rowCount = worksheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++)
            {
                var student = Utilites.MapRowToDto<StudentDto>(worksheet, row, images, _fileService);


                var context = new ValidationContext(student);
                var validationResults = new List<ValidationResult>();
                Validator.TryValidateObject(student, context, validationResults, true);

                if (validationResults.Any())
                {
                    var bug = new StringBuilder($"row[{row}]");
                    foreach (var error in validationResults.Select(v => v.ErrorMessage))
                        bug.Append(", " + error);
                    bugs.Add(bug.ToString());
                }

                students.Add(student);
            }
        }

        System.IO.File.Delete(tempFilePath); // Clean up temp file

        if (bugs.Any())
            return Ok(bugs);

        return Ok(new { students });
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