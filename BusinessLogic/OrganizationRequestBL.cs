using Models;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public partial class Logic
    {
        public async Task<PaginatedResultModel<PaginatedOrganizationRequestModel>> GetOrganizationRequests(OrganizationRequestSearchModel filters)
        {
            return await _dataAccess.GetOrganizationRequests(filters);
        }
        public async Task<PaginatedOrganizationRequestModel> GetOrganizationRequest(int requestId)
        {
            return await _dataAccess.GetOrganizationRequest(requestId);
        }
        public async Task<bool> UpdateOrganizationRequestStatus(OrganizationRequestThreadModel model)
        {
            return await _dataAccess.UpdateOrganizationRequestStatus(model);
        }
    }
}
