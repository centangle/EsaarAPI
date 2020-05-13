using EntityProvider.Helpers;
using Helpers;
using Models;
using Models.BriefModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EntityProvider.DbModels;
using Microsoft.EntityFrameworkCore;

namespace EntityProvider
{
    public partial class DataAccess
    {
        public async Task<int> CreateCampaign(CampaignModel model)
        {
            var memberOrgRoles = (await GetMemberRoleForOrganization(_context, model.Organization.Id, _loggedInMemberId)).FirstOrDefault();
            if (IsOrganizationMemberModerator(memberOrgRoles))
            {
                try
                {
                    var dbModel = SetCampaign(new Campaign(), model);
                    _context.Campaigns.Add(dbModel);
                    await _context.SaveChangesAsync();
                    model.Id = dbModel.Id;
                    return model.Id;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
                throw new KnownException("You are not authorized to perform this action");
        }
        public async Task<bool> UpdateCampaign(CampaignModel model)
        {
            var memberOrgRoles = (await GetMemberRoleForOrganization(_context, model.Organization.Id, _loggedInMemberId)).FirstOrDefault();
            if (IsOrganizationMemberModerator(memberOrgRoles))
            {
                try
                {
                    Campaign dbModel = await _context.Campaigns.Where(x => x.Id == model.Id && x.IsDeleted == false).FirstOrDefaultAsync();
                    if (dbModel != null)
                    {
                        SetCampaign(dbModel, model);
                        bool result = await _context.SaveChangesAsync() > 0;
                        return result;
                    }
                    return false;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
                throw new KnownException("You are not authorized to perform this action");
        }
        public async Task<bool> DeleteCampaign(int id)
        {
            Campaign dbModel = await _context.Campaigns.Where(x => x.Id == id && x.IsDeleted == false).FirstOrDefaultAsync();
            if (dbModel != null)
            {
                var memberOrgRoles = (await GetMemberRoleForOrganization(_context, dbModel.OrganizationId, _loggedInMemberId)).FirstOrDefault();
                if (IsOrganizationMemberModerator(memberOrgRoles))
                {
                    dbModel.IsDeleted = true;
                    return await _context.SaveChangesAsync() > 0;
                }
                else
                    throw new KnownException("You are not authorized to perform this action");
            }
            return false;
        }
        public async Task<CampaignModel> GetCampaign(int id)
        {
            return await (from c in _context.Campaigns
                          join o in _context.Organizations on c.OrganizationId equals o.Id
                          join e in _context.Events on c.EventId equals e.Id into le
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
        public async Task<PaginatedResultModel<CampaignModel>> GetCampaigns(CampaignSearchModel filters)
        {
            var campaignQueryable = (from c in _context.Campaigns
                                     join o in _context.Organizations on c.OrganizationId equals o.Id
                                     join e in _context.Events on c.EventId equals e.Id into le
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
            ImageHelper.Save(model, GetBaseUrl());
            dbModel.ImageUrl = model.ImageUrl;
            SetAndValidateBaseProperties(dbModel, model);
            if (dbModel.Id == 0)
            {
                dbModel.IsActive = false;
            }
            return dbModel;

        }

    }
}
