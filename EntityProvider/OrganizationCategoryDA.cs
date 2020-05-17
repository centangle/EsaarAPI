using Models.BriefModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EntityProvider
{
    public partial class DataAccess
    {
        public async Task<List<ItemBriefModel>> GetRootCategoriesByOrgOrCampaign(int organizationId, int? campaignId = null)
        {
            var categoriesIds = (await (from i in _context.Items
                                        join oi in _context.OrganizationItems on i.Id equals oi.ItemId
                                        where

                                        oi.OrganizationId == organizationId
                                        && (campaignId == null || oi.CampaignId == campaignId)
                                        && i.IsDeleted == false
                                        && oi.IsDeleted == false
                                        && i.IsPeripheral == true
                                        && i.RootId != null
                                        select i).ToListAsync()).GroupBy(x => x.RootId).Select(x => x.Key);
            return await (from i in _context.Items
                          where categoriesIds.Contains(i.Id)
                          select new ItemBriefModel
                          {
                              Id = i.Id,
                              Name = i.Name,
                              NativeName = i.NativeName,
                              ImageUrl = i.ImageUrl,
                          }).ToListAsync();
        }
    }
}
