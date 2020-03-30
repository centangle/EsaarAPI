using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using Models;
using System;
using System.Data.Entity;
using DataProvider.Helpers;

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
                        dbModel.Name = model.Name;
                        dbModel.NativeName = model.NativeName;
                        dbModel.Abbreviation = model.Abbreviation;
                        dbModel.NoOfBaseUnit = model.NoOfBaseUnit;
                        dbModel.Type = (int)model.Type;
                        dbModel.IsDeleted = false;
                        dbModel.IsActive = model.IsActive;
                        context.UOMs.Add(dbModel);
                        bool result = await context.SaveChangesAsync() > 0;
                        if (result)
                        {
                            model.Id = dbModel.Id;
                            AddChildUOM(context, model);
                            await context.SaveChangesAsync();
                        }
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
                        UOM dbModel = await context.UOMs.Where(x => x.Id == model.Id).FirstOrDefaultAsync();
                        if (dbModel != null)
                        {
                            dbModel.Name = model.Name;
                            dbModel.NativeName = model.NativeName;
                            dbModel.Abbreviation = model.Abbreviation;
                            dbModel.Type = (int)model.Type;
                            dbModel.IsDeleted = model.IsDeleted;
                            dbModel.IsActive = model.IsActive;
                            var childModified = await ModifyChildItems(context, model);
                            bool result = (await context.SaveChangesAsync() > 0 && childModified);
                            transaction.Commit();
                            return result;
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

        public async Task<bool> DeleteUOM(int Id)
        {
            using (CharityEntities context = new CharityEntities())
            {
                UOM dbModel = await context.UOMs.Where(x => x.Id == Id).FirstOrDefaultAsync();
                if (dbModel != null)
                {
                    dbModel.IsDeleted = true;
                    return await context.SaveChangesAsync() > 0;
                }
                return false;
            }
        }

        public async Task<PaginatedResultModel<UOMModel>> GetUOM(UOMSearchModel filters)
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
                    uom.Childrens = await (from u in context.UOMs
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

        public async Task<PaginatedResultModel<UOMModel>> GetUOMForDD(UOMSearchModel filters)
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

        private async Task<bool> ModifyChildItems(CharityEntities context, UOMModel model)
        {
            var masterList = await context.UOMs.Where(x => x.ParentId == model.Id).ToListAsync();
            var newItems = UpdatedListItem.NewItems(model.Childrens);
            var updatedItems = UpdatedListItem.UpdatedItems(masterList, model.Childrens);
            var deletedItems = UpdatedListItem.DeletedItems(masterList, model.Childrens);
            AddChildUOM(context, model);
            UpdateChildUOM(updatedItems, model.Childrens);
            DeleteChildUOM(deletedItems);
            return await context.SaveChangesAsync() > 0;
        }
        private void AddChildUOM(CharityEntities context, UOMModel model)
        {
            foreach (var item in model.Childrens)
            {
                UOM dbModel = new UOM();
                dbModel.Name = item.Name;
                dbModel.NativeName = item.NativeName;
                dbModel.Abbreviation = item.Abbreviation;
                dbModel.NoOfBaseUnit = item.NoOfBaseUnit;
                dbModel.IsActive = item.IsActive;
                dbModel.IsDeleted = item.IsDeleted;
                dbModel.ParentId = model.Id;
                context.UOMs.Add(dbModel);
            }
        }

        private void UpdateChildUOM(List<UOM> masterItems, ICollection<UOMModel> modifiedItems)
        {
            foreach (var cuom in modifiedItems)
            {
                UOM dbModel = masterItems.Where(x => x.Id == cuom.Id).FirstOrDefault();
                dbModel.Name = cuom.Name;
                dbModel.NativeName = cuom.NativeName;
                dbModel.Abbreviation = cuom.Abbreviation;
                dbModel.NoOfBaseUnit = cuom.NoOfBaseUnit;
                dbModel.IsActive = cuom.IsActive;
                dbModel.IsDeleted = cuom.IsDeleted;
                dbModel.ParentId = cuom.ParentId;
            }
        }

        private void DeleteChildUOM(List<UOM> deletedItems)
        {
            foreach (var item in deletedItems)
            {
                item.IsDeleted = true;
            }
        }
    }
}
