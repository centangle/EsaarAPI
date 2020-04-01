using Catalogs;
using Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DataManager.Controllers
{
    public class OrganizationController : BaseController
    {
        [HttpGet]
        public async Task<OrganizationModel> Get(int id)
        {
            return await logic.GetOrganization(id);
        }
        [HttpGet]
        public async Task<IEnumerable<OrganizationModel>> GetSingleOrganizationTree(int id, DataStructureCatalog dataStructure)
        {
            bool getHierarichalData = (dataStructure == DataStructureCatalog.List ? false : true);
            return await logic.GetSingleOrganizationHierarchy(id, getHierarichalData);
        }

        [HttpGet]
        public async Task<IEnumerable<OrganizationModel>> GetPeripheralOrganizations()
        {
            return await logic.GetPeripheralOrganizations();
        }

        [HttpGet]
        public async Task<IEnumerable<OrganizationModel>> GetRootOrganizations()
        {
            return await logic.GetRootOrganizations();
        }

        [HttpGet]
        public async Task<IEnumerable<OrganizationModel>> GetAllOrganizations(DataStructureCatalog dataStructure)
        {
            bool getHierarichalData = (dataStructure == DataStructureCatalog.List ? false : true);
            return await logic.GetAllOrganizations(getHierarichalData);
        }
        [HttpPost]
        public async Task<int> Create(OrganizationModel model)
        {
            return await logic.CreateOrganization(model);
        }
        [HttpPut]
        public async Task<bool> Update(OrganizationModel model)
        {
            return await logic.UpdateOrganization(model);
        }
        [HttpPost]
        public async Task<int> CreateSingleOrganizationWithChildrens(OrganizationModel model)
        {
            return await logic.CreateSingleOrganizationWithChildrens(model);
        }
        [HttpPut]
        public async Task<bool> UpdateSingleOrganizationWithChildren(OrganizationModel model)
        {
            return await logic.UpdateSingleOrganizationWithChildren(model);
        }
        [HttpPost]
        public async Task<bool> CreateMultipleOrganizationsWithChildrens(List<OrganizationModel> Organizations)
        {
            return await logic.CreateMultipleOrganizationsWithChildrens(Organizations);
        }
        [HttpPut]
        public async Task<bool> UpdateMultipleOrganizationsWithChildrens(List<OrganizationModel> Organizations)
        {
            return await logic.UpdateMultipleOrganizationsWithChildrens(Organizations);
        }

        [HttpDelete]
        public async Task<bool> DeleteOrganizationWithChildren(int id)
        {
            return await logic.DeleteOrganization(id);
        }
    }
}
