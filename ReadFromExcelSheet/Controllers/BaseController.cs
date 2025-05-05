using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.OpenApi.Models;
using OfficeOpenXml;
using ReadFromExcelSheet.BLL.Helper;
using ReadFromExcelSheet.BLL.Implementation;
using ReadFromExcelSheet.BLL.Interface;
using ReadFromExcelSheet.DAL.Entities;
using ReadFromExcelSheet.DAL.Extends;
using ReadFromExcelSheet.DTO;
using ReadFromExcelSheet.Resources;
using ReadFromExcelSheet.Utilites;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;


namespace ReadFromExcelSheet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController<Entity, SC, IdType, ReturnDto, ReturnWithDetailsDto, AddDto, EditDto> : ControllerBase
       where SC : EntitySC
       where Entity : BaseEntity<IdType>
       where ReturnDto : class
       where ReturnWithDetailsDto : class
       where AddDto : class,new()
       where EditDto : BaseDto<IdType>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IFileService _fileService;
        private readonly IStringLocalizer<SharedResources> _localizer;

        public BaseController(IUnitOfWork unitOfWork, IMapper mapper, IFileService fileService , IStringLocalizer<SharedResources> localizer)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            _fileService = fileService;
            this._localizer = localizer;
        } 

        [HttpPost("upload")]
        public async Task<IActionResult> UploadExcel(IFormFile file)  // Add the `new()` constraint
        {
            if (file == null || file.Length == 0)
                return BadRequest("Invalid file.");

            var bugs = new List<string>();
            var dtoList = new List<object>();  // Generic DTO list

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
                    // Map data to DTO
                    var dto = Utiltes.Utilites.MapRowToDto<AddDto>(worksheet, row, images, _fileService);

                    // Validate DTO data
                    var context = new ValidationContext(dto);
                    var validationResults = new List<ValidationResult>();
                    Validator.TryValidateObject(dto, context, validationResults, true);

                    if (validationResults.Any())
                    {
                        var bug = new StringBuilder($"row[{row}]");
                        foreach (var error in validationResults.Select(v => v.ErrorMessage))
                            bug.Append(", " + error);
                        bugs.Add(bug.ToString());
                        continue;
                    }

                    dtoList.Add(dto);
                }
            }

            System.IO.File.Delete(tempFilePath); // Cleanup temp file

            if (dtoList.Count == 0)
                return BadRequest("No valid data found to import.");

            // Now handle the logic based on your DTO type (e.g., StudentDto in this case)
            if (typeof(AddDto) == typeof(StudentDto))
            {
                var studentsToAdd = new List<Student>();
                foreach (var dto in dtoList.Cast<StudentDto>())
                {
                    string fileName = null;
                    if (dto.ProfilePicture != null && dto.ProfilePicture.Length > 0)
                    {
                        fileName = await _fileService.SaveFileAsync(dto.ProfilePicture, ".jpg", "Students");
                        var studentToAdd = mapper.Map<Student>(dto);
                        studentToAdd.ProfilePicture = fileName;
                        studentsToAdd.Add(studentToAdd);
                    }
                }

                var result = await unitOfWork.Students.SaveRange(studentsToAdd);
                await unitOfWork.CompleteAsync();

                if (result == null)
                    return StatusCode(500, "Failed to save students.");

                if (bugs.Any())
                    return Ok(new { Message = "Imported with warnings", Errors = bugs });

                return Ok(new { Message = "All students imported successfully", Students = studentsToAdd });
            }
            else
            {
                // Handle other DTO types dynamically
                return BadRequest("Unsupported DTO type.");
            }
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

        internal dynamic GetRepository()
        {

            switch (typeof(Entity).Name)
            {

                //#region Lookup
                case nameof(Student):
                    return unitOfWork.Students;
                //case nameof(AttendanceType):
                //    return unitOfWork.AttendanceTypeBL;
                //case nameof(Branch):
                //    return unitOfWork.BranchBL;
                //case nameof(BusinessSize):
                //    return unitOfWork.BusinessSizeBL;
                //case nameof(BusinessType):
                //    return unitOfWork.BusinessTypeBL;
                //case nameof(CertificateType):
                //    return unitOfWork.CertificateTypeBL;
                //case nameof(City):
                //    return unitOfWork.CityBL;
                //case nameof(Country):
                //    return unitOfWork.CountryBL;
                //case nameof(Department):
                //    return unitOfWork.DepartmentBL;
                //case nameof(DocumentType):
                //    return unitOfWork.DocumentTypeBL;
                //case nameof(FileDescription):
                //    return unitOfWork.FileDescriptionBL;
                //case nameof(FileDescriptionType):
                //    return unitOfWork.FileDescriptionTypeBL;
                //case nameof(Language):
                //    return unitOfWork.LanguageBL;
                //case nameof(LifeStyleType):
                //    return unitOfWork.LifeStyleTypeBL;
                //case nameof(MedicalInsuranceCompany):
                //    return unitOfWork.MedicalInsuranceCompanyBL;

                //case nameof(OnBoarding):
                //    return unitOfWork.OnBoardingBL;

                //case nameof(PersonalType):
                //    return unitOfWork.PersonalTypeBL;

                //case nameof(PositionType):
                //    return unitOfWork.PositionTypeBL;
                //case nameof(Position):
                //    return unitOfWork.PositionBL;

                //case nameof(RefernceType):
                //    return unitOfWork.RefernceTypeBL;



                //case nameof(SkillsType):
                //    return unitOfWork.SkillsTypeBL;
                //case nameof(University):
                //    return unitOfWork.UniversityBL;
                //case nameof(Zone):
                //    return unitOfWork.ZoneBL;
                //#endregion


                //#region Employee
                //case nameof(Employee):
                //    return unitOfWork.EmployeeBL;
                //case nameof(EmployeeAttendance):
                //    return unitOfWork.EmployeeAttendanceBL;
                //case nameof(EmployeeCertificate):
                //    return unitOfWork.EmployeeCertificateBL;
                //case nameof(EmployeeEducational):
                //    return unitOfWork.EmployeeEducationalBL;
                //case nameof(EmployeeHealthInformation):
                //    return unitOfWork.EmployeeHealthInformationBL;
                //case nameof(EmployeeIdentityDocument):
                //    return unitOfWork.EmployeeIdentityDocumentBL;
                //case nameof(EmployeeLanguage):
                //    return unitOfWork.EmployeeLanguageBL;
                //case nameof(EmployeeSkill):
                //    return unitOfWork.EmployeeSkillBL;
                //case nameof(EmployeeWorkHistory):
                //    return unitOfWork.EmployeeWorkHistoryBL;
                //case nameof(EmployeeSalary):
                //    return unitOfWork.EmployeeSalaryBL;
                //case nameof(EmployeeAttendanceLocation):
                //    return unitOfWork.EmployeeAttendanceLocationBL;

                //case nameof(EmployeeLevel):
                //    return unitOfWork.EmployeeLevelBL;
                //case nameof(EmployeeLevelSetting):
                //    return unitOfWork.EmployeeLevelSettingBL;
                //case nameof(EmployeeBenefit):
                //    return unitOfWork.EmployeeBenefitBL;

                //#endregion

                //#region Request
                //case nameof(RequestAttachment):
                //    return unitOfWork.RequestAttachmentBL;
                //case nameof(Softic.DAL.Entities.Request):
                //    return unitOfWork.RequestBL;
                //case nameof(RequestStatus):
                //    return unitOfWork.RequestStatusBL;

                //case nameof(RequestType):
                //    return unitOfWork.RequestTypeBL;
                //case nameof(RequestTypeConfig):
                //    return unitOfWork.RequestTypeConfigBL;

                //case nameof(RequestApprovalMatrix):
                //    return unitOfWork.RequestApprovalMatrixBL;
                //#endregion

                //#region User

                //case nameof(UserAddress):
                //    return unitOfWork.UserAddressBL;
                //case nameof(UserAttachment):
                //    return unitOfWork.UserAttachmentBL;
                //case nameof(UserEmail):
                //    return unitOfWork.UserEmailBL;
                //case nameof(UserLifeStyle):
                //    return unitOfWork.UserLifeStyleBL;
                //case nameof(UserRefernce):
                //    return unitOfWork.UserRefernceBL;
                //case nameof(UserTelephone):
                //    return unitOfWork.UserTelephoneBL;
                //#endregion

                //#region Issue
                //case nameof(Issue):
                //    return unitOfWork.IssueBL;
                //case nameof(IssueAttachment):
                //    return unitOfWork.IssueAttachmentBL;
                //case nameof(IssueExcuter):
                //    return unitOfWork.IssueExcuterBL;
                //case nameof(IssueComment):
                //    return unitOfWork.IssueCommentBL;

                //#endregion

                //case nameof(Support):
                //    return unitOfWork.SupportBL;
                //case nameof(SupportAttachment):
                //    return unitOfWork.SupportAttachmentBL;


                //case nameof(NotificationLog):
                //    return unitOfWork.NotificationLogBL;

                //case nameof(Asset):
                //    return unitOfWork.AssetBL;

                //case nameof(RelatedAsset):
                //    return unitOfWork.RelatedAssetBL;

                //case nameof(AssetAttachment):
                //    return unitOfWork.AssetAttachmentBL;

                //case nameof(DAL.TaskStatus):
                //    return unitOfWork.TaskStatusBL;
                //case nameof(TaskPriority):
                //    return unitOfWork.TaskPriorityBL;
                //case nameof(Tasks):
                //    return unitOfWork.TaskBL;
                //case nameof(ArchivedTask):
                //    return unitOfWork.ArchivedTasksBL;
                //case nameof(ToDoItem):
                //    return unitOfWork.ToDoItemBL;
                //case nameof(TaskAssignment):
                //    return unitOfWork.TaskAssignmentBL;
                //case nameof(TaskAttachment):
                //    return unitOfWork.TaskAttachmentBL;
                //case nameof(Softic.DAL.Entities.Help):
                //    return unitOfWork.HelpBL;
                //case nameof(HelpAttachment):
                //    return unitOfWork.HelpAttachmentBL;

                //case nameof(CompanySubscription):
                //    return unitOfWork.CompanySubscriptionBL;

                default:
                    throw new ArgumentException($"Repository for type {typeof(Entity).Name} not found");
            }

        }
    }
}
