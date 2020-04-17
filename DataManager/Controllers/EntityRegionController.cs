using BusinessLogic;
using Catalogs;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace DataManager.Controllers
{
    public class EntityRegionController : BaseController
    {
        [HttpGet]
        public async Task<PaginatedResultModel<PaginatedEntityRegionModel>> GetPaginated(int recordsPerPage, int currentPage, PaginationOrderCatalog orderDir, bool disablePagination, int entityId, EntityRegionTypeCatalog entityType, string orderByColumn = null, bool calculateTotal = true)
        {
            var _logic = new Logic(LoggedInMemberId);
            EntityRegionSearchModel filters = new EntityRegionSearchModel();
            filters.EntityId = entityId;
            filters.EntityType = entityType;
            SetPaginationProperties(filters, recordsPerPage, currentPage, orderDir, orderByColumn, disablePagination, calculateTotal);
            return await _logic.GetEntityRegions(filters);
        }
        [HttpPost]
        public async Task<bool> Modify(List<EntityRegionModel> entityRegions)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.ModifyMultipleEntityRegion(entityRegions);
        }
    }
}
