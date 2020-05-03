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
    public class OrganizationItemController : BaseController
    {
        [HttpGet]
        public async Task<OrganizationItemModel> Get(int id)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.GetOrganizationItem(id);
        }
        [HttpGet]
        public async Task<PaginatedResultModel<OrganizationItemPaginationModel>> GetPaginated(int recordsPerPage, int currentPage, PaginationOrderCatalog orderDir, bool disablePagination, int organizationId, SearchItemTypeCatalog itemType, int? campaignId = null, string itemName = null, string orderByColumn = null, bool calculateTotal = true)
        {
            var _logic = new Logic(LoggedInMemberId);
            OrganizationItemSearchModel filters = new OrganizationItemSearchModel();
            filters.ItemName = itemName;
            filters.CampaignId = campaignId;
            filters.OrganizationId = organizationId;
            filters.Type = itemType;
            SetPaginationProperties(filters, recordsPerPage, currentPage, orderDir, orderByColumn, disablePagination, calculateTotal);
            return await _logic.GetOrganizationItems(filters);
        }
        [HttpPost]
        public async Task<int> Create(OrganizationItemModel model)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.CreateOrganizationItem(model);
        }
        [HttpPut]
        public async Task<bool> Update(OrganizationItemModel model)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.UpdateOrganizationItem(model);
        }
        [HttpPost]
        public async Task<bool> CreateMultipleOrganizationItem(List<OrganizationItemModel> items)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.CreateMultipleOrganizationItem(items);
        }
        [HttpPut]
        public async Task<bool> UpdateMultipleOrganizationItem(List<OrganizationItemModel> items)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.UpdateMultipleOrganizationItem(items);
        }
        [HttpDelete]
        public async Task<bool> Delete(int id)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.DeleteOrganizationItems(new List<int> { id });
        }
        [HttpDelete]
        public async Task<bool> DeleteOrganizationItem(int organizationId, int itemId)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.DeleteOrganizationItem(organizationId, itemId);
        }
        [HttpDelete]
        public async Task<bool> DeleteMultipleOrganizationItem(List<int> ids)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.DeleteOrganizationItems(ids);
        }
    }
}