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
    public class OrganizationRegionController : BaseController
    {
        [HttpGet]
        public async Task<PaginatedResultModel<PaginatedEntityRegionModel>> GetPaginated(int recordsPerPage, int currentPage, PaginationOrderCatalog orderDir, bool disablePagination, int organizationId, string orderByColumn = null, bool calculateTotal = true)
        {
            var _logic = new Logic(LoggedInMemberId);
            EntityRegionSearchModel filters = new EntityRegionSearchModel();
            filters.EntityId = organizationId;
            filters.EntityType = EntityRegionTypeCatalog.Organization;
            SetPaginationProperties(filters, recordsPerPage, currentPage, orderDir, orderByColumn, disablePagination, calculateTotal);
            return await _logic.GetEntityRegions(filters);
        }
        [HttpPost]
        public async Task<bool> Modify(int organizationId, List<EntityRegionModel> entityRegions)
        {
            var _logic = new Logic(LoggedInMemberId);
            foreach (var entityRegion in entityRegions)
            {
                entityRegion.Id = organizationId;
                entityRegion.EntityType = EntityRegionTypeCatalog.Organization;

            }
            return await _logic.ModifyMultipleEntityRegion(entityRegions, organizationId, null);
        }
    }
}
