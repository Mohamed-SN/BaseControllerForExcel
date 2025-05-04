using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.OpenApi.Models;
using ReadFromExcelSheet.BLL.Helper;
using ReadFromExcelSheet.BLL.Interface;
using ReadFromExcelSheet.DAL.Extends;
using ReadFromExcelSheet.DTO;
using ReadFromExcelSheet.Resources;
using System.Diagnostics.Metrics;
using System.Net;
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
       where AddDto : BaseDto<IdType>
       where EditDto : BaseDto<IdType>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IStringLocalizer<SharedResources> _localizer;

        public BaseController(IUnitOfWork unitOfWork, IMapper mapper, IStringLocalizer<SharedResources> localizer)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this._localizer = localizer;
        }

        [HttpPost("Get")]

        public virtual async Task<IActionResult> Get(SC sc)
        {
            string? CompanyId = User.FindFirst("CompanyId")?.Value.ToString();
            sc.CompanyId = !string.IsNullOrEmpty(CompanyId) ? short.Parse(CompanyId) : null;

            //UserDto? userDto = await authService.GetCurrentUser(User.FindFirst(ClaimTypes.Email)?.Value ?? "");
            try
            {
                dynamic repo = GetRepository();
                var res = await repo.GetAll(sc);


                if (sc.WithDetails)
                {
                    var data = new Pagination<ReturnWithDetailsDto>
                    {
                        PageIndex = res.PageIndex,
                        PageSize = res.PageSize,
                        TotalRows = res.TotalRows,
                        TotalPages = res.TotalPages,
                        List = mapper.Map<List<ReturnWithDetailsDto>>(res.List)
                    };

                    return Ok(new ApiResponse<Pagination<ReturnWithDetailsDto>>(
                        HttpStatusCode.OK,
                        SharedResources.Success,
                        _localizer,
                        data
                    ));

                }
                else
                {

                    Pagination<ReturnDto> data = new Pagination<ReturnDto>()
                    {
                        PageIndex = res.PageIndex,
                        PageSize = res.PageSize,
                        TotalRows = res.TotalRows,
                        TotalPages = res.TotalPages,
                        List = mapper.Map<List<ReturnDto>>(res.List)
                    };
                    return Ok(new ApiResponse<Pagination<ReturnDto>>(HttpStatusCode.OK, SharedResources.Success,
                            _localizer, data));
                }
            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponse<Entity?>(HttpStatusCode.BadRequest, SharedResources.FailedToGetData,
                        _localizer, null, new[] { e.Message }));
            }
        }


        [HttpPost("Add")]
        public virtual async Task<IActionResult> Add(AddDto dto)
        {
            dynamic repo = GetRepository();
            try
            {
                Entity data = mapper.Map<Entity>(dto);
                var res = await repo.Add(data);
                await unitOfWork.CompleteAsync();

                return Ok(new ApiResponse<ReturnDto>(HttpStatusCode.OK, SharedResources.AddedSuccessfuly,
                        _localizer, mapper.Map<ReturnDto>(res)));
            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponse<Entity?>(HttpStatusCode.BadRequest, SharedResources.AddFailed,
                        _localizer, null, new[] { e.InnerException?.Message ?? e.Message }));

            }

        }

        [HttpPost("Edit")]
        public virtual async Task<IActionResult> Edit(EditDto dto)
        {
            dynamic repo = GetRepository();
            try
            {
                Entity? data = await repo.Get(dto.Id);
                if (data is null)
                    return NotFound(new ApiResponse<Entity?>(HttpStatusCode.NotFound, SharedResources.NotFound,
                        _localizer, null, new[] { $"Object with this id Not Found" }));

                data = mapper.Map<Entity>(dto);
                var res = await repo.Edit(data);
                await unitOfWork.CompleteAsync();
                return Ok(new ApiResponse<ReturnDto>(HttpStatusCode.OK, SharedResources.Updated,
                        _localizer, mapper.Map<ReturnDto>(res)));
            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponse<Entity?>(HttpStatusCode.BadRequest, SharedResources.UpdateFailed,
                        _localizer, null, new[] { e.Message }));

            }

        }
        [HttpPost("Delete/{id}/{CompanyId}")]
        public async Task<IActionResult> Delete(IdType id, short CompanyId)
        {
            dynamic repo = GetRepository();

            try
            {
                Entity? data = await repo.Delete(id);
                if (data is null)
                    return NotFound(new ApiResponse<Entity?>(HttpStatusCode.NotFound, SharedResources.NotFound,
                        _localizer, null, new[] { "Not Found" }));

                await unitOfWork.CompleteAsync();
                return Ok(new ApiResponse<ReturnDto>(HttpStatusCode.OK, SharedResources.Deleted,
                        _localizer, mapper.Map<ReturnDto>(data)));
            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponse<Entity?>(HttpStatusCode.BadRequest, SharedResources.DeletedFailed,
                        _localizer, null, new[] { e.Message }));

            }


        }

        [HttpPost("Activate/{id}/{CompanyId}")]
        public async Task<IActionResult> Activate(IdType id, short CompanyId)
        {
            dynamic repo = GetRepository();

            try
            {
                Entity? data = await repo.Activate(id);
                if (data is null)
                    return NotFound(new ApiResponse<Entity?>(HttpStatusCode.NotFound, SharedResources.NotFound,
                        _localizer, null, new[] { "Not Found" }));

                await unitOfWork.CompleteAsync();
                return Ok(new ApiResponse<ReturnDto>(HttpStatusCode.OK, SharedResources.ActivateSuccess,
                        _localizer, mapper.Map<ReturnDto>(data)));
            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponse<Entity?>(HttpStatusCode.BadRequest, SharedResources.ActivateFailed,
                        _localizer, null, new[] { e.Message }));

            }


        }

        [HttpPost("DeActivate/{id}/{CompanyId}")]
        public async Task<IActionResult> DeActivate(IdType id, short CompanyId)
        {
            dynamic repo = GetRepository();

            try
            {
                Entity? data = await repo.DeActivate(id);
                if (data is null)
                    return NotFound(new ApiResponse<Entity?>(HttpStatusCode.NotFound, SharedResources.NotFound,
                        _localizer, null, new[] { "Not Found" }));

                await unitOfWork.CompleteAsync();
                return Ok(new ApiResponse<ReturnDto>(HttpStatusCode.OK, SharedResources.DeActivate,
                        _localizer, mapper.Map<ReturnDto>(data)));
            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponse<Entity?>(HttpStatusCode.BadRequest, SharedResources.DeActivateFailed,
                        _localizer, null, new[] { e.Message }));

            }


        }

        [HttpPost("Count")]
        public virtual async Task<IActionResult> Count(SuperAdminFilter sc)
        {

            try
            {
                dynamic repo = GetRepository();
                int count = await repo.Count(sc);
                return Ok(new ApiResponse<int>(HttpStatusCode.OK, SharedResources.Success,
                        _localizer, count));
            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponse<int?>(HttpStatusCode.BadRequest, SharedResources.CountFailed,
                        _localizer, null, new[] { e.Message }));
            }
        }
        internal dynamic GetRepository()
        {

            switch (typeof(Entity).Name)
            {

                //#region Lookup
                //case nameof(AttachmentCategory):
                //    return unitOfWork.AttachmentCategoryBL;
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
