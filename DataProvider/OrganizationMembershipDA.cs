using Catalogs;
using Helpers;
using Models;
using System.Threading.Tasks;

namespace DataProvider
{
    public partial class DataAccess
    {
        public async Task<int> RequestOrganizationMembership(OrganizationRequestModel model)
        {
            if (model.Type == OrganizationRequestTypeCatalog.Member)
            {
                var memberModel = GetOrganizationMembershipModel(model);
                return await AddMemberToOrganization(memberModel);
            }
            else
            {
                return await AddOrganizationRequest(model);
            }
        }
        private async Task<int> AddMemberToOrganization(OrganizationMembershipModel model)
        {
            using (CharityEntities context = new CharityEntities())
            {
                var dbModel = SetOrganizationMember(new OrganizationMember(), model);
                context.OrganizationMembers.Add(dbModel);
                await context.SaveChangesAsync();
                model.Id = dbModel.Id;
                return model.Id;
            }
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
            dbModel.OrganizationId = model.Organization.Id;
            dbModel.MemberId = model.Member.Id;
            SetBaseProperties(dbModel, model);
            return dbModel;
        }
        private OrganizationMembershipModel GetOrganizationMembershipModel(OrganizationRequestModel model)
        {
            OrganizationMembershipModel membershipModel = new OrganizationMembershipModel();
            membershipModel.Organization = model.Organization;
            membershipModel.Member = model.Entity;
            switch (model.Type)
            {
                case OrganizationRequestTypeCatalog.Member:
                    membershipModel.Type = OrganizationMemberTypeCatalog.Member;
                    break;
                case OrganizationRequestTypeCatalog.Volunteer:
                    membershipModel.Type = OrganizationMemberTypeCatalog.Volunteer;
                    break;
                case OrganizationRequestTypeCatalog.Moderator:
                    membershipModel.Type = OrganizationMemberTypeCatalog.Moderator;
                    break;
            }
            return membershipModel;
        }
    }
}
