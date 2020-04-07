using Models;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public partial class Logic
    {
        public async Task<bool> AssignRequest(int organizationId, int requestId, int? moderatorId)
        {
            return await _dataAccess.AssignRequest(organizationId, requestId, moderatorId);
        }
        public async Task<PaginatedResultModel<OrganizationRequestModel>> GetOrganizationRequests(OrganizationRequestSearchModel filters)
        {
            return await _dataAccess.GetOrganizationRequests(filters);
        }
    }
}
