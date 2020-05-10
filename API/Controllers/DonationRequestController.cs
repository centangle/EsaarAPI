using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic;
using Catalogs;
using Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DonationRequestController : BaseController
    {
        private readonly Logic _logic;

        public DonationRequestController(Logic logic)
        {
            _logic = logic;
        }
        [HttpGet]
        [Route("GetPaginated")]
        public async Task<PaginatedResultModel<PaginatedDonationRequestModel>> GetPaginated(int recordsPerPage, int currentPage, PaginationOrderCatalog orderDir, bool disablePagination, int? organizationId = null, DonationRequestTypeCatalog? type = null, StatusCatalog? status = null, TimePeriodCatalog? timePeriod = null,
            DateTime? startDate = null, DateTime? endDate = null, string memberName = null
            , string orderByColumn = null, bool calculateTotal = true)
        {

            DonationRequestSearchModel filters = new DonationRequestSearchModel();
            filters.OrganizationId = organizationId;
            filters.MemberName = memberName;
            filters.Type = type;
            filters.Status = status;
            filters.TimePeriod = timePeriod;
            filters.StartDate = startDate;
            filters.EndDate = endDate;
            SetPaginationProperties(filters, recordsPerPage, currentPage, orderDir, orderByColumn, disablePagination, calculateTotal);
            return await _logic.GetDonationRequests(filters);
        }
        //[HttpGet]
        //public async Task<PaginatedResultModel<PaginatedDonationRequestModel>> GetPaginatedRequestsForVolunteer(int recordsPerPage, int currentPage, PaginationOrderCatalog orderDir, bool disablePagination, int? organizationId = null, DonationRequestTypeCatalog? type = null, string orderByColumn = null, bool calculateTotal = true)
        //{
        //    
        //    DonationRequestSearchModel filters = new DonationRequestSearchModel();
        //    filters.OrganizationId = organizationId;
        //    filters.Type = type;
        //    SetPaginationProperties(filters, recordsPerPage, currentPage, orderDir, orderByColumn, disablePagination, calculateTotal);
        //    return await _logic.GetDonationRequestsForVolunteer(filters);
        //}
        [HttpGet]
        public async Task<PaginatedDonationRequestModel> Get(int organizationRequestId)
        {

            var model = await _logic.GetDonationRequestDetail(organizationRequestId);
            if (model.CanAccessRequestThread == false)
            {
                throw new KnownException("You are not authorized");
            }
            return model;
        }
        [HttpGet]
        [Route("GetItems")]
        public async Task<List<DonationRequestOrganizationItemModel>> GetItems(int organizationRequestId)
        {

            return await _logic.GetDonationRequestItems(organizationRequestId);
        }
        [HttpPut]
        [Route("AssignModeratorToRequest")]
        public async Task<bool> AssignModeratorToRequest(int organizationId, int donationRequestId, int? moderatorId = null)
        {

            return await _logic.AssignModeratorToDonationRequest(organizationId, donationRequestId, moderatorId);
        }
        [HttpPut]
        [Route("AssignVolunteerToRequest")]
        public async Task<bool> AssignVolunteerToRequest(int organizationId, int donationRequestId, int? volunteerId = null)
        {

            return await _logic.AssignVolunteerToDonationRequest(organizationId, donationRequestId, volunteerId);
        }
        [HttpPost]
        [Route("Create")]
        public async Task<int> Create(DonationRequestModel model)
        {

            return await _logic.AddDonationRequest(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<bool> Update(DonationRequestModel model)
        {

            return await _logic.UpdateDonationRequest(model);
        }
        [HttpPost]
        [Route("UpdateStatus")]
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

            return await _logic.UpdateDonationRequestStatus(donationRequestOrganizationId, note, items, updatedStatus);
        }
        [HttpGet]
        [Route("GetRequestStatus")]
        public Array GetRequestStatus()
        {
            return Enum.GetValues(typeof(StatusCatalog)).Cast<StatusCatalog>().ToArray();
        }
        [HttpGet]
        [Route("GetRequestType")]
        public Array GetRequestType()
        {
            return Enum.GetValues(typeof(DonationRequestTypeCatalog)).Cast<DonationRequestTypeCatalog>().ToArray();
        }
        [HttpGet]
        [Route("GetTimePeriod")]
        public Array GetTimePeriod()
        {
            return Enum.GetValues(typeof(TimePeriodCatalog)).Cast<TimePeriodCatalog>().ToArray();
        }
    }
}