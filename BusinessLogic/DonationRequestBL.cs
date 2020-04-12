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
        public async Task<PaginatedResultModel<PaginatedDonationRequestModel>> GetDonationRequests(DonationRequestSearchModel filters)
        {
            return await _dataAccess.GetDonationRequests(filters);
        }
    }
}
