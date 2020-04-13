﻿using Models;
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
        public async Task<bool> AssignModeratorToDonationRequest(int organizationId, int donationRequestId, int? moderatorId)
        {
            return await _dataAccess.AssignModeratorToDonationRequest(organizationId, donationRequestId, moderatorId);
        }
        public async Task<bool> AssignVolunteerToDonationRequest(int organizationId, int donationRequestId, int? volunteerId)
        {
            return await _dataAccess.AssignVolunteerToDonationRequest(organizationId, donationRequestId, volunteerId);
        }
        public async Task<DonationRequestModel> GetBriefDonationRequest(int organizationRequestId)
        {
            return await _dataAccess.GetBriefDonationRequest(organizationRequestId);
        }
        public async Task<PaginatedResultModel<PaginatedDonationRequestModel>> GetDonationRequests(DonationRequestSearchModel filters)
        {
            return await _dataAccess.GetDonationRequests(filters);
        }
        public async Task<PaginatedResultModel<PaginatedDonationRequestModel>> GetDonationRequestsForVolunteer(DonationRequestSearchModel filters)
        {
            return await _dataAccess.GetDonationRequestsForVolunteer(filters);
        }
    }
}
