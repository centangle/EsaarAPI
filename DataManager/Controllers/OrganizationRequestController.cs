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
    public class OrganizationRequestController : BaseController
    {
        [HttpPut]
        public async Task<bool> AssignRequest(int organizationId, int requestId, int? moderatorId = null)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.AssignOrganizationRequest(organizationId, requestId, moderatorId);
        }
        [HttpGet]
        public async Task<PaginatedResultModel<PaginatedOrganizationRequestModel>> GetPaginated(int recordsPerPage, int currentPage, PaginationOrderCatalog orderDir, bool disablePagination, int? organizationId = null, OrganizationRequestTypeCatalog? type = null, string orderByColumn = null, bool calculateTotal = true)
        {
            var _logic = new Logic(LoggedInMemberId);
            OrganizationRequestSearchModel filters = new OrganizationRequestSearchModel();
            filters.OrganizationId = organizationId;
            filters.Types = type;
            SetPaginationProperties(filters, recordsPerPage, currentPage, orderDir, orderByColumn, disablePagination, calculateTotal);
            return await _logic.GetOrganizationRequests(filters);
        }
        [HttpGet]
        public async Task<PaginatedOrganizationRequestModel> Get(int requestId)
        {
            var _logic = new Logic(LoggedInMemberId);
            var model = await _logic.GetOrganizationRequest(requestId);
            if (model.CanAccessRequestThread == false)
            {
                throw new KnownException("You are not authorized");
            }
            return model;
        }
        [HttpGet]
        public Array GetRequestStatus()
        {
            return Enum.GetValues(typeof(StatusCatalog)).Cast<StatusCatalog>()
                .Where(x => x != StatusCatalog.Collected
                && x != StatusCatalog.Delivered
                && x != StatusCatalog.VolunteerAssigned
                ).ToArray();
        }
    }
}
