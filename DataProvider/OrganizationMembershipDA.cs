using Catalogs;
using Helpers;
using Models;
using Models.BriefModel;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace DataProvider
{
    public partial class DataAccess
    {
        public async Task<int> RequestOrganizationMembership(OrganizationRequestModel model)
        {
            using (CharityEntities context = new CharityEntities())
            {
                var memberModel = GetOrganizationMembershipModel(model);
                if (model.Type == OrganizationRequestTypeCatalog.Member)
                {
                    var dbModel = await AddMemberToOrganization(context, memberModel);
                    await context.SaveChangesAsync();
                    model.Id = dbModel.Id;
                    return model.Id;
                }
                else
                {
                    if (await IsOrganizationMembershipRequestAllowed(context, model, memberModel))
                    {
                        return await AddOrganizationRequest(context, model);
                    }
                    return 0;
                }
            }
        }
        public async Task<List<OrganizationMembershipModel>> GetMemberRoleForOrganization(int organizationId, int? memberId)
        {
            using (CharityEntities context = new CharityEntities())
            {
                return await GetMemberRoleForOrganization(context, organizationId, memberId);
            }
        }
        private async Task<OrganizationMember> AddMemberToOrganization(CharityEntities context, OrganizationMembershipModel model)
        {
            var dbModel = SetOrganizationMember(new OrganizationMember(), model);
            if (!(await IsAlreadyAMember(context, dbModel.OrganizationId, dbModel.MemberId, model.Role)))
            {
                context.OrganizationMembers.Add(dbModel);
            }
            return dbModel;

        }
        private OrganizationMember SetOrganizationMember(OrganizationMember dbModel, OrganizationMembershipModel model)
        {
            if (model.Organization == null || model.Organization.Id < 1)
            {
                throw new KnownException("Organization is required.");
            }
            model.Member = SetEntityId(model.Member, "Member is required.");
            dbModel.OrganizationId = model.Organization.Id;
            dbModel.MemberId = model.Member.Id;
            dbModel.Type = (int)model.Role;
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
                    membershipModel.Role = OrganizationMemberRolesCatalog.Member;
                    break;
                case OrganizationRequestTypeCatalog.Volunteer:
                    membershipModel.Role = OrganizationMemberRolesCatalog.Volunteer;
                    break;
                case OrganizationRequestTypeCatalog.Moderator:
                    membershipModel.Role = OrganizationMemberRolesCatalog.Moderator;
                    break;
                case OrganizationRequestTypeCatalog.Owner:
                    membershipModel.Role = OrganizationMemberRolesCatalog.Owner;
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
                await AddMemberToOrganization(context, memberModel);
            }
        }
        private async Task<List<OrganizationMembershipModel>> GetMemberRoleForOrganization(CharityEntities context, int? organizationId, int? memberId, bool verifyActiveMember = true)
        {
            List<OrganizationMembershipModel> orgMemberRoles = new List<OrganizationMembershipModel>();
            var lstOrgMembRole = await (from o in context.Organizations
                                        join om in context.OrganizationMembers on o.Id equals om.OrganizationId
                                        join m in context.Members on om.MemberId equals m.Id
                                        where
                                        (organizationId == null || o.Id == organizationId)
                                        && (memberId == null || m.Id == memberId)
                                        && om.IsDeleted == false
                                        && (verifyActiveMember == false || om.IsActive == true)
                                        select new OrganizationMembershipModel
                                        {
                                            Id = om.Id,
                                            Organization = new BaseBriefModel
                                            {
                                                Id = o.Id,
                                                Name = o.Name,
                                                NativeName = o.NativeName,
                                            },
                                            Member = new BaseBriefModel
                                            {
                                                Id = m.Id,
                                                Name = m.Name,
                                                NativeName = m.NativeName,
                                            },
                                            Role = (OrganizationMemberRolesCatalog)om.Type,
                                        }).GroupBy(x => x.Organization.Id).ToListAsync();
            foreach (var orgMemberRole in lstOrgMembRole)
            {
                var memberRoles = orgMemberRole.Select(x => x.Role).ToList();
                orgMemberRoles.Add(new OrganizationMembershipModel()
                {
                    Organization = orgMemberRole.FirstOrDefault().Organization,
                    Member = orgMemberRole.FirstOrDefault().Member,
                    Roles = memberRoles,
                });
            }
            return orgMemberRoles;
        }
        private async Task<bool> IsOrganizationMembershipRequestAllowed(CharityEntities context, OrganizationRequestModel requestModel, OrganizationMembershipModel membershipModel)
        {
            var sameTypeOfRequestInProcess = (await context.OrganizationRequests.Where(
                                                                x =>
                                                                x.OrganizationId == requestModel.Organization.Id
                                                                && x.EntityId == _loggedInMemberId
                                                                && x.EntityType == (int)requestModel.EntityType
                                                                && x.Type == (int)requestModel.Type
                                                                && x.Status != (int)StatusCatalog.Approved
                                                                && x.Status != (int)StatusCatalog.Rejected
                                                                && x.IsDeleted == false).CountAsync()) > 0;

            if (sameTypeOfRequestInProcess)
            {
                throw new KnownException($"You already have a request in process for {membershipModel.Role}");
            }
            if (!(await IsAlreadyAMember(context, membershipModel.Organization.Id, _loggedInMemberId, membershipModel.Role)))
                return true;
            else
                return false;
        }
        private async Task<bool> IsAlreadyAMember(CharityEntities context, int organizationId, int memberId, OrganizationMemberRolesCatalog role)
        {
            var memberOrganization = (await GetMemberRoleForOrganization(context, organizationId, memberId, false)).FirstOrDefault();
            if (memberOrganization != null && memberOrganization.Roles.Contains(role))
            {
                throw new KnownException($"This user is already {role} for this organization.");
            }
            return false;
        }
        private async Task<bool> IsOrganizationRequestThreadAccessible(CharityEntities context, RequestThreadModel requestModel)
        {
            var orgRequest = await context.OrganizationRequests.Where(x => x.Id == requestModel.Entity.Id && x.IsDeleted == false).FirstOrDefaultAsync();
            if (orgRequest != null)
            {
                if (orgRequest.CreatedBy == _loggedInMemberId)
                {
                    return true;
                }
                else
                {
                    var organizationMember = (await GetMemberRoleForOrganization(context, orgRequest.OrganizationId, _loggedInMemberId)).FirstOrDefault();
                    if (organizationMember != null &&
                      (organizationMember.Roles.Contains(OrganizationMemberRolesCatalog.Moderator)
                      || organizationMember.Roles.Contains(OrganizationMemberRolesCatalog.Owner)))
                    {
                        return true;
                    }
                }
                throw new KnownException("You are not authorized to perform this action");
            }
            throw new KnownException("Request has been deleted");
        }

    }
}
