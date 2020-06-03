using System;
using System.Collections.Generic;
using System.Drawing;
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
    public class OrganizationController : BaseController
    {
        private readonly Logic _logic;

        public OrganizationController(Logic logic)
        {
            _logic = logic;
        }
        [HttpGet]
        [Route("Get/{id}")]
        public async Task<OrganizationModel> Get(int id)
        {
            return await _logic.GetOrganization(id);
        }
        [HttpGet]
        [Route("GetCategories/{id}")]
        public async Task<List<ItemBriefModel>> GetCategories(int id)
        {
            return await _logic.GetRootCategoriesByOrganization(id);
        }
        [HttpGet]
        [Route("GetPaginated")]
        public async Task<PaginatedResultModel<OrganizationModel>> GetPaginated(int recordsPerPage, int currentPage, PaginationOrderCatalog orderDir, bool disablePagination, string name = null, double? longitude = null, double? latitude = null, [FromQuery] List<int> rootCategories = null, RegionSearchTypeCatalog? searchType = null, [FromQuery] List<RegionLevelSearchModel> regions = null, RegionRadiusTypeCatalog? radiusType = null, float? radius = null, bool? fetchOwnedByMeOnly = null, string orderByColumn = null, bool calculateTotal = true)
        {
            OrganizationSearchModel filters = new OrganizationSearchModel();
            filters.Name = name;
            filters.Longitude = longitude ?? 0;
            filters.Latitude = latitude ?? 0;
            filters.Regions = regions ?? new List<RegionLevelSearchModel>();
            filters.Radius = radius;
            filters.RootCategories = rootCategories ?? new List<int>();
            filters.RadiusType = radiusType;
            filters.SearchType = searchType;
            filters.OwnedByMe = fetchOwnedByMeOnly ?? false;
            SetPaginationProperties(filters, recordsPerPage, currentPage, orderDir, orderByColumn, disablePagination, calculateTotal);
            return await _logic.GetOrganizations(filters);
        }
        [HttpGet]
        [Route("GetSingleOrganizationTree")]
        public async Task<IEnumerable<OrganizationModel>> GetSingleOrganizationTree(int id, DataStructureCatalog dataStructure)
        {
            bool getHierarichalData = (dataStructure == DataStructureCatalog.List ? false : true);
            return await _logic.GetSingleOrganizationHierarchy(id, getHierarichalData);
        }
        [HttpGet]
        [Route("GetPeripheralOrganizations")]
        public async Task<IEnumerable<OrganizationModel>> GetPeripheralOrganizations()
        {
            return await _logic.GetPeripheralOrganizations();
        }
        [HttpGet]
        [Route("GetRootOrganizations")]
        public async Task<IEnumerable<OrganizationModel>> GetRootOrganizations()
        {
            return await _logic.GetRootOrganizations();
        }
        [HttpGet]
        [Route("GetAllOrganizations")]
        public async Task<IEnumerable<OrganizationModel>> GetAllOrganizations(DataStructureCatalog dataStructure)
        {
            bool getHierarichalData = (dataStructure == DataStructureCatalog.List ? false : true);
            return await _logic.GetAllOrganizations(getHierarichalData);
        }
        [HttpPost]
        [Route("Create")]
        public async Task<int> Create(OrganizationModel model)
        {
            return await _logic.CreateOrganization(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<bool> Update(OrganizationModel model)
        {
            return await _logic.UpdateOrganization(model);
        }
        [HttpPost]
        [Route("CreateSingleOrganizationWithChildrens")]
        public async Task<bool> CreateSingleOrganizationWithChildrens(OrganizationModel model)
        {
            return await _logic.CreateSingleOrganizationWithChildrens(model);
        }
        [HttpPut]
        [Route("UpdateSingleOrganizationWithChildren")]
        public async Task<bool> UpdateSingleOrganizationWithChildren(OrganizationModel model)
        {
            return await _logic.UpdateSingleOrganizationWithChildren(model);
        }
        [HttpPost]
        [Route("CreateMultipleOrganizationsWithChildrens")]
        public async Task<bool> CreateMultipleOrganizationsWithChildrens(List<OrganizationModel> Organizations)
        {
            return await _logic.CreateMultipleOrganizationsWithChildrens(Organizations);
        }
        [HttpPut]
        [Route("UpdateMultipleOrganizationsWithChildrens")]
        public async Task<bool> UpdateMultipleOrganizationsWithChildrens(List<OrganizationModel> Organizations)
        {
            return await _logic.UpdateMultipleOrganizationsWithChildrens(Organizations);
        }
        [HttpDelete]
        [Route("DeleteOrganizationWithChildren")]
        public async Task<bool> DeleteOrganizationWithChildren(int id)
        {
            return await _logic.DeleteOrganization(id);
        }



    }
}