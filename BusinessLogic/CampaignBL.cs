using Models;
using Models.BriefModel;
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
        public async Task<bool> DeleteCampaign(int id)
        {
            return await _dataAccess.DeleteCampaign(id);
        }
        public async Task<CampaignModel> GetCampaign(int id)
        {
            return await _dataAccess.GetCampaign(id);
        }
        public async Task<PaginatedResultModel<CampaignModel>> GetCampaigns(CampaignSearchModel filters)
        {
            return await _dataAccess.GetCampaigns(filters);
        }
        public async Task<List<ItemBriefModel>> GetRootCategoriesByCampaign(int campaignId)
        {
            return await _dataAccess.GetRootCategoriesByCampaign(campaignId);
        }
    }
}
