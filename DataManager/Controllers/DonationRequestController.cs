using BusinessLogic;
using Catalogs;
using Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace DataManager.Controllers
{
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
        [HttpGet]
        public async Task<DonationRequestModel> Get(int organizationRequestId)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.GetBriefDonationRequest(organizationRequestId);
        }
        [HttpPut]
        public async Task<bool> AssignRequest(int organizationId, int donationRequestId, int? moderatorId = null)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.AssignDonationRequest(organizationId, donationRequestId, moderatorId);
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
        public async Task<bool> Approve(List<DonationRequestOrganizationItemModel> items, int donationRequestOrganizationId)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.AddDonationRequestOrganizationItems(items, donationRequestOrganizationId);
        }
        [HttpPut]
        public async Task<bool> Collect(List<DonationRequestOrganizationItemModel> items, int donationRequestOrganizationId)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.UpdateDonationRequestOrganizationItems(items, donationRequestOrganizationId);
        }
        [HttpPut]
        public async Task<bool> Deliver(List<DonationRequestOrganizationItemModel> items, int donationRequestOrganizationId)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.UpdateDonationRequestOrganizationItems(items, donationRequestOrganizationId);
        }
    }
}
