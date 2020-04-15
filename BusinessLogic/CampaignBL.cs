using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public partial class Logic
    {
        public async Task<int> CreateCampaign(CampaignModel model)
        {
            return await _dataAccess.CreateCampaign(model);
        }
        public async Task<bool> UpdateCampaign(CampaignModel model)
        {
            return await _dataAccess.UpdateCampaign(model);
        }
    }
}
