using Helpers;
using Models;
using System.Threading.Tasks;

namespace DataProvider
{
    public partial class DataAccess
    {
        public async Task<int> RequestMembership(OrganizationMembershipModel model)
        {
            return 0;
        }

        private OrganizationMember SetOrganizationMember(OrganizationMember dbModel, OrganizationMembershipModel model)
        {
            if (model.Organization == null || model.Organization.Id < 1)
            {
                throw new KnownException("Organization is required.");
            }
            if (model.Member == null || model.Member.Id < 1)
            {
                if (_loggedInMemberId == 0)
                    throw new KnownException("Member is required.");
                else
                {
                    model.Member = new Models.BriefModel.BaseBriefModel();
                    model.Member.Id = _loggedInMemberId;
                }
            }
            SetBaseProperties(dbModel, model);
            return dbModel;
        }
    }
}
