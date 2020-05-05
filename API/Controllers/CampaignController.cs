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
    public class CampaignController : BaseController
    {
        private readonly Logic _logic;

        public CampaignController(Logic logic)
        {
            _logic = logic;
        }
        [HttpGet]
        public async Task<CampaignModel> Get(int id)
        {
            
            return await _logic.GetCampaign(id);
        }
        [HttpGet]
        [Route("GetPaginated")]
        public async Task<PaginatedResultModel<CampaignModel>> GetPaginated(int recordsPerPage, int currentPage, PaginationOrderCatalog orderDir, bool disablePagination, int? organizationId = null, int? eventId = null, string name = null, string orderByColumn = null, bool calculateTotal = true)
        {
            
            CampaignSearchModel filters = new CampaignSearchModel();
            filters.Name = name;
            filters.OrganizationId = organizationId;
            filters.EventId = eventId;
            SetPaginationProperties(filters, recordsPerPage, currentPage, orderDir, orderByColumn, disablePagination, calculateTotal);
            return await _logic.GetCampaigns(filters);
        }

        [HttpPost]
        [Route("Create")]
        public async Task<int> Create(CampaignModel model)
        {
            
            return await _logic.CreateCampaign(model);
        }

        [HttpPost]
        [Route("Update")]
        public async Task<bool> Update(CampaignModel model)
        {
            
            return await _logic.UpdateCampaign(model);
        }

        [HttpDelete]
        [Route("Delete")]
        public async Task<bool> Delete(int id)
        {
            
            return await _logic.DeleteCampaign(id);
        }
    }
}