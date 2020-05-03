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
    public class OrganizationRequestController : BaseController
    {
        [HttpPut]
        [Route("AssignRequest")]
        public async Task<bool> AssignRequest(int organizationId, int requestId, int? moderatorId = null)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.AssignOrganizationRequest(organizationId, requestId, moderatorId);
        }
        [HttpGet]
        [Route("GetPaginated")]
        public async Task<PaginatedResultModel<PaginatedOrganizationRequestModel>> GetPaginated(int recordsPerPage, int currentPage, PaginationOrderCatalog orderDir, bool disablePagination, int? organizationId = null, OrganizationRequestTypeCatalog? type = null, string orderByColumn = null, bool calculateTotal = true)
        {
            var _logic = new Logic(LoggedInMemberId);
            OrganizationRequestSearchModel filters = new OrganizationRequestSearchModel();
            filters.OrganizationId = organizationId;
            filters.Type = type;
            SetPaginationProperties(filters, recordsPerPage, currentPage, orderDir, orderByColumn, disablePagination, calculateTotal);
            return await _logic.GetOrganizationRequests(filters);
        }
        [HttpGet]
        [Route("Get")]
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
        [Route("GetRequestStatus")]
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