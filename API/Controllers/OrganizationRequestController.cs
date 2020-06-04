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
        private readonly Logic _logic;

        public OrganizationRequestController(Logic logic)
        {
            _logic = logic;
        }

        [HttpPost]
        [Route("UpdateStatus")]
        public async Task<bool> UpdateStatus(OrganizationRequestThreadModel model)
        {
            return await _logic.UpdateOrganizationRequestStatus(model);
        }
        [HttpGet]
        [Route("GetPaginated")]
        public async Task<PaginatedResultModel<PaginatedOrganizationRequestModel>> GetPaginated(int recordsPerPage, int currentPage, PaginationOrderCatalog orderDir, bool disablePagination, int? organizationId = null, [FromQuery] List<OrganizationRequestTypeCatalog> types = null, [FromQuery] List<StatusCatalog> statuses = null, TimePeriodCatalog? timePeriod = null,
            DateTime? startDate = null, DateTime? endDate = null, string memberName = null
            , string orderByColumn = null, bool calculateTotal = true)
        {
            OrganizationRequestSearchModel filters = new OrganizationRequestSearchModel();
            filters.OrganizationId = organizationId;
            filters.MemberName = memberName;
            filters.Types = types ?? new List<OrganizationRequestTypeCatalog>();
            filters.Statuses = statuses??new List<StatusCatalog>();
            filters.TimePeriod = timePeriod;
            filters.StartDate = startDate;
            filters.EndDate = endDate;
            SetPaginationProperties(filters, recordsPerPage, currentPage, orderDir, orderByColumn, disablePagination, calculateTotal);
            return await _logic.GetOrganizationRequests(filters);
        }
        [HttpGet]
        [Route("Get")]
        public async Task<PaginatedOrganizationRequestModel> Get(int requestId)
        {

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
        [HttpGet]
        [Route("GetRequestType")]
        public Array GetRequestType()
        {
            return Enum.GetValues(typeof(OrganizationRequestTypeCatalog)).Cast<OrganizationRequestTypeCatalog>()
                .Where(x => x != OrganizationRequestTypeCatalog.Item
                && x != OrganizationRequestTypeCatalog.Region
                ).ToArray();
        }
        [HttpGet]
        [Route("GetTimePeriod")]
        public Array GetTimePeriod()
        {
            return Enum.GetValues(typeof(TimePeriodCatalog)).Cast<TimePeriodCatalog>().ToArray();
        }
    }
}