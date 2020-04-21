using Catalogs;
using DataProvider.Helpers;
using Helpers;
using Models;
using Models.BriefModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DataProvider
{
    public partial class DataAccess
    {
        public async Task<int> CreateOrganizationItem(OrganizationItemModel model)
        {
            using (CharityEntities context = new CharityEntities())
            {
                var dbModel = await CreateOrganizationItem(context, model);
                await context.SaveChangesAsync();
                model.Id = dbModel.Id;
                return model.Id;
            }
        }
        private async Task<OrganizationItem> CreateOrganizationItem(CharityEntities context, OrganizationItemModel model)
        {
            var currentItems = await GetCurrentOrganizationItems(context, model.Organization.Id);
            if (!DoesOrganizationItemBindingExist(model.Organization.Id, model.Item.Id, currentItems))
            {
                var dbModel = SetOrganizationItem(new OrganizationItem(), model);
                context.OrganizationItems.Add(dbModel);
                return dbModel;
            }
            return null;
        }
        public async Task<bool> UpdateOrganizationItem(OrganizationItemModel model)
        {
            using (CharityEntities context = new CharityEntities())
            {
                OrganizationItem dbModel = await context.OrganizationItems.Where(x => x.Id == model.Id && x.IsDeleted == false).FirstOrDefaultAsync();
                if (dbModel != null)
                {
                    if (dbModel.ItemId != model.Item.Id)
                    {
                        throw new KnownException("Cannot change item in update");
                    }
                    SetOrganizationItem(dbModel, model);
                    return await context.SaveChangesAsync() > 0;
                }
                return false;
            }

        }
        public async Task<bool> CreateMultipleOrganizationItem(List<OrganizationItemModel> organizationItems)
        {
            using (CharityEntities context = new CharityEntities())
            {
                var currentItems = await GetCurrentOrganizationItems(context, organizationItems.FirstOrDefault().Organization.Id);
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (var item in organizationItems)
                        {
                            if (!DoesOrganizationItemBindingExist(item.Organization.Id, item.Item.Id, currentItems))
                            {
                                var dbModel = SetOrganizationItem(new OrganizationItem(), item);
                                context.OrganizationItems.Add(dbModel);
                            }
                        }
                        await context.SaveChangesAsync();
                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
        }
        public async Task<bool> UpdateMultipleOrganizationItem(List<OrganizationItemModel> organizationItems)
        {
            using (CharityEntities context = new CharityEntities())
            {

                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (var orgItemModel in organizationItems)
                        {
                            OrganizationItem dbModel = await context.OrganizationItems.Where(x => x.Id == orgItemModel.Id && x.IsDeleted == false).FirstOrDefaultAsync();
                            if (dbModel != null)
                            {
                                if (dbModel.ItemId != orgItemModel.Item.Id)
                                {
                                    throw new KnownException("Cannot change item in update");
                                }
                                SetOrganizationItem(dbModel, orgItemModel);
                            }
                        }
                        await context.SaveChangesAsync();
                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }

        }
        public OrganizationItem SetOrganizationItem(OrganizationItem dbModel, OrganizationItemModel model)
        {
            if (model.Organization == null || model.Organization.Id < 1)
            {
                throw new KnownException("Organization is required.");
            }
            if (model.Item == null || model.Item.Id < 1)
            {
                throw new KnownException("Item is required.");
            }
            dbModel.OrganizationId = model.Organization.Id;
            dbModel.ItemId = model.Item.Id;
            if (model.Campaign == null || model.Campaign.Id == 0)
            {
                dbModel.CampaignId = null;
            }
            else
            {
                dbModel.CampaignId = model.Campaign.Id;
            }
            if (model.CampaignItemUOM == null || model.CampaignItemUOM.Id == 0)
            {
                dbModel.CampaignItemUOM = null;
            }
            else
            {
                dbModel.CampaignItemUOM = model.CampaignItemUOM.Id;
            }
            dbModel.CampaignItemTarget = model.CampaignItemTarget;
            dbModel.IsCampaignItemOnly = model.IsCampaignItemOnly;
            SetAndValidateBaseProperties(dbModel, model);
            return dbModel;
        }
        public async Task<bool> DeleteOrganizationItems(List<int> ids)
        {
            using (CharityEntities context = new CharityEntities())
            {

                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (var id in ids)
                        {
                            OrganizationItem dbModel = await context.OrganizationItems.Where(x => x.Id == id && x.IsDeleted == false).FirstOrDefaultAsync();
                            dbModel.IsDeleted = true;
                        }
                        await context.SaveChangesAsync();
                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }

        }
        public async Task<bool> DeleteOrganizationItem(int organizationId, int itemId)
        {
            using (CharityEntities context = new CharityEntities())
            {
                OrganizationItem dbModel = await context.OrganizationItems.Where(x => x.OrganizationId == organizationId && x.ItemId == itemId && x.IsDeleted == false).FirstOrDefaultAsync();
                if (dbModel != null)
                {
                    dbModel.IsDeleted = true;
                    return await context.SaveChangesAsync() > 0;
                }
            }
            return false;


        }
        public async Task<OrganizationItemModel> GetOrganizationItem(int id)
        {
            using (CharityEntities context = new CharityEntities())
            {
                return await (from oi in context.OrganizationItems
                              join o in context.Organizations on oi.OrganizationId equals o.Id
                              join i in context.Items on oi.ItemId equals i.Id
                              join c in context.Campaigns on oi.CampaignId equals c.Id into tc
                              from c in tc.DefaultIfEmpty()
                              join ciuom in context.UOMs on oi.CampaignItemUOM equals ciuom.Id into tci
                              from ciuom in tci.DefaultIfEmpty()
                              where oi.Id == id
                              && oi.IsDeleted == false
                              select new OrganizationItemModel
                              {
                                  Id = oi.Id,
                                  Organization = new BaseBriefModel()
                                  {
                                      Id = o.Id,
                                      Name = o.Name,
                                      NativeName = o.NativeName,
                                  },
                                  Campaign = new BaseBriefModel()
                                  {
                                      Id = c == null ? 0 : c.Id,
                                      Name = c == null ? "" : c.Name,
                                      NativeName = c == null ? "" : c.NativeName,
                                  },
                                  Item = new ItemBriefModel()
                                  {
                                      Id = i.Id,
                                      Name = i.Name,
                                      NativeName = i.NativeName,
                                      ImageUrl = i.ImageUrl,
                                      Worth = i.Worth ?? 0,
                                      Description = i.Description,
                                  },
                                  CampaignItemTarget = oi.CampaignItemTarget ?? 0,
                                  CampaignItemUOM = new UOMBriefModel()
                                  {
                                      Id = ciuom == null ? 0 : ciuom.Id,
                                      Name = ciuom == null ? "" : ciuom.Name,
                                      NativeName = ciuom == null ? "" : ciuom.NativeName,
                                      NoOfBaseUnit = ciuom == null ? 0 : ciuom.NoOfBaseUnit,
                                  },
                                  IsCampaignItemOnly = oi.IsCampaignItemOnly,
                                  IsActive = oi.IsActive,
                              }).FirstOrDefaultAsync();
            }

        }
        public async Task<PaginatedResultModel<OrganizationItemPaginationModel>> GetOrganizationItems(OrganizationItemSearchModel filters)
        {
            using (CharityEntities context = new CharityEntities())
            {
                var orgItemQueryable = (from oi in context.OrganizationItems
                                        join o in context.Organizations on oi.OrganizationId equals o.Id
                                        join i in context.Items on oi.ItemId equals i.Id
                                        join iuom in context.UOMs on i.DefaultUOM equals iuom.Id
                                        where o.Id == filters.OrganizationId
                                        && (filters.CampaignId == null || oi.CampaignId == filters.CampaignId)
                                        && (filters.Type == SearchItemTypeCatalog.All || i.Type == (int)filters.Type)
                                        && (string.IsNullOrEmpty(filters.ItemName) || i.Name.Contains(filters.ItemName))
                                        && oi.IsDeleted == false
                                        select new OrganizationItemPaginationModel
                                        {
                                            Id = oi.Id,
                                            Organization = new BaseBriefModel()
                                            {
                                                Id = o.Id,
                                                Name = o.Name,
                                                NativeName = o.NativeName,
                                            },
                                            ItemName = i.Name,
                                            ItemNativeName = i.NativeName,
                                            Item = new ItemBriefModel()
                                            {
                                                Id = i.Id,
                                                Name = i.Name,
                                                NativeName = i.NativeName,
                                                ImageUrl = i.ImageUrl,
                                                Worth = i.Worth ?? 0,
                                                Description = i.Description,
                                            },
                                            ItemDefaultUOM = new UOMBriefParentModel()
                                            {
                                                Id = iuom.Id,
                                                Name = iuom.Name,
                                                NativeName = iuom.NativeName,
                                                ParentId = iuom.ParentId,
                                            },
                                            IsActive = oi.IsActive,
                                        }).AsQueryable();
                var paginatedResult = await orgItemQueryable.Paginate(filters);

                // Fill UOM From UI DropDown
                List<UOMModel> availableUOM = new List<UOMModel>();
                foreach (var paginatedItem in paginatedResult.Items)
                {
                    int itemDefaultUOMRootId = (paginatedItem.ItemDefaultUOM.ParentId == null ?
                        paginatedItem.ItemDefaultUOM.Id :
                        (paginatedItem.ItemDefaultUOM.ParentId ?? 0));
                    var existingUOM = availableUOM.Where(x => x.Id == itemDefaultUOMRootId).FirstOrDefault();
                    if (existingUOM == null)
                    {
                        var uom = await GetUOM(itemDefaultUOMRootId);
                        if (uom != null)
                        {
                            availableUOM.Add(uom);
                            paginatedItem.ItemUOMs.AddRange(GetUOMTreeFlatList(uom));
                        }
                    }

                }
                return paginatedResult;
            }

        }
        private async Task<List<OrganizationItem>> GetCurrentOrganizationItems(CharityEntities context, int organizationId)
        {
            return await context.OrganizationItems.Where(x => x.OrganizationId == organizationId && (x.CampaignId == null || x.CampaignId == 0) && x.IsDeleted == false).ToListAsync();
        }
        private bool DoesOrganizationItemBindingExist(int organizationId, int itemId, List<OrganizationItem> currentItems)
        {
            var alreadyExist = currentItems.Where(x => x.OrganizationId == organizationId && x.ItemId == itemId && x.IsDeleted == false).Count() > 0;
            if (alreadyExist)
            {
                throw new KnownException("This item already exist in organization");
            }
            else
                return false;
        }
    }
}
