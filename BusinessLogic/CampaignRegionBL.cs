using Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public partial class Logic
    {
        public async Task<bool> ModifyCampaignRegions(int campaignId, List<EntityRegionModel> regions)
        {
            return await _dataAccess.ModifyCampaignRegions(campaignId, regions);
        }
    }
}
