using Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public partial class Logic
    {
        public async Task<int> CreateOrganization(OrganizationModel model)
        {
            return await _dataAccess.CreateOrganization(model);
        }
        public async Task<bool> UpdateOrganization(OrganizationModel model)
        {
            return await _dataAccess.UpdateOrganization(model);
        }
        public async Task<bool> CreateSingleOrganizationWithChildrens(OrganizationModel model)
        {
            return await _dataAccess.CreateSingleOrganizationWithChildrens(model);
        }

        public async Task<bool> UpdateSingleOrganizationWithChildren(OrganizationModel model)
        {
            return await _dataAccess.UpdateSingleOrganizationWithChildren(model);
        }
        public async Task<bool> CreateMultipleOrganizationsWithChildrens(List<OrganizationModel> Organizations)
        {
            return await _dataAccess.CreateMultipleOrganizationsWithChildrens(Organizations);
        }

        public async Task<bool> UpdateMultipleOrganizationsWithChildrens(List<OrganizationModel> Organizations)
        {
            return await _dataAccess.UpdateMultipleOrganizationsWithChildrens(Organizations);
        }

        public async Task<OrganizationModel> GetOrganization(int id)
        {
            return await _dataAccess.GetOrganization(id);
        }
        public async Task<PaginatedResultModel<OrganizationModel>> GetOrganizations(OrganizationSearchModel filters)
        {
            return await _dataAccess.GetOrganizations(filters);
        }
        public async Task<IEnumerable<OrganizationModel>> GetSingleOrganizationHierarchy(int id, bool getHierarchicalData)
        {
            return await _dataAccess.GetSingleTreeOrganization(id, getHierarchicalData);
        }
        public async Task<IEnumerable<OrganizationModel>> GetPeripheralOrganizations()
        {
            return await _dataAccess.GetPeripheralOrganizations();
        }
        public async Task<IEnumerable<OrganizationModel>> GetRootOrganizations()
        {
            return await _dataAccess.GetRootOrganizations();
        }
        public async Task<IEnumerable<OrganizationModel>> GetAllOrganizations(bool getHierarchicalData)
        {
            return await _dataAccess.GetAllOrganizations(getHierarchicalData);
        }
        public async Task<bool> DeleteOrganization(int id)
        {
            return await _dataAccess.DeleteOrganization(id);
        }
    }
}