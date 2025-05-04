using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using ReadFromExcelSheet.DTO;
using ReadFromExcelSheet.Utiltes;
using System.ComponentModel.DataAnnotations;
using System.Text;

[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    [HttpPost("upload")]
    public async Task<IActionResult> UploadExcel(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Invalid file.");
        List<string> bugs = new List<string>();

        var students = new List<StudentDto>();

        using (var stream = new MemoryStream())
        {
            await file.CopyToAsync(stream);
            //chchxlxs
            //if (file.ContentType == "")
            //{

            //}
            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets[0];
                var rowCount = worksheet.Dimension.Rows;
                var pictures = worksheet.Drawings.OfType<ExcelPicture>().ToList();

                for (int row = 2; row <= rowCount; row++)
                {

                    StudentDto student = Utilites.MapRowToDto<StudentDto>(worksheet, row, pictures);

                    ValidationContext context = new ValidationContext(student);
                    List<ValidationResult> validationResults = new List<ValidationResult>();
                    Validator.TryValidateObject(student, context, validationResults, true);
                    if (validationResults.Any())
                    {

                        StringBuilder bug = new StringBuilder($"row[{row}]");
                        foreach (var ress in validationResults.Select(a => a.ErrorMessage).ToList())
                            bug.Append(", " + ress);
                        bugs.Add(bug.ToString());
                    }


                    students.Add((student));
                }
            }
        }

        if (bugs.Any())
        {
            return Ok(bugs);
        }


        return Ok(students);
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