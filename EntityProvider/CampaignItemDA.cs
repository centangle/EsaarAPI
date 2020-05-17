using EntityProvider.DbModels;
using Helpers;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.BriefModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityProvider
{
    public partial class DataAccess
    {
        public async Task<bool> ModifyCampaignItems(int campaignId, List<OrganizationItemModel> items)
        {
            var campaign = await _context.Campaigns.Where(x => x.Id == campaignId).FirstOrDefaultAsync();
            if (campaign != null)
            {
                var masterList = await _context.OrganizationItems.Where(x => x.CampaignId == campaign.Id).ToListAsync();
                var newItems = items.Where(x => x.Id == 0).ToList();
                var updatedItems = masterList.Where(m => items.Any(s => m.Id == s.Id));
                var deletedItems = masterList.Where(m => !items.Any(s => m.Id == s.Id));
                AddCampaignItems(_context, newItems, campaign.Id, campaign.OrganizationId);
                UpdateCampaignItems(updatedItems, items, campaign.Id);
                DeleteCampaignItems(deletedItems);
                return (await _context.SaveChangesAsync() > 0);
            }
            else
            {
                throw new KnownException("No such campaign exist");
            }
        }
        private void AddCampaignItems(CharityContext _context, ICollection<OrganizationItemModel> campaignItems, int campaignId, int organizationId)
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
                _context.OrganizationItems.Add(dbModel);
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
        public async Task<PaginatedResultModel<OrganizationItemPaginationModel>> GetCampaignItems(OrganizationItemSearchModel filters)
        {
            var campaign = await _context.Campaigns.Where(x => x.Id == filters.CampaignId).FirstOrDefaultAsync();
            if (campaign != null)
            {
                filters.OrganizationId = campaign.OrganizationId;
                return await GetOrganizationItems(filters);
            }
            else
            {
                return new PaginatedResultModel<OrganizationItemPaginationModel>();
            }

        }

    }
}
