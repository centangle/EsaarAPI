using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public async Task<bool> AssignDonationRequest(int organizationId, int donationRequestId, int? moderatorId)
        {
            return await _dataAccess.AssignDonationRequest(organizationId, donationRequestId, moderatorId);
        }
        public async Task<DonationRequestModel> GetBriefDonationRequest(int organizationRequestId)
        {
            return await _dataAccess.GetBriefDonationRequest(organizationRequestId);
        }
        public async Task<PaginatedResultModel<PaginatedDonationRequestModel>> GetDonationRequests(DonationRequestSearchModel filters)
        {
            return await _dataAccess.GetDonationRequests(filters);
        }
    }
}
