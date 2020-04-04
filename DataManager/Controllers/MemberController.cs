using BusinessLogic;
using Models.BriefModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataManager.Controllers
{
    [Authorize]
    public class MemberController : BaseController
    {
        public async Task<List<MemberBriefModel>> GetMemberForDD(string filter)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.GetMemberForDD(filter);
        }
    }
}
