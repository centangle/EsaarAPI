using EntityProvider.DbModels;
using Helpers;
using Models;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace EntityProvider
{
    public partial class DataAccess
    {
        public async Task<bool> ModifyCampaignRegions(int campaignId, List<EntityRegionModel> regions)
        {
            Campaign campaign = await _context.Campaigns.Where(x => x.Id == campaignId && x.IsDeleted == false).FirstOrDefaultAsync();
            if (campaign != null)
            {
                return await ModifyEntityRegions(_context, regions, campaign.OrganizationId, null, true);
            }
            else
            {
                throw new KnownException("No such campaign exist");
            }
        }
    }
}
