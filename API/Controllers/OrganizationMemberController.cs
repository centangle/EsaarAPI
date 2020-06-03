using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic;
using Catalogs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrganizationMemberController : BaseController
    {
        private readonly Logic _logic;

        public OrganizationMemberController(Logic logic)
        {
            _logic = logic;
        }
        [HttpGet]
        [Route("GetPaginated")]
        public async Task<PaginatedResultModel<OrganizationMemberModel>> GetPaginated(int recordsPerPage, int currentPage, PaginationOrderCatalog orderDir, bool disablePagination, int? organizationId = null, string organizationName = null, int? memberId = null, string memberName = null, OrganizationMemberRolesCatalog? type = null, string orderByColumn = null, bool calculateTotal = true)
        {
            
            OrganizationMemberSearchModel filters = new OrganizationMemberSearchModel();
            filters.OrganizationId = organizationId;
            filters.OrganizationName = organizationName;
            filters.MemberId = memberId;
            filters.MemberName = memberName;
            filters.Role = type;
            SetPaginationProperties(filters, recordsPerPage, currentPage, orderDir, orderByColumn, disablePagination, calculateTotal);
            return await _logic.GetOrganizationMembers(filters);
        }
        [HttpGet]
        [Route("GetOrganizationMembersForDD")]
        public async Task<PaginatedResultModel<OrganizationMemberModel>> GetOrganizationMembersForDD(int numberOfRecords, int organizationId, OrganizationMemberRolesCatalog type, string memberName = null)
        {
            OrganizationMemberSearchModel filters = new OrganizationMemberSearchModel();
            filters.OrganizationId = organizationId;
            filters.MemberName = memberName;
            filters.Role = type;
            filters.RecordsPerPage = numberOfRecords;
            return await _logic.GetOrganizationMembers(filters);
        }
        [HttpGet]
        [Route("GetMemberRole")]
        public async Task<List<OrganizationMemberRolesCatalog>> GetMemberRole(int organizationId)
        {
            
            var result = (await _logic.GetMemberRoleForOrganization(organizationId, LoggedInMemberId)).FirstOrDefault();
            if (result != null)
                return result.Roles;
            return new List<OrganizationMemberRolesCatalog>();
        }
        [HttpPost]
        [Route("RequestMembership")]
        public async Task<int> RequestMembership(OrganizationRequestModel model)
        {
            
            return await _logic.RequestOrganizationMembership(model);
        }
        [HttpPost]
        [Route("UpdateOrganizationMembershipRegions")]
        public async Task<bool> UpdateOrganizationMembershipRegions(OrganizationRequestModel model)
        {
            
            return await _logic.UpdateOrganizationMembershipRegions(model);
        }
    }
}