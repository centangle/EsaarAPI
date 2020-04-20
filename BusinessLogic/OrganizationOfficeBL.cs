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
        public async Task<int> CreateOrganizationOffice(OrganizationOfficeModel model)
        {
            return await _dataAccess.CreateOrganizationOffice(model);
        }
        public async Task<bool> UpdateOrganizationOffice(OrganizationOfficeModel model)
        {
            return await _dataAccess.UpdateOrganizationOffice(model);
        }
        public async Task<bool> DeleteOrganizationOffice(int id)
        {
            return await _dataAccess.DeleteOrganizationOffice(id);
        }
        public async Task<OrganizationOfficeModel> GetOrganizationOffice(int id)
        {
            return await _dataAccess.GetOrganizationOffice(id);
        }
        public async Task<PaginatedResultModel<OrganizationOfficeModel>> GetOrganizationOffices(OrganizationOfficeSearchModel filters)
        {
            return await _dataAccess.GetOrganizationOffices(filters);
        }
    }
}
