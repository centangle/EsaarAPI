using Models;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public partial class Logic
    {
        public async Task<PaginatedResultModel<OrganizationRequestModel>> GetOrganizationRequests(OrganizationRequestSearchModel filters)
        {
            return await _dataAccess.GetOrganizationRequests(filters);
        }
    }
}
