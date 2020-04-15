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
                var currentItems = await GetCurrentOrganizationItems(context, model.Organization.Id);
                if (!DoesOrganizationItemBindingExist(model.Organization.Id, model.Item.Id, currentItems))
                {
                    var dbModel = SetOrganizationItem(new OrganizationItem(), model);
                    context.OrganizationItems.Add(dbModel);
                    await context.SaveChangesAsync();
                    model.Id = dbModel.Id;
                    return model.Id;
                }
                return 0;
            }
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
            SetBaseProperties(dbModel, model);
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
                                  Item = new BaseBriefModel()
                                  {
                                      Id = i.Id,
                                      Name = i.Name,
                                      NativeName = i.NativeName,
                                  },
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
                                        where o.Id == filters.OrganizationId
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
                                            Item = new BaseBriefModel()
                                            {
                                                Id = i.Id,
                                                Name = i.Name,
                                                NativeName = i.NativeName,
                                            },
                                            IsActive = oi.IsActive,
                                        }).AsQueryable();
                return await orgItemQueryable.Paginate(filters);
            }

        }
        private async Task<List<OrganizationItem>> GetCurrentOrganizationItems(CharityEntities context, int organizationId)
        {
            return await context.OrganizationItems.Where(x => x.OrganizationId == organizationId && x.IsDeleted == false).ToListAsync();
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
