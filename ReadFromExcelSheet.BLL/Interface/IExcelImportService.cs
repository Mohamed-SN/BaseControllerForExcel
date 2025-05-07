using Microsoft.AspNetCore.Http;
using ReadFromExcelSheet.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReadFromExcelSheet.BLL.Interface
{
    public interface IExcelImportService
    {
        //Task<List<TDataDto>> ImportExcelAsync<TFileDto, TDataDto>(TFileDto fileDto)
        //    where TFileDto : BaseFileDto
        //    where TDataDto : class, new();

        Task<(List<TEntity> Entities, List<string> Errors)> ProcessExcelImport<TDto, TEntity>(
        IFormFile file,
        int foreignKeyValue)
        where TDto : class, new()
        where TEntity : class, new();

    }
}
