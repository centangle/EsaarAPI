using BusinessLogic;
using Catalogs;
using Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Xml.XPath;

namespace DataManager.Controllers
{
    [Authorize]
    public class OrganizationMemberController : BaseController
    {
        [HttpPost]
        public async Task<List<OrganizationMemberRolesCatalog>> GetMemberRole(int organizationId)
        {
            var _logic = new Logic(LoggedInMemberId);
            var result = (await _logic.GetMemberRoleForOrganization(organizationId, LoggedInMemberId)).FirstOrDefault();
            if (result != null)
                return result.Roles;
            return new List<OrganizationMemberRolesCatalog>();
        }
        [HttpPost]
        public async Task<int> RequestMembership(OrganizationRequestModel model)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.RequestOrganizationMembership(model);
        }
    }
}
