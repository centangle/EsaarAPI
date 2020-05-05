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
    public class OrganizationRegionController : BaseController
    {
        private readonly Logic _logic;

        public OrganizationRegionController(Logic logic)
        {
            _logic = logic;
        }
        [HttpGet]
        [Route("GetPaginated")]
        public async Task<PaginatedResultModel<PaginatedEntityRegionModel>> GetPaginated(int recordsPerPage, int currentPage, PaginationOrderCatalog orderDir, bool disablePagination, int organizationId, string orderByColumn = null, bool calculateTotal = true)
        {
            
            EntityRegionSearchModel filters = new EntityRegionSearchModel();
            filters.EntityId = organizationId;
            filters.EntityType = EntityRegionTypeCatalog.Organization;
            SetPaginationProperties(filters, recordsPerPage, currentPage, orderDir, orderByColumn, disablePagination, calculateTotal);
            return await _logic.GetEntityRegions(filters);
        }

        [HttpPost]
        [Route("Modify")]
        public async Task<bool> Modify(int organizationId, List<EntityRegionModel> entityRegions)
        {
            
            foreach (var entityRegion in entityRegions)
            {
                entityRegion.Entity.Id = organizationId;
                entityRegion.EntityType = EntityRegionTypeCatalog.Organization;

            }
            return await _logic.ModifyMultipleEntityRegion(entityRegions, organizationId, null);
        }

        [HttpGet]
        [Route("Levels")]
        public async Task<List<BaseBriefModel>> Levels(int organizationId)
        {
            
            return await _logic.GetOrganizationRegionAllowedLevels(organizationId);
        }
    }
}