using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.ViewModels.Campaign;
using BusinessLogic;
using Catalogs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.BriefModel;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampaignRegionController : BaseController
    {
        private readonly Logic _logic;

        public CampaignRegionController(Logic logic)
        {
            _logic = logic;
        }
        [HttpGet]
        [Route("GetPaginated")]
        public async Task<PaginatedResultModel<PaginatedEntityRegionModel>> GetPaginated(int recordsPerPage, int currentPage, PaginationOrderCatalog orderDir, bool disablePagination, int campaignId, string orderByColumn = null, bool calculateTotal = true)
        {

            EntityRegionSearchModel filters = new EntityRegionSearchModel();
            filters.EntityId = campaignId;
            filters.EntityType = EntityRegionTypeCatalog.Campaign;
            SetPaginationProperties(filters, recordsPerPage, currentPage, orderDir, orderByColumn, disablePagination, calculateTotal);
            return await _logic.GetEntityRegions(filters);
        }

        [HttpPost]
        [Route("Modify")]
        public async Task<bool> Modify(CampaignRegionViewModel model)
        {
            List<EntityRegionModel> entityRegions = new List<EntityRegionModel>();
            foreach (var region in model.Regions)
            {
                EntityRegionModel entityRegion = new EntityRegionModel
                {
                    Entity = new BaseBriefModel
                    {
                        Id = model.CampaignId
                    },
                    EntityType = EntityRegionTypeCatalog.Campaign,
                    Region = new RegionBriefModel
                    {
                        Id = region.Region.Id
                    },
                    RegionLevel = region.RegionLevel
                };
                entityRegions.Add(entityRegion);

            }
            return await _logic.ModifyCampaignRegions(model.CampaignId, entityRegions);
        }
    }
}