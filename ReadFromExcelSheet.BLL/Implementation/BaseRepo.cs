using Microsoft.EntityFrameworkCore;
using ReadFromExcelSheet.BLL.Helper;
using ReadFromExcelSheet.BLL.Interface;
using ReadFromExcelSheet.DAL.Database;
using ReadFromExcelSheet.DAL.Extends;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadFromExcelSheet.BLL.Implementation
{
    public class BaseRepo<Entity, SC, IdType> : IBaseRepo<Entity, SC, IdType>
         where Entity : BaseEntity<IdType>
         where SC : EntitySC
    {
        #region fields
        private readonly ApplicationDbContext context;

        // private readonly IStringLocalizer<SharedResourcesKeys> _localizer;

        #endregion


        #region Ctor
        public BaseRepo(ApplicationDbContext context /* IStringLocalizer<SharedResourcesKeys> localizer */ )
        {
            this.context = context;
            // _localizer = localizer;
        }
        #endregion


        #region Actions
        public virtual IQueryable<Entity> Search(IQueryable<Entity> qry, SC sC)
        {
            qry = qry.Where(a =>
                                (!sC.CompanyId.HasValue || a.CompanyId == sC.CompanyId) &&
                                a.IsDelete == sC.IsDelete);


            return qry;
        }

        public virtual async Task<Entity?> Get(IdType id)
        {
            Entity? entity = await context.Set<Entity>().AsNoTracking().FirstOrDefaultAsync(a => a.Id.ToString() == id.ToString());
            /*
               if (entity == null)
               {
                   throw new Exception(_localizer[SharedResourcesKeys.NotFound]);
               }
            */
            return entity;
        }

        public virtual async Task<Pagination<Entity>> GetAll(SC sC)
        {
            Pagination<Entity> data = new();
            IQueryable<Entity> qry = context.Set<Entity>().AsNoTracking().AsQueryable();
            qry = Search(qry, sC);
            data.TotalRows = qry.Count();
            //data.TotalPages = sC.PageSize > data.TotalRows ? 1 : (int)Math.Round(Convert.ToDouble(data.TotalRows) / sC.PageSize);
            data.TotalPages = (int)Math.Ceiling((double)data.TotalRows / sC.PageSize);


            data.PageIndex = sC.PageIndex;
            data.PageSize = sC.PageSize;
            qry = qry.Skip(sC.PageSize * (sC.PageIndex - 1)).Take(sC.PageSize);

            data.List = await qry.ToListAsync();
            return data;
        }


        public virtual async Task<Entity> Save(Entity entity)
        {
            try
            {
                if (Convert.ToInt32(entity.Id) != 0)
                {
                    context.Set<Entity>().Update(entity);
                    //    throw new Exception(_localizer[SharedResourcesKeys.Updated]);
                }
                else
                {
                    entity.CreatedOn = DateTime.Now;
                    await context.Set<Entity>().AddAsync(entity);
                    //   throw new Exception(_localizer[SharedResourcesKeys.Created]);
                }
                return entity;
            }
            catch (Exception ex)
            {
                //   throw new Exception(_localizer[SharedResourcesKeys.AddFailed], ex);
                throw new Exception(ex.InnerException?.Message ?? ex.Message, ex);
            }
        }
        public virtual async Task<List<Entity>> SaveRange(List<Entity> entity)
        {
            try
            {


                foreach (var item in entity)
                {
                    if (Convert.ToInt32(item.Id) != 0)
                    {
                        context.Set<Entity>().Update(item);
                    }
                    else
                    {
                        item.CreatedOn = DateTime.Now;
                        await context.Set<Entity>().AddAsync(item);
                    }
                }
                return entity;
            }
            catch (Exception ex)
            {
                // throw new Exception(_localizer[SharedResourcesKeys.AddFailed], ex);
                throw new Exception(ex.InnerException?.Message ?? ex.Message, ex);
            }
        }
        public virtual async Task<Entity?> Add(Entity entity)
        {
            try
            {


                entity.CreatedOn = DateTime.Now;
                await context.Set<Entity>().AddAsync(entity);
                return entity;
            }
            catch (Exception ex)
            {
                //  throw new Exception(_localizer[SharedResourcesKeys.AddFailed], ex);
                throw new Exception(ex.InnerException?.Message ?? ex.Message, ex);
            }
        }
        public virtual async Task<Entity?> Edit(Entity entity)
        {
            try
            {
                await Task.Delay(1);
                context.Set<Entity>().Update(entity);
                return entity;
            }
            catch (Exception ex)
            {
                //    throw new Exception(_localizer[SharedResourcesKeys.UpdateFailed], ex);
                throw new Exception(ex.InnerException?.Message ?? ex.Message, ex);
            }
        }
        public virtual async Task<Entity?> Delete(IdType id)
        {
            try
            {
                Entity? entity = await context.Set<Entity>().FirstOrDefaultAsync(a => a.Id.ToString() == id.ToString());
                if (entity is not null)
                {
                    //entity.IsActive = false;
                    entity.IsDelete = true;
                    entity.DeletedOn = DateTime.Now;
                    context.Set<Entity>().Update(entity);
                }
                return entity;
            }
            catch (Exception ex)
            {
                // throw new Exception(_localizer[SharedResourcesKeys.DeletedFailed], ex);
                throw new Exception(ex.InnerException?.Message ?? ex.Message, ex);
            }
        }
        public virtual async Task<List<Entity>> DeleteRange(List<Entity> entities)
        {
            try
            {
                if (entities is not null)
                {
                    foreach (var entity in entities)
                    {
                        entity.IsDelete = true;
                        entity.DeletedOn = DateTime.Now;
                    }
                    context.Set<Entity>().UpdateRange(entities);
                }
                return entities;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException?.Message ?? ex.Message, ex);
            }
        }


        public async Task<Entity?> ForceDelete(IdType id)
        {
            try
            {
                Entity? entity = await context.Set<Entity>().FirstOrDefaultAsync(a => a.Id.ToString() == id.ToString());
                if (entity is not null)
                    context.Set<Entity>().Remove(entity);

                return entity;
            }
            catch (Exception ex)
            {
                // throw new Exception(_localizer[SharedResourcesKeys.DeletedFailed], ex);
                throw new Exception(ex.InnerException?.Message ?? ex.Message, ex);
            }
        }


        public virtual async Task<List<Entity>> EditRange(List<Entity> entities)
        {
            try
            {
                await Task.Delay(1);
                context.Set<Entity>().UpdateRange(entities);
                return entities;
            }
            catch (Exception ex)
            {
                //   throw new Exception(_localizer[SharedResourcesKeys.UpdateFailed], ex);
                throw new Exception(ex.InnerException?.Message ?? ex.Message, ex);
            }
        }



        public virtual async Task<Entity?> Activate(IdType id)
        {
            try
            {
                Entity? entity = await context.Set<Entity>().FirstOrDefaultAsync(a => a.Id.ToString() == id.ToString());
                if (entity is not null)
                {
                    entity.IsActive = true;
                    context.Set<Entity>().Update(entity);
                }
                return entity;
            }
            catch (Exception ex)
            {
                // throw new Exception(_localizer[SharedResourcesKeys.UpdateFailed], ex);
                throw new Exception(ex.InnerException?.Message ?? ex.Message, ex);
            }
        }
        public virtual async Task<Entity?> DeActivate(IdType id)
        {
            try
            {
                Entity? entity = await context.Set<Entity>().FirstOrDefaultAsync(a => a.Id.ToString() == id.ToString());
                if (entity is not null)
                {
                    entity.IsActive = false;
                    context.Set<Entity>().Update(entity);
                }
                return entity;
            }
            catch (Exception ex)
            {
                //  throw new Exception(_localizer[SharedResourcesKeys.UpdateFailed], ex);
                throw new Exception(ex.InnerException?.Message ?? ex.Message, ex);
            }
        }


        public virtual async Task<int> Count(SuperAdminFilter filter)
        {
            int count = await context.Set<Entity>().CountAsync(a =>
               !filter.CompanyId.HasValue || a.CompanyId == filter.CompanyId &&
               !filter.IsDelete.HasValue || a.IsDelete == filter.IsDelete
               );
            return count;
        }


        #endregion
    }
}
