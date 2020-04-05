using BusinessLogic;
using Models;
using System.Threading.Tasks;
using System.Web.Http;

namespace DataManager.Controllers
{
    [Authorize]
    public class OrganizationMemberController : BaseController
    {
        [HttpPost]
        public async Task<int> RequestMembership(OrganizationRequestModel model)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.RequestOrganizationMembership(model);
        }
    }
}
