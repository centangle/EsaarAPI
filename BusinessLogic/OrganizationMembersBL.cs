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
        public async Task<List<OrganizationMemberModel>> GetMemberRoleForOrganization(int organizationId, int? memberId)
        {
            return await _dataAccess.GetMemberRoleForOrganization(organizationId, memberId);
        }
        public async Task<int> RequestOrganizationMembership(OrganizationRequestModel model)
        {
            return await _dataAccess.RequestOrganizationMembership(model);
        }
        public async Task<bool> UpdateOrganizationMembershipRegions(OrganizationRequestModel model)
        {
            return await _dataAccess.UpdateOrganizationMembershipRegions(model);
        }
        public async Task<PaginatedResultModel<OrganizationMemberModel>> GetOrganizationMembers(OrganizationMemberSearchModel filters)
        {
            return await _dataAccess.GetOrganizationMembers(filters);
        }
        public async Task<List<OrganizationMemberModel>> GetOrganizationMembersForDD(OrganizationMemberSearchModel filters)
        {
            return await _dataAccess.GetOrganizationMembersForDD(filters);
        }
    }
}
