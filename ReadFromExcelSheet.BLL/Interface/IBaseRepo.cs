using ReadFromExcelSheet.BLL.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadFromExcelSheet.BLL.Interface
{
    public interface IBaseRepo<Entity, SC, IdType>
    where Entity : class
    where SC : EntitySC
    {
        Task<Entity?> Get(IdType id);
        Task<Pagination<Entity>> GetAll(SC sC);
        IQueryable<Entity> Search(IQueryable<Entity> qry, SC sC);

        Task<Entity> Save(Entity entity);
        Task<List<Entity>> SaveRange(List<Entity> entity);
        Task<Entity?> Add(Entity entity);
        Task<Entity?> Edit(Entity entity);
        Task<Entity?> Delete(IdType id);
        Task<Entity?> ForceDelete(IdType id);
        Task<List<Entity>> EditRange(List<Entity> entities);
        Task<List<Entity>> DeleteRange(List<Entity> entities);

        Task<Entity?> Activate(IdType id);
        Task<Entity?> DeActivate(IdType id);

        Task<int> Count(SuperAdminFilter filter);

    }
}
