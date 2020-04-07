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
        [HttpGet]
        public async Task<PaginatedResultModel<OrganizationMemberModel>> GetPaginated(int recordsPerPage, int currentPage, PaginationOrderCatalog orderDir, bool disablePagination, int? organizationId = null, string organizationName = null, int? memberId = null, string memberName = null, OrganizationMemberRolesCatalog? type = null, string orderByColumn = null, bool calculateTotal = true)
        {
            var _logic = new Logic(LoggedInMemberId);
            OrganizationMemberSearchModel filters = new OrganizationMemberSearchModel();
            filters.OrganizationId = organizationId;
            filters.OrganizationName = organizationName;
            filters.MemberId = memberId;
            filters.MemberName = memberName;
            filters.Role = type;
            SetPaginationProperties(filters, recordsPerPage, currentPage, orderDir, orderByColumn, disablePagination, calculateTotal);
            return await _logic.GetOrganizationMembers(filters);
        }
        [HttpGet]
        public async Task<PaginatedResultModel<OrganizationMemberModel>> GetOrganizationMembersForDD(int numberOfRecords, int organizationId, OrganizationMemberRolesCatalog type, string memberName = null)
        {
            var _logic = new Logic(LoggedInMemberId);
            OrganizationMemberSearchModel filters = new OrganizationMemberSearchModel();
            filters.OrganizationId = organizationId;
            filters.MemberName = memberName;
            filters.Role = type;
            filters.RecordsPerPage = numberOfRecords;
            return await _logic.GetOrganizationMembers(filters);
        }

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
