using AutoMapper;
using OfficeOpenXml;
using ReadFromExcelSheet.BLL.Interface;
using ReadFromExcelSheet.DTO;
using ReadFromExcelSheet.Utilites;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;

namespace ReadFromExcelSheet.Utiltes
{
    //public class ExcelImportService : IExcelImportService
    //{
    //    private readonly IFileService _fileService;
    //    private readonly IMapper _mapper;

    //    public ExcelImportService(IFileService fileService, IMapper mapper)
    //    {
    //        _fileService = fileService;
    //        _mapper = mapper;
    //    }

    //    //    public async Task<(List<TEntity> Entities, List<string> Errors)> ProcessExcelImport<TDto, TEntity>(
    //    //IFormFile file,
    //    //int foreignKeyValue,
    //    //string foreignKeyName)
    //    //where TDto : class, new()
    //    //where TEntity : class, new()
    //    //    {
    //    //        if (file == null || file.Length == 0)
    //    //            throw new ArgumentException("Invalid file.");

    //    //        var bugs = new List<string>();
    //    //        var dtoList = new List<TDto>();

    //    //        var tempFilePath = Path.GetTempFileName();
    //    //        await using (var fs = new FileStream(tempFilePath, FileMode.Create))
    //    //        {
    //    //            await file.CopyToAsync(fs);
    //    //        }

    //    //        var images = ExcelImageExtractor.ExtractImagesByOrder(tempFilePath);

    //    //        using var stream = new MemoryStream(System.IO.File.ReadAllBytes(tempFilePath));
    //    //        using var package = new ExcelPackage(stream);
    //    //        var worksheet = package.Workbook.Worksheets[0];

    //    //        for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
    //    //        {
    //    //            var dto = Utilites.MapRowToDto<TDto>(worksheet, row, images, _fileService);

    //    //            var foreignProp = typeof(TDto).GetProperty(foreignKeyName);
    //    //            if (foreignProp != null && foreignProp.PropertyType == typeof(int))
    //    //                foreignProp.SetValue(dto, foreignKeyValue);

    //    //            var context = new ValidationContext(dto);
    //    //            var validationResults = new List<ValidationResult>();
    //    //            Validator.TryValidateObject(dto, context, validationResults, true);

    //    //            if (validationResults.Any())
    //    //            {
    //    //                var bug = new StringBuilder($"row[{row}]");
    //    //                foreach (var error in validationResults)
    //    //                    bug.Append(", " + error.ErrorMessage);
    //    //                bugs.Add(bug.ToString());
    //    //                continue;
    //    //            }

    //    //            dtoList.Add(dto);
    //    //        }

    //    //        System.IO.File.Delete(tempFilePath);

    //    //        var entities = new List<TEntity>();

    //    //        foreach (var dto in dtoList)
    //    //        {
    //    //            var entity = _mapper.Map<TEntity>(dto);

    //    //            var imageProps = typeof(TDto).GetProperties().Where(p => p.PropertyType == typeof(byte[]));
    //    //            foreach (var prop in imageProps)
    //    //            {
    //    //                var imageData = (byte[])prop.GetValue(dto);
    //    //                if (imageData != null && imageData.Length > 0)
    //    //                {
    //    //                    var fileName = await _fileService.SaveFileAsync(imageData, ".jpg", typeof(TEntity).Name);
    //    //                    var entityProp = typeof(TEntity).GetProperty(prop.Name);
    //    //                    if (entityProp != null && entityProp.PropertyType == typeof(string))
    //    //                        entityProp.SetValue(entity, fileName);
    //    //                }
    //    //            }

    //    //            var entityFkProp = typeof(TEntity).GetProperty(foreignKeyName);
    //    //            if (entityFkProp != null && entityFkProp.PropertyType == typeof(int))
    //    //                entityFkProp.SetValue(entity, foreignKeyValue);

    //    //            entities.Add(entity);
    //    //        }

    //    //        return (entities, bugs);
    //    //    }



    ////    public async Task<(List<TEntity> Entities, List<string> Errors)> ProcessExcelImport<TDto, TEntity>(
    ////IFormFile file,
    ////int foreignKeyValue)
    ////where TDto : class, new()
    ////where TEntity : class, new()
    ////    {
    ////        if (file == null || file.Length == 0)
    ////            throw new ArgumentException("Invalid file.");

    ////        var bugs = new List<string>();
    ////        var dtoList = new List<TDto>();

    ////        var tempFilePath = Path.GetTempFileName();
    ////        await using (var fs = new FileStream(tempFilePath, FileMode.Create))
    ////        {
    ////            await file.CopyToAsync(fs);
    ////        }

    ////        var images = ExcelImageExtractor.ExtractImagesByOrder(tempFilePath);

    ////        using var stream = new MemoryStream(System.IO.File.ReadAllBytes(tempFilePath));
    ////        using var package = new ExcelPackage(stream);
    ////        var worksheet = package.Workbook.Worksheets[0];

    ////        for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
    ////        {
    ////            var dto = Utilites.MapRowToDto<TDto>(worksheet, row, images, _fileService);

    ////            var foreignProp = typeof(TDto).GetProperties()
    ////             .Where(p => p.Name != "Id" && p.Name.EndsWith("Id") && p.PropertyType == typeof(int));

    ////            if (foreignProp != null)
    ////                foreignProp.SetValue(dto, foreignKeyValue);

    ////            if (foreignProp != null && foreignProp.PropertyType == typeof(int))
    ////                foreignProp.SetValue(dto, foreignKeyValue);

    ////            var context = new ValidationContext(dto);
    ////            var validationResults = new List<ValidationResult>();
    ////            Validator.TryValidateObject(dto, context, validationResults, true);

    ////            if (validationResults.Any())
    ////            {
    ////                var bug = new StringBuilder($"row[{row}]");
    ////                foreach (var error in validationResults)
    ////                    bug.Append(", " + error.ErrorMessage);
    ////                bugs.Add(bug.ToString());
    ////                continue;
    ////            }

    ////            dtoList.Add(dto);
    ////        }

    ////        System.IO.File.Delete(tempFilePath);

    ////        var entities = new List<TEntity>();

    ////        foreach (var dto in dtoList)
    ////        {
    ////            var entity = _mapper.Map<TEntity>(dto);

    ////            var imageProps = typeof(TDto).GetProperties().Where(p => p.PropertyType == typeof(byte[]));
    ////            foreach (var prop in imageProps)
    ////            {
    ////                var imageData = (byte[])prop.GetValue(dto);
    ////                if (imageData != null && imageData.Length > 0)
    ////                {
    ////                    var fileName = await _fileService.SaveFileAsync(imageData, ".jpg", typeof(TEntity).Name);
    ////                    var entityProp = typeof(TEntity).GetProperty(prop.Name);
    ////                    if (entityProp != null && entityProp.PropertyType == typeof(string))
    ////                        entityProp.SetValue(entity, fileName);
    ////                }
    ////            }

    ////            var entityFkProp = typeof(TEntity).GetProperties()
    ////            .FirstOrDefault(p => p.Name != "Id" && p.Name.EndsWith("Id") && p.PropertyType == typeof(int));

    ////            if (entityFkProp != null)
    ////                entityFkProp.SetValue(entity, foreignKeyValue);
    ////            if (entityFkProp != null && entityFkProp.PropertyType == typeof(int))
    ////                entityFkProp.SetValue(entity, foreignKeyValue);

    ////            entities.Add(entity);
    ////        }

    ////        return (entities, bugs);
    ////    }

        
    //}
}
