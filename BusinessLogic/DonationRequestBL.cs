using Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public partial class Logic
    {
        public async Task<int> AddDonationRequest(DonationRequestModel model)
        {
            return await _dataAccess.AddDonationRequest(model);
        }
        public async Task<bool> UpdateDonationRequest(DonationRequestModel model)
        {
            return await _dataAccess.UpdateDonationRequest(model);
        }
        public async Task<PaginatedDonationRequestModel> GetDonationRequestDetail(int organizationRequestId)
        {
            return await _dataAccess.GetDonationRequestDetail(organizationRequestId);
        }
        public async Task<PaginatedResultModel<PaginatedDonationRequestModel>> GetDonationRequests(DonationRequestSearchModel filters)
        {
            return await _dataAccess.GetDonationRequests(filters);
        }
        public async Task<List<DonationRequestOrganizationItemModel>> GetDonationRequestItems(int organizationRequestId)
        {
            return await _dataAccess.GetDonationRequestItems(organizationRequestId);
        }
    }
}
