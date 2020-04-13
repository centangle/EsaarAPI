using Models;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public partial class Logic
    {
        public async Task<bool> AssignOrganizationRequest(int organizationId, int requestId, int? moderatorId)
        {
            return await _dataAccess.AssignOrganizationRequest(organizationId, requestId, moderatorId);
        }
        public async Task<PaginatedResultModel<PaginatedOrganizationRequestModel>> GetOrganizationRequests(OrganizationRequestSearchModel filters)
        {
            return await _dataAccess.GetOrganizationRequests(filters);
        }
    }
}
