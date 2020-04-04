using Catalogs;
using Helpers;
using Models;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DataProvider
{
    public partial class DataAccess
    {
        public async Task<int> RequestOrganizationMembership(OrganizationRequestModel model)
        {
            using (CharityEntities context = new CharityEntities())
            {
                if (model.Type == OrganizationRequestTypeCatalog.Member)
                {
                    var memberModel = GetOrganizationMembershipModel(model);
                    var dbModel = AddMemberToOrganization(context, memberModel);
                    await context.SaveChangesAsync();
                    model.Id = dbModel.Id;
                    return model.Id;
                }
                else
                {
                    return await AddOrganizationRequest(context, model);
                }
            }
        }
        private OrganizationMember AddMemberToOrganization(CharityEntities context, OrganizationMembershipModel model)
        {
            var dbModel = SetOrganizationMember(new OrganizationMember(), model);
            context.OrganizationMembers.Add(dbModel);
            return dbModel;

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
            dbModel.Type = (int)model.Type;
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

        private async Task AddOrganizationMemberForRequest(CharityEntities context, RequestThreadModel requestModel)
        {
            var requestDB = await context.OrganizationRequests.Where(x => x.Id == requestModel.Entity.Id).FirstOrDefaultAsync();
            if (requestDB != null)
            {
                OrganizationRequestModel model = new OrganizationRequestModel();
                model.Organization.Id = requestDB.OrganizationId;
                model.Entity.Id = requestDB.EntityId;
                model.Type = (OrganizationRequestTypeCatalog)requestDB.Type;
                var memberModel = GetOrganizationMembershipModel(model);
                AddMemberToOrganization(context, memberModel);
            }
        }

    }
}
