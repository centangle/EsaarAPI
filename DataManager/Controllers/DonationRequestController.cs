using BusinessLogic;
using Catalogs;
using Helpers;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace DataManager.Controllers
{
    [Authorize]
    public class DonationRequestController : BaseController
    {

        [HttpGet]
        public async Task<PaginatedResultModel<PaginatedDonationRequestModel>> GetPaginated(int recordsPerPage, int currentPage, PaginationOrderCatalog orderDir, bool disablePagination, int? organizationId = null, DonationRequestTypeCatalog? type = null, string orderByColumn = null, bool calculateTotal = true)
        {
            var _logic = new Logic(LoggedInMemberId);
            DonationRequestSearchModel filters = new DonationRequestSearchModel();
            filters.OrganizationId = organizationId;
            filters.Type = type;
            SetPaginationProperties(filters, recordsPerPage, currentPage, orderDir, orderByColumn, disablePagination, calculateTotal);
            return await _logic.GetDonationRequests(filters);
        }
        //[HttpGet]
        //public async Task<PaginatedResultModel<PaginatedDonationRequestModel>> GetPaginatedRequestsForVolunteer(int recordsPerPage, int currentPage, PaginationOrderCatalog orderDir, bool disablePagination, int? organizationId = null, DonationRequestTypeCatalog? type = null, string orderByColumn = null, bool calculateTotal = true)
        //{
        //    var _logic = new Logic(LoggedInMemberId);
        //    DonationRequestSearchModel filters = new DonationRequestSearchModel();
        //    filters.OrganizationId = organizationId;
        //    filters.Type = type;
        //    SetPaginationProperties(filters, recordsPerPage, currentPage, orderDir, orderByColumn, disablePagination, calculateTotal);
        //    return await _logic.GetDonationRequestsForVolunteer(filters);
        //}
        [HttpGet]
        public async Task<PaginatedDonationRequestModel> Get(int organizationRequestId)
        {
            var _logic = new Logic(LoggedInMemberId);
            var model = await _logic.GetDonationRequestDetail(organizationRequestId);
            if (model.CanAccessRequestThread == false)
            {
                throw new KnownException("You are not authorized");
            }
            return model;
        }
        [HttpGet]
        public async Task<List<DonationRequestOrganizationItemModel>> GetItems(int organizationRequestId)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.GetDonationRequestItems(organizationRequestId);
        }
        [HttpPut]
        public async Task<bool> AssignModeratorToRequest(int organizationId, int donationRequestId, int? moderatorId = null)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.AssignModeratorToDonationRequest(organizationId, donationRequestId, moderatorId);
        }
        [HttpPut]
        public async Task<bool> AssignVolunteerToRequest(int organizationId, int donationRequestId, int? volunteerId = null)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.AssignVolunteerToDonationRequest(organizationId, donationRequestId, volunteerId);
        }
        [HttpPost]
        public async Task<int> Create(DonationRequestModel model)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.AddDonationRequest(model);
        }
        [HttpPut]
        public async Task<bool> Update(DonationRequestModel model)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.UpdateDonationRequest(model);
        }
        [HttpPost]
        public async Task<bool> UpdateStatus(int donationRequestOrganizationId, List<DonationRequestOrganizationItemModel> items, DonationRequestUpdateStatusCatalog status, string note = null)
        {
            StatusCatalog updatedStatus;
            if (status == DonationRequestUpdateStatusCatalog.Approved)
            {
                updatedStatus = StatusCatalog.Approved;
            }
            else if (status == DonationRequestUpdateStatusCatalog.Collected)
            {
                updatedStatus = StatusCatalog.Collected;
            }
            else
            {
                updatedStatus = StatusCatalog.Delivered;
            }
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.UpdateDonationRequestStatus(donationRequestOrganizationId, note, items, updatedStatus);
        }
        [HttpGet]
        public Array GetRequestStatus()
        {
            return Enum.GetValues(typeof(StatusCatalog)).Cast<StatusCatalog>().ToArray();
        }
    }
}
