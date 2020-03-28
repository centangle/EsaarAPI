using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using Models;
using System;
using System.Data.Entity;

namespace DataProvider
{
    public partial class DataAccess
    {
        public async Task<int> AddUOM(UOMModel model)
        {
            using (CharityEntities context = new CharityEntities())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        UOM dbModel = new UOM();
                        dbModel.Id = model.Id;
                        dbModel.Name = model.Name;
                        dbModel.NativeName = model.NativeName;
                        dbModel.Abbreviation = model.Abbreviation;
                        dbModel.NoOfBaseUnit = model.NoOfBaseUnit;
                        dbModel.Type = (int)model.Type;
                        dbModel.IsDeleted = false;
                        dbModel.IsActive = model.IsActive;
                        context.UOMs.Add(dbModel);
                        bool result = await context.SaveChangesAsync() > 0;
                        model.Id = dbModel.Id;
                        await AddChildUOM(context, model);
                        transaction.Commit();
                        return model.Id;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return 0;
                    }
                }
            }
        }

        public async Task<bool> UpdateUOM(UOMModel model)
        {
            using (CharityEntities context = new CharityEntities())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        UOM uOM = await context.UOMs.Where(x => x.Id == model.Id).FirstOrDefaultAsync();
                        if (uOM != null)
                        {
                            uOM.Name = model.Name;
                            uOM.NativeName = model.NativeName;
                            uOM.Abbreviation = model.Abbreviation;
                            uOM.Type = (int)model.Type;
                            uOM.IsDeleted = model.IsDeleted;
                            await DeleteChildUOMByParentId(context, uOM.Id);
                            var childAdded = await AddChildUOM(context, model);
                            bool result = (await context.SaveChangesAsync() > 0 || childAdded);
                            transaction.Commit();
                        }
                        return false;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return false;
                    }
                }

            }
        }

        public async Task<bool> DeleteUOMById(int Id)
        {
            using (CharityEntities context = new CharityEntities())
            {
                UOM uOM = await context.UOMs.Where(x => x.Id == Id).FirstOrDefaultAsync();
                if (uOM != null)
                {
                    uOM.IsDeleted = true;
                    return await context.SaveChangesAsync() > 0;
                }
                return false;
            }
        }

        public async Task<PaginatedResultModel<UOMModel>> GetUOMByFilter(UOMSearchModel filters)
        {
            using (CharityEntities context = new CharityEntities())
            {
                PaginatedResultModel<UOMModel> searchResult = new PaginatedResultModel<UOMModel>();
                IQueryable<UOMModel> UOMsQueryable = (from u in context.UOMs
                                                      where (string.IsNullOrEmpty(filters.Name) || u.Name.ToLower().Contains(filters.Name.ToLower()))
                                                       && (u.ParentId == null)
                                                       && u.IsDeleted == false
                                                      select new UOMModel
                                                      {
                                                          Id = u.Id,
                                                          Name = u.Name,
                                                          Abbreviation = u.Abbreviation,
                                                          NoOfBaseUnit = u.NoOfBaseUnit,
                                                          ParentId = null,
                                                      }).AsQueryable();
                List<UOMModel> UOMList = await UOMsQueryable.OrderBy(x => x.Name).Skip((filters.CurrentPage - 1) * filters.RecordsPerPage).Take(filters.RecordsPerPage).ToListAsync();
                foreach (var uom in UOMList)
                {
                    uom.ChildUOMS = await (from u in context.UOMs
                                           where u.ParentId == uom.Id
                                           select new UOMModel
                                           {
                                               Id = u.Id,
                                               Name = u.Name,
                                               Abbreviation = u.Abbreviation,
                                               NoOfBaseUnit = u.NoOfBaseUnit,
                                               ParentId = u.ParentId,
                                           }).ToListAsync();
                }
                searchResult.Items = UOMList ?? new List<UOMModel>();
                if (filters.CalculateTotal)
                {
                    searchResult.TotalCount = UOMsQueryable == null ? 0 : UOMsQueryable.Count();
                }
                return searchResult;
            }
        }

        public async Task<PaginatedResultModel<UOMModel>> GetUOMsForDDAsync(UOMSearchModel filters)
        {
            using (CharityEntities context = new CharityEntities())
            {
                PaginatedResultModel<UOMModel> searchResult = new PaginatedResultModel<UOMModel>();
                List<UOMModel> UOMList = new List<UOMModel>();
                //======================Get Only Parent UOMS====================
                UOMList = await (from u in context.UOMs
                                 where
                                 (string.IsNullOrEmpty(filters.Name) || u.Name.Contains(filters.Name) || u.Abbreviation.Contains(filters.Name))
                                 && ((filters.ParentId == null || u.Id == filters.ParentId) && u.ParentId == null)
                                 && u.IsDeleted == false
                                 select new UOMModel
                                 {
                                     Id = u.Id,
                                     Name = u.Name,
                                     NoOfBaseUnit = u.NoOfBaseUnit,
                                 }).ToListAsync();
                //======================Get Only Child UOMS====================

                UOMList.AddRange(await (from u in context.UOMs
                                        where
                                        (string.IsNullOrEmpty(filters.Name) || u.Name.Contains(filters.Name) || u.Abbreviation.Contains(filters.Name))
                                        && ((filters.ParentId == null || u.ParentId == filters.ParentId) && u.ParentId != null)
                                        && u.IsDeleted == false
                                        select new UOMModel
                                        {
                                            Id = u.Id,
                                            Name = u.Name,
                                            ParentId = u.ParentId,
                                            NoOfBaseUnit = u.NoOfBaseUnit,
                                        }).ToListAsync());
                searchResult.Items = UOMList ?? new List<UOMModel>();
                if (filters.CalculateTotal)
                {
                    searchResult.TotalCount = UOMList.Count();
                }
                return searchResult;
            }
        }

        private async Task<bool> AddChildUOM(CharityEntities context, UOMModel model)
        {
            foreach (var cuom in model.ChildUOMS)
            {
                UOM dbModel = new UOM();
                dbModel.Name = cuom.Name;
                dbModel.NativeName = cuom.NativeName;
                dbModel.Abbreviation = cuom.Abbreviation;
                dbModel.NoOfBaseUnit = cuom.NoOfBaseUnit;
                dbModel.IsActive = cuom.IsActive;
                dbModel.IsDeleted = cuom.IsDeleted;
                dbModel.ParentId = model.Id;
                context.UOMs.Add(dbModel);
            }
            return await context.SaveChangesAsync() > 0;
        }

        private async Task<bool> DeleteChildUOMByParentId(CharityEntities context, int ParentId)
        {
            List<UOM> childUOMs = await context.UOMs.Where(x => x.ParentId == ParentId).AsNoTracking().ToListAsync();
            context.UOMs.RemoveRange(childUOMs);
            return await context.SaveChangesAsync() > 0;
        }
    }
}
