using Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public partial class Logic
    {
        public async Task<int> CreateOrganization(OrganizationModel model)
        {
            return await DA.CreateOrganization(model);
        }
        public async Task<bool> UpdateOrganization(OrganizationModel model)
        {
            return await DA.UpdateOrganization(model);
        }
        public async Task<int> CreateSingleOrganizationWithChildrens(OrganizationModel model)
        {
            return await DA.CreateSingleOrganizationWithChildrens(model);
        }

        public async Task<bool> UpdateSingleOrganizationWithChildren(OrganizationModel model)
        {
            return await DA.UpdateSingleOrganizationWithChildren(model);
        }
        public async Task<bool> CreateMultipleOrganizationsWithChildrens(List<OrganizationModel> Organizations)
        {
            return await DA.CreateMultipleOrganizationsWithChildrens(Organizations);
        }

        public async Task<bool> UpdateMultipleOrganizationsWithChildrens(List<OrganizationModel> Organizations)
        {
            return await DA.UpdateMultipleOrganizationsWithChildrens(Organizations);
        }

        public async Task<OrganizationModel> GetOrganization(int id)
        {
            return await DA.GetOrganization(id);
        }
        public async Task<IEnumerable<OrganizationModel>> GetSingleOrganizationHierarchy(int id, bool getHierarchicalData)
        {
            return await DA.GetSingleTreeOrganization(id, getHierarchicalData);
        }
        public async Task<IEnumerable<OrganizationModel>> GetPeripheralOrganizations()
        {
            return await DA.GetPeripheralOrganizations();
        }
        public async Task<IEnumerable<OrganizationModel>> GetRootOrganizations()
        {
            return await DA.GetRootOrganizations();
        }
        public async Task<IEnumerable<OrganizationModel>> GetAllOrganizations(bool getHierarchicalData)
        {
            return await DA.GetAllOrganizations(getHierarchicalData);
        }
        public async Task<bool> DeleteOrganization(int id)
        {
            return await DA.DeleteOrganization(id);
        }
    }
}