using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using OfficeOpenXml;
using ReadFromExcelSheet.BLL.Implementation;
using ReadFromExcelSheet.BLL.Interface;
using ReadFromExcelSheet.DAL.Entities;
using ReadFromExcelSheet.DAL.Extends;
using ReadFromExcelSheet.DTO;
using ReadFromExcelSheet.Utilites;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ReadFromExcelSheet.Utiltes
{
    public interface IExcelSheetService<UploadFile, AddDto, Entity, IdType>
        where UploadFile : BaseFileDto
        where AddDto : class, new()
        where Entity : BaseEntity<IdType>
    {
        Task<(List<AddDto> dtoList, List<string> bugs)> ProcessExcelFileAsync(
            UploadFile file,
            dynamic repo);
    }




    public class ExcelSheetService<UploadFile, AddDto, Entity, IdType> : IExcelSheetService<UploadFile, AddDto, Entity, IdType>
    where UploadFile : BaseFileDto
    where AddDto : class, new()
    where Entity : BaseEntity<IdType>
    {
        private readonly IFileService _fileService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public ExcelSheetService(IFileService fileService, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _fileService = fileService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<(List<AddDto> dtoList, List<string> bugs)> ProcessExcelFileAsync(
            UploadFile file,
            dynamic repo)
        {
            var bugs = new List<string>();
            var dtoList = new List<AddDto>();

            if (file.File == null || file.File.Length == 0)
            {
                bugs.Add("Invalid file.");
                return (dtoList, bugs);
            }

            var tempFilePath = Path.GetTempFileName();
            await using (var fs = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write))
            {
                await file.File.CopyToAsync(fs);
            }

            var images = ExcelImageExtractor.ExtractImagesByOrder(tempFilePath);

            using (var stream = new MemoryStream(File.ReadAllBytes(tempFilePath)))
            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets[0];
                var rowCount = worksheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++)
                {
                    try
                    {
                        var dto = Utiltes.Utilites.MapRowToDto<AddDto>(worksheet, row, images, _fileService);

                        if (file.File != null)
                        {
                            var foreignProps = file.GetType().GetProperties()
                                .Where(p => p.Name != "Id" && p.Name.EndsWith("Id"));

                            var entityProps = typeof(AddDto).GetProperties()
                                .Where(p => p.Name != "Id" && p.Name.EndsWith("Id"));

                            foreach (var foreignProp in foreignProps)
                            {
                                var value = foreignProp.GetValue(file);
                                if (value is int intValue)
                                {
                                    entityProps.FirstOrDefault(p => p.Name.ToLower() == foreignProp.Name.ToLower())
                                                ?.SetValue(dto, intValue);
                                }
                            }
                        }

                        var context = new ValidationContext(dto);
                        var validationResults = new List<ValidationResult>();
                        Validator.TryValidateObject(dto, context, validationResults, true);

                        if (validationResults.Any())
                        {
                            var bug = new StringBuilder($"row[{row}]: ");
                            foreach (var error in validationResults.Select(v => v.ErrorMessage))
                                bug.Append(", " + error);
                            bugs.Add(bug.ToString());
                            continue;
                        }
                        else
                        {
                            dtoList.Add(dto);
                        }
                    }
                    catch (Exception ex)
                    {
                        bugs.Add($"row[{row}] - Error: {ex.Message}");
                    }
                }
            }









            System.IO.File.Delete(tempFilePath);

            if (dtoList.Count == 0)
                bugs.Add($"No valid data found to import.");

            var entityToAdd = new List<Entity>();
            foreach (var dto in dtoList.Cast<AddDto>())
            {
                var entityItem = _mapper.Map<Entity>(dto);

                // Get the image properties from the DTO
                var imageProperties = dto.GetType().GetProperties()
                    .Where(p => p.PropertyType == typeof(byte[]))
                    .ToList();

                // Assign the saved file names to the entity
                foreach (var prop in imageProperties)
                {
                    var imageValue = (byte[])prop.GetValue(dto);
                    if (imageValue != null && imageValue.Length > 0)
                    {
                        var fileName = await _fileService.SaveFileAsync(
                            imageValue,
                            ".jpg",
                            $"{typeof(Entity).Name}"
                        );

                        // Get the corresponding property in the entity
                        var entityProp = entityItem.GetType().GetProperty(prop.Name);
                        if (entityProp != null)
                        {
                            entityProp.SetValue(entityItem, fileName);
                        }
                    }
                }

                entityToAdd.Add(entityItem);
            }

            var result = await repo.SaveRange(entityToAdd);
            await _unitOfWork.CompleteAsync();

            if (result == null )
               bugs.Add($"Failed to save {typeof(Entity).Name}.");

            if (bugs.Any())
                bugs.Add("Imported with warnings");

            return _mapper.Map<List<AddDto>>(result);









        }
    }







}
