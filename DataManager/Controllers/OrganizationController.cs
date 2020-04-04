using BusinessLogic;
using Catalogs;
using Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;


namespace DataManager.Controllers
{
    [Authorize]
    public class OrganizationController:BaseController
    { 
        [HttpGet]
        public async Task<OrganizationModel> Get(int id)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.GetOrganization(id);
        }
        [HttpGet]
        public async Task<IEnumerable<OrganizationModel>> GetSingleOrganizationTree(int id, DataStructureCatalog dataStructure)
        {
            var _logic = new Logic(LoggedInMemberId);
            bool getHierarichalData = (dataStructure == DataStructureCatalog.List ? false : true);
            return await _logic.GetSingleOrganizationHierarchy(id, getHierarichalData);
        }

        [HttpGet]
        public async Task<IEnumerable<OrganizationModel>> GetPeripheralOrganizations()
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.GetPeripheralOrganizations();
        }

        [HttpGet]
        public async Task<IEnumerable<OrganizationModel>> GetRootOrganizations()
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.GetRootOrganizations();
        }

        [HttpGet]
        public async Task<IEnumerable<OrganizationModel>> GetAllOrganizations(DataStructureCatalog dataStructure)
        {
            var _logic = new Logic(LoggedInMemberId);
            bool getHierarichalData = (dataStructure == DataStructureCatalog.List ? false : true);
            return await _logic.GetAllOrganizations(getHierarichalData);
        }
        [HttpPost]
        public async Task<int> Create(OrganizationModel model)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.CreateOrganization(model);
        }
        [HttpPut]
        public async Task<bool> Update(OrganizationModel model)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.UpdateOrganization(model);
        }
        [HttpPost]
        public async Task<bool> CreateSingleOrganizationWithChildrens(OrganizationModel model)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.CreateSingleOrganizationWithChildrens(model);
        }
        [HttpPut]
        public async Task<bool> UpdateSingleOrganizationWithChildren(OrganizationModel model)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.UpdateSingleOrganizationWithChildren(model);
        }
        [HttpPost]
        public async Task<bool> CreateMultipleOrganizationsWithChildrens(List<OrganizationModel> Organizations)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.CreateMultipleOrganizationsWithChildrens(Organizations);
        }
        [HttpPut]
        public async Task<bool> UpdateMultipleOrganizationsWithChildrens(List<OrganizationModel> Organizations)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.UpdateMultipleOrganizationsWithChildrens(Organizations);
        }

        [HttpDelete]
        public async Task<bool> DeleteOrganizationWithChildren(int id)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.DeleteOrganization(id);
        }
    }
}
