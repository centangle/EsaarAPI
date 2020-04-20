using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public partial class Logic
    {
        public async Task<int> CreateOrganizationAccount(OrganizationAccountModel model)
        {
            return await _dataAccess.CreateOrganizationAccount(model);
        }
        public async Task<bool> UpdateOrganizationAccount(OrganizationAccountModel model)
        {
            return await _dataAccess.UpdateOrganizationAccount(model);
        }
        public async Task<bool> DeleteOrganizationAccount(int id)
        {
            return await _dataAccess.DeleteOrganizationAccount(id);
        }
        public async Task<OrganizationAccountModel> GetOrganizationAccount(int id)
        {
            return await _dataAccess.GetOrganizationAccount(id);
        }
        public async Task<PaginatedResultModel<OrganizationAccountModel>> GetOrganizationAccounts(OrganizationAccountSearchModel filters)
        {
            return await _dataAccess.GetOrganizationAccounts(filters);
        }
    }
}
