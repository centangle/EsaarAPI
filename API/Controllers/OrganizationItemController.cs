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
        private readonly Logic _logic;

        public OrganizationItemController(Logic logic)
        {
            _logic = logic;
        }
        [HttpGet]
        [Route("Get")]
        public async Task<OrganizationItemModel> Get(int id)
        {
            
            return await _logic.GetOrganizationItem(id);
        }
        [HttpGet]
        [Route("GetPaginated")]
        public async Task<PaginatedResultModel<OrganizationItemPaginationModel>> GetPaginated(int recordsPerPage, int currentPage, PaginationOrderCatalog orderDir, bool disablePagination, int organizationId, SearchItemTypeCatalog itemType, int? campaignId = null, string itemName = null, string orderByColumn = null, bool calculateTotal = true)
        {
            
            OrganizationItemSearchModel filters = new OrganizationItemSearchModel();
            filters.ItemName = itemName;
            filters.CampaignId = campaignId;
            filters.OrganizationId = organizationId;
            filters.Type = itemType;
            SetPaginationProperties(filters, recordsPerPage, currentPage, orderDir, orderByColumn, disablePagination, calculateTotal);
            return await _logic.GetOrganizationItems(filters);
        }
        [HttpPost]
        [Route("Create")]
        public async Task<int> Create(OrganizationItemModel model)
        {
            
            return await _logic.CreateOrganizationItem(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<bool> Update(OrganizationItemModel model)
        {
            
            return await _logic.UpdateOrganizationItem(model);
        }
        [HttpPost]
        [Route("CreateMultipleOrganizationItem")]
        public async Task<bool> CreateMultipleOrganizationItem(List<OrganizationItemModel> items)
        {
            
            return await _logic.CreateMultipleOrganizationItem(items);
        }
        [HttpPut]
        [Route("UpdateMultipleOrganizationItem")]
        public async Task<bool> UpdateMultipleOrganizationItem(List<OrganizationItemModel> items)
        {
            
            return await _logic.UpdateMultipleOrganizationItem(items);
        }
        [HttpDelete]
        [Route("Delete")]
        public async Task<bool> Delete(int id)
        {
            
            return await _logic.DeleteOrganizationItems(new List<int> { id });
        }
        [HttpDelete]
        [Route("DeleteOrganizationItem")]
        public async Task<bool> DeleteOrganizationItem(int organizationId, int itemId)
        {
            
            return await _logic.DeleteOrganizationItem(organizationId, itemId);
        }
        [HttpDelete]
        [Route("DeleteMultipleOrganizationItem")]
        public async Task<bool> DeleteMultipleOrganizationItem(List<int> ids)
        {
            
            return await _logic.DeleteOrganizationItems(ids);
        }
    }
}