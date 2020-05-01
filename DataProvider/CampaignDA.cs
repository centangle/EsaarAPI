using DataProvider.Helpers;
using Helpers;
using Models;
using Models.BriefModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProvider
{
    public partial class DataAccess
    {
        public async Task<int> CreateCampaign(CampaignModel model)
        {
            using (CharityEntities context = new CharityEntities())
            {
                var memberOrgRoles = (await GetMemberRoleForOrganization(context, model.Organization.Id, _loggedInMemberId)).FirstOrDefault();
                if (IsOrganizationMemberModerator(memberOrgRoles))
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        try
                        {
                            var dbModel = SetCampaign(new Campaign(), model);
                            context.Campaigns.Add(dbModel);
                            bool result = await context.SaveChangesAsync() > 0;
                            if (result)
                            {
                                model.Id = dbModel.Id;
                                AddCampaignItems(context, model.Items, model.Id, model.Organization.Id);
                                await context.SaveChangesAsync();
                            }
                            transaction.Commit();
                            return model.Id;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw ex;
                        }
                    }
                }
                else
                    throw new KnownException("You are not authorized to perform this action");
            }
        }
        public async Task<bool> UpdateCampaign(CampaignModel model)
        {
            using (CharityEntities context = new CharityEntities())
            {
                var memberOrgRoles = (await GetMemberRoleForOrganization(context, model.Organization.Id, _loggedInMemberId)).FirstOrDefault();
                if (IsOrganizationMemberModerator(memberOrgRoles))
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        try
                        {
                            Campaign dbModel = await context.Campaigns.Where(x => x.Id == model.Id && x.IsDeleted == false).FirstOrDefaultAsync();
                            if (dbModel != null)
                            {
                                SetCampaign(dbModel, model);
                                await ModifyCampaignItems(context, model);
                                bool result = await context.SaveChangesAsync() > 0;
                                transaction.Commit();
                                return result;
                            }
                            return false;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw ex;
                        }
                    }
                }
                else
                    throw new KnownException("You are not authorized to perform this action");
            }

        }
        public async Task<bool> DeleteCampaign(int id)
        {
            using (CharityEntities context = new CharityEntities())
            {
                Campaign dbModel = await context.Campaigns.Where(x => x.Id == id && x.IsDeleted == false).FirstOrDefaultAsync();
                if (dbModel != null)
                {
                    var memberOrgRoles = (await GetMemberRoleForOrganization(context, dbModel.OrganizationId, _loggedInMemberId)).FirstOrDefault();
                    if (IsOrganizationMemberModerator(memberOrgRoles))
                    {
                        dbModel.IsDeleted = true;
                        return await context.SaveChangesAsync() > 0;
                    }
                    else
                        throw new KnownException("You are not authorized to perform this action");
                }
                return false;
            }
        }
        public async Task<CampaignModel> GetCampaign(int id)
        {
            using (CharityEntities context = new CharityEntities())
            {
                return await (from c in context.Campaigns
                              join o in context.Organizations on c.OrganizationId equals o.Id
                              join e in context.Events on c.EventId equals e.Id into le
                              from e in le.DefaultIfEmpty()
                              where c.Id == id
                              && c.IsDeleted == false
                              select new CampaignModel
                              {
                                  Id = c.Id,
                                  Name = c.Name,
                                  NativeName = c.NativeName,
                                  Description = c.Description,
                                  ImageUrl = c.ImageUrl,
                                  IsActive = c.IsActive,
                                  StartDate = c.StartDate,
                                  EndDate = c.EndDate,
                                  Organization = new BaseBriefModel()
                                  {
                                      Id = o.Id,
                                      Name = o.Name,
                                      NativeName = o.NativeName,
                                  },
                                  Event = new BaseBriefModel()
                                  {
                                      Id = e == null ? 0 : e.Id,
                                      Name = e == null ? "" : e.Name,
                                      NativeName = e == null ? "" : e.NativeName,
                                  }
                              }).FirstOrDefaultAsync();
            }
        }
        public async Task<PaginatedResultModel<CampaignModel>> GetCampaigns(CampaignSearchModel filters)
        {
            using (CharityEntities context = new CharityEntities())
            {
                var campaignQueryable = (from c in context.Campaigns
                                         join o in context.Organizations on c.OrganizationId equals o.Id
                                         join e in context.Events on c.EventId equals e.Id into le
                                         from e in le.DefaultIfEmpty()
                                         where (string.IsNullOrEmpty(filters.Name) || c.Name.Contains(filters.Name) || c.NativeName.Contains(filters.Name))
                                         && (filters.OrganizationId == null || c.OrganizationId == filters.OrganizationId)
                                         && (filters.EventId == null || c.EventId == filters.EventId)
                                         && c.IsDeleted == false
                                         select new CampaignModel
                                         {
                                             Id = c.Id,
                                             Name = c.Name,
                                             NativeName = c.NativeName,
                                             Description = c.Description,
                                             ImageUrl = c.ImageUrl,
                                             IsActive = c.IsActive,
                                             StartDate = c.StartDate,
                                             EndDate = c.EndDate,
                                             Organization = new BaseBriefModel()
                                             {
                                                 Id = o.Id,
                                                 Name = o.Name,
                                                 NativeName = o.NativeName,
                                             },
                                             Event = new BaseBriefModel()
                                             {
                                                 Id = e == null ? 0 : e.Id,
                                                 Name = e == null ? "" : e.Name,
                                                 NativeName = e == null ? "" : e.NativeName,
                                             }
                                         }).AsQueryable();
                return await campaignQueryable.Paginate(filters);
            }
        }
        private Campaign SetCampaign(Campaign dbModel, CampaignModel model)
        {
            if (model.Organization == null || model.Organization.Id == 0)
            {
                throw new KnownException("Organization for campaign must be set");
            }
            dbModel.Name = model.Name;
            dbModel.NativeName = model.NativeName;
            dbModel.OrganizationId = model.Organization.Id;
            if (model.Event != null && model.Event.Id > 0)
            {
                dbModel.EventId = model.Event.Id;
            }
            dbModel.StartDate = model.StartDate;
            dbModel.EndDate = model.EndDate;
            dbModel.Description = model.Description;
            ImageHelper.Save(model);
            dbModel.ImageUrl = model.ImageUrl;
            SetAndValidateBaseProperties(dbModel, model);
            if (dbModel.Id == 0)
            {
                dbModel.IsActive = false;
            }
            return dbModel;

        }
        private void AddCampaignItems(CharityEntities context, ICollection<OrganizationItemModel> campaignItems, int campaignId, int organizationId)
        {
            foreach (var item in campaignItems)
            {
                item.Campaign = new BaseBriefModel
                {
                    Id = campaignId,
                };
                item.Organization = new BaseBriefModel
                {
                    Id = organizationId,
                };
                var dbModel = SetOrganizationItem(new OrganizationItem(), item);
                context.OrganizationItems.Add(dbModel);
            }
        }
        private void UpdateCampaignItems(IEnumerable<OrganizationItem> modfiedItems, IEnumerable<OrganizationItemModel> campaignItems, int campaignId)
        {
            foreach (var dbModel in modfiedItems)
            {
                OrganizationItemModel model = campaignItems.Where(x => x.Id == dbModel.Id).FirstOrDefault();
                SetOrganizationItem(dbModel, model);
            }
        }
        private void DeleteCampaignItems(IEnumerable<OrganizationItem> deletedItems)
        {
            foreach (var item in deletedItems)
            {
                item.IsDeleted = true;
            }
        }
        private async Task ModifyCampaignItems(CharityEntities context, CampaignModel model)
        {
            var masterList = await context.OrganizationItems.Where(x => x.CampaignId == model.Id).ToListAsync();
            var newItems = model.Items.Where(x => x.Id == 0).ToList();
            var updatedItems = masterList.Where(m => model.Items.Any(s => m.Id == s.Id));
            var deletedItems = masterList.Where(m => !model.Items.Any(s => m.Id == s.Id));
            AddCampaignItems(context, newItems, model.Id, model.Organization.Id);
            UpdateCampaignItems(updatedItems, model.Items, model.Id);
            DeleteCampaignItems(deletedItems);
        }
    }
}
