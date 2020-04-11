using BusinessLogic;
using Models;
using System.Threading.Tasks;
using System.Web.Http;

namespace DataManager.Controllers
{
    public class DonationRequestController : BaseController
    {
        [HttpPost]
        public async Task<int> Create(DonationRequestModel model)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.AddDonationRequest(model);
        }
        [HttpPut]
        public async Task<bool> Update(DonationRequestModel model)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.UpdateDonationRequest(model);
        }
    }
}
