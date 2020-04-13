using Catalogs;
using DataProvider.Helpers;
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
        public async Task<List<OrganizationMemberModel>> GetMemberRoleForOrganization(int organizationId, int? memberId)
        {
            using (CharityEntities context = new CharityEntities())
            {
                return await GetMemberRoleForOrganization(context, organizationId, memberId);
            }
        }
        public async Task<PaginatedResultModel<OrganizationMemberModel>> GetOrganizationMembers(OrganizationMemberSearchModel filters)
        {
            using (CharityEntities context = new CharityEntities())
            {
                var requestQueryable = (from om in context.OrganizationMembers
                                        join o in context.Organizations on om.OrganizationId equals o.Id
                                        join m in context.Members on om.MemberId equals m.Id
                                        where
                                        (filters.OrganizationId == null || o.Id == filters.OrganizationId)
                                        && (string.IsNullOrEmpty(filters.OrganizationName) || o.Name.Contains(filters.OrganizationName) || o.NativeName.Contains(filters.OrganizationName))
                                        && (filters.MemberId == null || m.Id == filters.MemberId)
                                        && (string.IsNullOrEmpty(filters.MemberName) || m.Name.Contains(filters.MemberName) || m.NativeName.Contains(filters.MemberName))
                                        && (filters.Role == null || om.Type == (int)filters.Role.Value)
                                        && om.IsDeleted == false
                                        select new OrganizationMemberModel
                                        {
                                            Id = om.Id,
                                            Organization = new BaseBriefModel()
                                            {
                                                Id = o.Id,
                                                Name = o.Name,
                                                NativeName = o.NativeName,
                                            },
                                            Member = new BaseBriefModel()
                                            {
                                                Id = m.Id,
                                                Name = m.Name,
                                                NativeName = m.NativeName,
                                            },
                                            Role = (OrganizationMemberRolesCatalog)om.Type,
                                            CreatedDate = om.CreatedDate
                                        }).AsQueryable();

                return await requestQueryable.Paginate(filters);
            }
        }
        public async Task<List<OrganizationMemberModel>> GetOrganizationMembersForDD(OrganizationMemberSearchModel filters)
        {
            using (CharityEntities context = new CharityEntities())
            {
                var organizationMember = (await GetMemberRoleForOrganization(context, filters.OrganizationId, _loggedInMemberId)).FirstOrDefault();
                if (IsOrganizationMemberModerator(organizationMember))
                {
                    return await (from om in context.OrganizationMembers
                                  join o in context.Organizations on om.OrganizationId equals o.Id
                                  join m in context.Members on om.MemberId equals m.Id
                                  where
                                  (o.Id == filters.OrganizationId)
                                  && (string.IsNullOrEmpty(filters.MemberName) || m.Name.Contains(filters.MemberName) || m.NativeName.Contains(filters.MemberName))
                                  && om.Type == (int)filters.Role.Value
                                  && om.IsDeleted == false
                                  select new OrganizationMemberModel
                                  {
                                      Id = om.Id,
                                      Organization = new BaseBriefModel()
                                      {
                                          Id = o.Id,
                                          Name = o.Name,
                                          NativeName = o.NativeName,
                                      },
                                      Member = new BaseBriefModel()
                                      {
                                          Id = m.Id,
                                          Name = m.Name,
                                          NativeName = m.NativeName,
                                      },
                                      Role = (OrganizationMemberRolesCatalog)om.Type,
                                      CreatedDate = om.CreatedDate
                                  }).OrderBy(x => x.Member.Name).Take(filters.RecordsPerPage).ToListAsync();
                }
                else
                {
                    return new List<OrganizationMemberModel>();
                }
            }
        }

        #region[Private Functions]
        private async Task<OrganizationMember> AddMemberToOrganization(CharityEntities context, OrganizationMemberModel model)
        {
            var dbModel = SetOrganizationMember(new OrganizationMember(), model);
            if (!(await IsAlreadyAMember(context, dbModel.OrganizationId, dbModel.MemberId, model.Role)))
            {
                context.OrganizationMembers.Add(dbModel);
            }
            return dbModel;

        }
        private OrganizationMember SetOrganizationMember(OrganizationMember dbModel, OrganizationMemberModel model)
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
        private OrganizationMemberModel GetOrganizationMembershipModel(OrganizationRequestModel model)
        {
            OrganizationMemberModel membershipModel = new OrganizationMemberModel();
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
        private async Task<List<OrganizationMemberModel>> GetMemberRoleForOrganization(CharityEntities context, int? organizationId, int? memberId, bool verifyActiveMember = true)
        {
            List<OrganizationMemberModel> orgMemberRoles = new List<OrganizationMemberModel>();
            var lstOrgMembRole = await (from o in context.Organizations
                                        join om in context.OrganizationMembers on o.Id equals om.OrganizationId
                                        join m in context.Members on om.MemberId equals m.Id
                                        where
                                        (organizationId == null || o.Id == organizationId)
                                        && (memberId == null || m.Id == memberId)
                                        && om.IsDeleted == false
                                        && (verifyActiveMember == false || om.IsActive == true)
                                        select new OrganizationMemberModel
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
                orgMemberRoles.Add(new OrganizationMemberModel()
                {
                    Organization = orgMemberRole.FirstOrDefault().Organization,
                    Member = orgMemberRole.FirstOrDefault().Member,
                    Roles = memberRoles,
                });
            }
            return orgMemberRoles;
        }
        private async Task<bool> IsOrganizationMembershipRequestAllowed(CharityEntities context, OrganizationRequestModel requestModel, OrganizationMemberModel membershipModel)
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
                    if (orgRequest.ModeratorId == _loggedInMemberId)
                    {
                        return true;
                    }
                    else
                    {
                        var organizationMember = (await GetMemberRoleForOrganization(context, orgRequest.OrganizationId, _loggedInMemberId)).FirstOrDefault();
                        if (IsOrganizationMemberOwner(organizationMember))
                        {
                            return true;
                        }
                    }
                }
                throw new KnownException("You are not authorized to perform this action");
            }
            throw new KnownException("Request has been deleted");
        }

        private bool IsOrganizationMemberModerator(OrganizationMemberModel organizationMember)
        {
            if (organizationMember != null &&
                        (
                            organizationMember.Roles.Contains(OrganizationMemberRolesCatalog.Owner)
                            ||
                            organizationMember.Roles.Contains(OrganizationMemberRolesCatalog.Moderator)
                        )
                    )
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool IsOrganizationMemberVolunteer(OrganizationMemberModel organizationMember)
        {
            if (organizationMember != null && organizationMember.Roles.Contains(OrganizationMemberRolesCatalog.Volunteer))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool IsOrganizationMemberOwner(OrganizationMemberModel organizationMember)
        {
            if (organizationMember != null && organizationMember.Roles.Contains(OrganizationMemberRolesCatalog.Owner))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion


    }
}
