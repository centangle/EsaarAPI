using Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public partial class Logic
    {
        public async Task<int> CreateOrganizationItem(OrganizationItemModel model)
        {
            return await _dataAccess.CreateOrganizationItem(model);
        }
        public async Task<bool> UpdateOrganizationItem(OrganizationItemModel model)
        {
            return await _dataAccess.UpdateOrganizationItem(model);
        }
        public async Task<bool> CreateMultipleOrganizationItem(List<OrganizationItemModel> organizationItems)
        {
            return await _dataAccess.CreateMultipleOrganizationItem(organizationItems);
        }
        public async Task<bool> UpdateMultipleOrganizationItem(List<OrganizationItemModel> organizationItems)
        {
            return await _dataAccess.UpdateMultipleOrganizationItem(organizationItems);
        }
        public async Task<bool> DeleteOrganizationItems(List<int> ids)
        {
            return await _dataAccess.DeleteOrganizationItems(ids);
        }
        public async Task<OrganizationItemModel> GetOrganizationItem(int id)
        {
            return await _dataAccess.GetOrganizationItem(id);
        }
        public async Task<PaginatedResultModel<OrganizationItemPaginationModel>> GetOrganizationItems(OrganizationItemSearchModel filters)
        {
            return await _dataAccess.GetOrganizationItems(filters);
        }
    }
}
