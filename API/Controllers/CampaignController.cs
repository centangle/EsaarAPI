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
using Models.BriefModel;

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
        [Route("Get/{id}")]
        public async Task<CampaignModel> Get(int id)
        {
            return await _logic.GetCampaign(id);
        }
        [HttpGet]
        [Route("GetCategories/{id}")]
        public async Task<List<ItemBriefModel>> GetCategories(int id)
        {
            return await _logic.GetRootCategoriesByCampaign(id);
        }
        [HttpGet]
        [Route("GetPaginated")]
        public async Task<PaginatedResultModel<CampaignModel>> GetPaginated(int recordsPerPage, int currentPage, PaginationOrderCatalog orderDir, bool disablePagination, int? organizationId = null, int? eventId = null, string name = null, double? longitude = null, double? latitude = null, [FromQuery] List<int> rootCategories = null, RegionSearchTypeCatalog? searchType = null, [FromQuery] List<RegionLevelSearchModel> regions = null, RegionRadiusTypeCatalog? radiusType = null, float? radius = null, string orderByColumn = null, bool calculateTotal = true)
        {

            CampaignSearchModel filters = new CampaignSearchModel();
            filters.Name = name;
            filters.Longitude = longitude ?? 0;
            filters.Latitude = latitude ?? 0;
            filters.Regions = regions ?? new List<RegionLevelSearchModel>();
            filters.Radius = radius;
            filters.RootCategories = rootCategories ?? new List<int>();
            filters.RadiusType = radiusType;
            filters.SearchType = searchType;
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