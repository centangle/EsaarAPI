using BusinessLogic;
using Models;
using System.Threading.Tasks;
using System.Web.Http;

namespace DataManager.Controllers
{
    public class CampaignController : BaseController
    {
        [HttpPost]
        public async Task<int> Create(CampaignModel model)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.CreateCampaign(model);
        }
        [HttpPut]
        public async Task<bool> Update(CampaignModel model)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.UpdateCampaign(model);
        }
    }
}
