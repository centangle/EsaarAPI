using Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public partial class Logic
    {
        public async Task<bool> ModifyCampaignItems(int campaignId, List<OrganizationItemModel> items)
        {
            return await _dataAccess.ModifyCampaignItems(campaignId, items);
        }
    }
}
