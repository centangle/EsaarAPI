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
    public class OrganizationController : BaseController
    {
        [HttpGet]
        [Route("Get")]
        public async Task<OrganizationModel> Get(int id)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.GetOrganization(id);
        }
        [HttpGet]
        [Route("GetCategories")]
        public async Task<List<ItemBriefModel>> GetCategories(int id)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.GetOrganizationCategories(id);
        }
        [HttpGet]
        [Route("GetPaginated")]
        public async Task<PaginatedResultModel<OrganizationModel>> GetPaginated(int recordsPerPage, int currentPage, PaginationOrderCatalog orderDir, bool disablePagination, string name = null, string orderByColumn = null, bool calculateTotal = true)
        {
            var _logic = new Logic(LoggedInMemberId);
            OrganizationSearchModel filters = new OrganizationSearchModel();
            filters.Name = name;
            SetPaginationProperties(filters, recordsPerPage, currentPage, orderDir, orderByColumn, disablePagination, calculateTotal);
            return await _logic.GetOrganizations(filters);
        }
        [HttpGet]
        [Route("GetSingleOrganizationTree")]
        public async Task<IEnumerable<OrganizationModel>> GetSingleOrganizationTree(int id, DataStructureCatalog dataStructure)
        {
            var _logic = new Logic(LoggedInMemberId);
            bool getHierarichalData = (dataStructure == DataStructureCatalog.List ? false : true);
            return await _logic.GetSingleOrganizationHierarchy(id, getHierarichalData);
        }
        [HttpGet]
        [Route("GetPeripheralOrganizations")]
        public async Task<IEnumerable<OrganizationModel>> GetPeripheralOrganizations()
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.GetPeripheralOrganizations();
        }
        [HttpGet]
        [Route("GetRootOrganizations")]
        public async Task<IEnumerable<OrganizationModel>> GetRootOrganizations()
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.GetRootOrganizations();
        }
        [HttpGet]
        [Route("GetAllOrganizations")]
        public async Task<IEnumerable<OrganizationModel>> GetAllOrganizations(DataStructureCatalog dataStructure)
        {
            var _logic = new Logic(LoggedInMemberId);
            bool getHierarichalData = (dataStructure == DataStructureCatalog.List ? false : true);
            return await _logic.GetAllOrganizations(getHierarichalData);
        }
        [HttpPost]
        [Route("Create")]
        public async Task<int> Create(OrganizationModel model)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.CreateOrganization(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<bool> Update(OrganizationModel model)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.UpdateOrganization(model);
        }
        [HttpPost]
        [Route("CreateSingleOrganizationWithChildrens")]
        public async Task<bool> CreateSingleOrganizationWithChildrens(OrganizationModel model)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.CreateSingleOrganizationWithChildrens(model);
        }
        [HttpPut]
        [Route("UpdateSingleOrganizationWithChildren")]
        public async Task<bool> UpdateSingleOrganizationWithChildren(OrganizationModel model)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.UpdateSingleOrganizationWithChildren(model);
        }
        [HttpPost]
        [Route("CreateMultipleOrganizationsWithChildrens")]
        public async Task<bool> CreateMultipleOrganizationsWithChildrens(List<OrganizationModel> Organizations)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.CreateMultipleOrganizationsWithChildrens(Organizations);
        }
        [HttpPut]
        [Route("UpdateMultipleOrganizationsWithChildrens")]
        public async Task<bool> UpdateMultipleOrganizationsWithChildrens(List<OrganizationModel> Organizations)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.UpdateMultipleOrganizationsWithChildrens(Organizations);
        }
        [HttpDelete]
        [Route("DeleteOrganizationWithChildren")]
        public async Task<bool> DeleteOrganizationWithChildren(int id)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.DeleteOrganization(id);
        }
    }
}