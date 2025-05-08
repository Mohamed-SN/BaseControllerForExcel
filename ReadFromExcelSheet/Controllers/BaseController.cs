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
using ReadFromExcelSheet.Utiltes;
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
    public class BaseController<Entity, SC, IdType, ReturnDto, ReturnWithDetailsDto, AddDto, EditDto, UploadFile> : ControllerBase
       where SC : EntitySC
       where Entity : BaseEntity<IdType>
       where ReturnDto : class
       where ReturnWithDetailsDto : class
       where AddDto : class, new()
       where EditDto : BaseDto<IdType>
        where UploadFile : BaseFileDto

    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IFileService _fileService;
        private readonly IStringLocalizer<SharedResources> _localizer;
        private readonly IExcelSheetService<UploadFile, AddDto, Entity, IdType> _excelSheetService;

        public BaseController(IUnitOfWork unitOfWork, IMapper mapper, IFileService fileService, IStringLocalizer<SharedResources> localizer, IExcelSheetService<UploadFile, AddDto, Entity, IdType> excelSheetService)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            _fileService = fileService;
            this._localizer = localizer;
            _excelSheetService = excelSheetService;
            //_excelSheetService = excelSheetService;
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


        //[HttpPost("upload")]
        //public async Task<IActionResult> UploadExcel([FromForm] UploadFile file)
        //{
        //    dynamic repo = GetRepository();

        //    if (file.File == null || file.File.Length == 0)
        //        return BadRequest("Invalid file.");

        //    var bugs = new List<string>();
        //    var dtoList = new List<AddDto>();

        //    // Save uploaded Excel file temporarily
        //    var tempFilePath = Path.GetTempFileName();
        //    await using (var fs = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write))
        //    {
        //        await file.File.CopyToAsync(fs);
        //    }

        //    // Extract images from Excel using OpenXML
        //    var images = ExcelImageExtractor.ExtractImagesByOrder(tempFilePath);

        //    using (var stream = new MemoryStream(System.IO.File.ReadAllBytes(tempFilePath)))
        //    using (var package = new ExcelPackage(stream))
        //    {
        //        var worksheet = package.Workbook.Worksheets[0];
        //        var rowCount = worksheet.Dimension.Rows;

        //        for (int row = 2; row <= rowCount; row++)
        //        {
        //            // Map data to DTO
        //            var dto = Utiltes.Utilites.MapRowToDto<AddDto>(worksheet, row, images, _fileService);
        //            if (file.File != null)
        //            {
        //                var foreignProps = file.GetType().GetProperties()
        //                    .Where(p => p.Name != "Id" && p.Name.EndsWith("Id"));

        //                var EntityProps = typeof(AddDto).GetProperties()
        //                    .Where(p => p.Name != "Id" && p.Name.EndsWith("Id"));

        //                foreach (var foreignProp in foreignProps)
        //                {
        //                    var value = foreignProp.GetValue(file);
        //                    if (value is int intValue)
        //                    {
        //                        EntityProps.FirstOrDefault(p => p.Name.ToLower() == foreignProp.Name.ToLower())?.SetValue(dto, intValue);
        //                    }


        //                }
        //            }



        //            var context = new ValidationContext(dto);
        //            var validationResults = new List<ValidationResult>();
        //            Validator.TryValidateObject(dto, context, validationResults, true);

        //            if (validationResults.Any())
        //            {
        //                var bug = new StringBuilder($"row[{row}]");
        //                foreach (var error in validationResults.Select(v => v.ErrorMessage))
        //                    bug.Append(", " + error);
        //                bugs.Add(bug.ToString());
        //                continue;
        //            }

        //            dtoList.Add(dto);
        //        }
        //    }

        //    System.IO.File.Delete(tempFilePath);

        //    if (dtoList.Count == 0)
        //        return BadRequest("No valid data found to import.");

        //    var entityToAdd = new List<Entity>();
        //    foreach (var dto in dtoList.Cast<AddDto>())
        //    {
        //        var entityItem = mapper.Map<Entity>(dto);

        //        // Get the image properties from the DTO
        //        var imageProperties = dto.GetType().GetProperties()
        //            .Where(p => p.PropertyType == typeof(byte[]))
        //            .ToList();

        //        // Assign the saved file names to the entity
        //        foreach (var prop in imageProperties)
        //        {
        //            var imageValue = (byte[])prop.GetValue(dto);
        //            if (imageValue != null && imageValue.Length > 0)
        //            {
        //                var fileName = await _fileService.SaveFileAsync(
        //                    imageValue,
        //                    ".jpg",
        //                    $"{typeof(Entity).Name}"
        //                );

        //                // Get the corresponding property in the entity
        //                var entityProp = entityItem.GetType().GetProperty(prop.Name);
        //                if (entityProp != null)
        //                {
        //                    entityProp.SetValue(entityItem, fileName);
        //                }
        //            }
        //        }

        //        entityToAdd.Add(entityItem);
        //    }

        //    var result = await repo.SaveRange(entityToAdd);
        //    await unitOfWork.CompleteAsync();

        //    if (result == null)
        //        return StatusCode(500, $"Failed to save {typeof(Entity).Name}.");

        //    if (bugs.Any())
        //        return Ok(new { Message = "Imported with warnings", Errors = bugs });

        //    return Ok(new { Message = $"All {typeof(Entity).Name} imported successfully", Entity = entityToAdd });
        //}
        [HttpPost("upload")]
        public async Task<IActionResult> UploadExcel([FromForm] UploadFile file)
        {

            dynamic repo = GetRepository();

            var result = await _excelSheetService.ProcessExcelFileAsync(file, repo);

            if (!result.success)
                return BadRequest(result.message);

            return Ok(result.message);
        }




        [HttpGet("template")]
        public IActionResult GetTemplate()  // Generic method
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add(typeof(Entity).Name); // Use the type name dynamically

                var properties = typeof(AddDto).GetProperties()
                .Where(a => !a.Name
                .ToLower()
                .Contains("id"))
                .ToList(); // Get properties of the generic type

                // Set the header row dynamically
                for (int i = 0; i < properties.Count(); i++)
                {
                    worksheet.Cells[1, i + 1].Value = properties[i].Name;  // Assign property names as column headers
                }

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                var excelBytes = package.GetAsByteArray();
                var fileName = $"{typeof(Entity).Name}Template_{DateTime.Now:yyyyMMddHHmmss}.xlsx";

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
                case nameof(Company):
                    return unitOfWork.Companies;
              
                default:
                    throw new ArgumentException($"Repository for type {typeof(Entity).Name} not found");
            }

        }
    }
}
