using Catalogs;
using EntityProvider.Helpers;
using Helpers;
using Models;
using Models.BriefModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EntityProvider.DbModels;
using Microsoft.EntityFrameworkCore;

namespace EntityProvider
{
    public partial class DataAccess
    {
        public async Task<int> RequestOrganizationMembership(OrganizationRequestModel model)
        {
            var memberModel = GetOrganizationMembershipModel(model);
            if (model.Type == OrganizationRequestTypeCatalog.Member)
            {
                var dbModel = await AddMemberToOrganization(_context, memberModel);
                await _context.SaveChangesAsync();
                model.Id = dbModel.Id;
                return model.Id;
            }
            else
            {
                if (await IsOrganizationMembershipRequestAllowed(_context, model, memberModel))
                {
                    if (model.Regions == null || model.Regions.Count == 0)
                    {
                        throw new KnownException("Regions must be specified");
                    }
                    using (var transaction = _context.Database.BeginTransaction())
                    {
                        try
                        {
                            var organizationRequestId = await AddOrganizationRequest(_context, model);
                            if (model.Type == OrganizationRequestTypeCatalog.Volunteer || model.Type == OrganizationRequestTypeCatalog.Moderator)
                            {
                                foreach (var entityRegion in model.Regions)
                                {
                                    entityRegion.Entity.Id = model.Entity.Id;
                                    entityRegion.EntityType = EntityRegionTypeCatalog.OrganizationMember;
                                    entityRegion.RequestId = organizationRequestId;
                                    entityRegion.RequestType = EntityRegionRequestTypeCatalog.OrganizationMember;
                                }
                                await ModifyEntityRegions(_context, model.Regions, model.Organization.Id, organizationRequestId, true);
                            }
                            transaction.Commit();
                            return organizationRequestId;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw ex;
                        }
                    }

                }
                return 0;
            }
        }
        public async Task<bool> UpdateOrganizationMembershipRegions(OrganizationRequestModel model)
        {
            var memberModel = GetOrganizationMembershipModel(model);
            var organizationRequest = await _context.OrganizationRequests.Where(
                                                            x =>
                                                            x.OrganizationId == model.Organization.Id
                                                            && x.EntityType == (int)model.EntityType
                                                            && x.Type == (int)model.Type
                                                            && x.IsDeleted == false).FirstOrDefaultAsync();
            if (organizationRequest != null)
            {
                if (organizationRequest.Status == (int)StatusCatalog.Approved)
                {
                    throw new KnownException("This request is already approved by moderator. You cannot change this request anymore.");
                }
                else if (organizationRequest.Status == (int)StatusCatalog.Rejected)
                {
                    throw new KnownException("This request is rejected by moderator. You cannot change this request anymore.");
                }
                else
                {
                    using (var transaction = _context.Database.BeginTransaction())
                    {
                        try
                        {
                            if (model.Regions == null || model.Regions.Count == 0)
                            {
                                throw new KnownException("Regions must be specified");
                            }
                            foreach (var entityRegion in model.Regions)
                            {
                                if (model.Entity == null || model.Entity.Id == 0)
                                {
                                    model.Entity = new BaseBriefModel()
                                    {
                                        Id = _loggedInMemberId
                                    };
                                }
                                entityRegion.Entity.Id = model.Entity.Id;
                                entityRegion.EntityType = EntityRegionTypeCatalog.OrganizationMember;
                                entityRegion.RequestId = organizationRequest.Id;
                                entityRegion.RequestType = EntityRegionRequestTypeCatalog.OrganizationMember;
                            }
                            await ModifyEntityRegions(_context, model.Regions, organizationRequest.OrganizationId, organizationRequest.Id, true);
                            var requestThreadModel = GetRequestThreadModelForOrganization(organizationRequest.Id, (StatusCatalog)organizationRequest.Status, "Regions has been updated");
                            await AddRequestThread(_context, requestThreadModel);
                            transaction.Commit();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw ex;
                        }
                    }
                }
            }
            throw new KnownException("Could not find Request with specified Id");
        }
        public async Task<List<OrganizationMemberModel>> GetMemberRoleForOrganization(int organizationId, int? memberId)
        {
            return await GetMemberRoleForOrganization(_context, organizationId, memberId);
        }
        public async Task<PaginatedResultModel<OrganizationMemberModel>> GetOrganizationMembers(OrganizationMemberSearchModel filters)
        {
            var requestQueryable = (from om in _context.OrganizationMembers
                                    join o in _context.Organizations on om.OrganizationId equals o.Id
                                    join m in _context.Members on om.MemberId equals m.Id
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
        public async Task<List<OrganizationMemberModel>> GetOrganizationMembersForDD(OrganizationMemberSearchModel filters)
        {
            var memberOrgRoles = (await GetMemberRoleForOrganization(_context, filters.OrganizationId, _loggedInMemberId)).FirstOrDefault();
            if (IsOrganizationMemberModerator(memberOrgRoles))
            {
                return await (from om in _context.OrganizationMembers
                              join o in _context.Organizations on om.OrganizationId equals o.Id
                              join m in _context.Members on om.MemberId equals m.Id
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

        #region[Private Functions]
        private async Task<OrganizationMember> AddMemberToOrganization(CharityContext _context, OrganizationMemberModel model)
        {
            var dbModel = SetOrganizationMember(new OrganizationMember(), model);
            if (!(await IsAlreadyAMember(_context, dbModel.OrganizationId, dbModel.MemberId, model.Role)))
            {
                _context.OrganizationMembers.Add(dbModel);
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
            SetAndValidateBaseProperties(dbModel, model);
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
        private async Task<int> AddOrganizationMemberForRequest(CharityContext _context, RequestThreadModel requestModel)
        {
            var requestDB = await _context.OrganizationRequests.Where(x => x.Id == requestModel.Entity.Id).FirstOrDefaultAsync();
            if (requestDB != null)
            {
                OrganizationRequestModel model = new OrganizationRequestModel();
                model.Organization.Id = requestDB.OrganizationId;
                model.Entity.Id = requestDB.EntityId;
                model.Type = (OrganizationRequestTypeCatalog)requestDB.Type;
                var memberModel = GetOrganizationMembershipModel(model);
                await AddMemberToOrganization(_context, memberModel);
                return await _context.SaveChangesAsync();
            }
            return 0;
        }
        private async Task<List<OrganizationMemberModel>> GetMemberRoleForOrganization(CharityContext _context, int? organizationId, int? memberId, bool verifyActiveMember = true)
        {
            List<OrganizationMemberModel> orgMemberRoles = new List<OrganizationMemberModel>();
            var lstOrgMembRole = (await (from o in _context.Organizations
                                         join om in _context.OrganizationMembers on o.Id equals om.OrganizationId
                                         join m in _context.Members on om.MemberId equals m.Id
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
                                         }).ToListAsync()).GroupBy(x => x.Organization.Id);
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
        private async Task<List<OrganizationMemberRegionModel>> GetMemberRegionsForOrganization(CharityContext _context, int? organizationId, int? memberId, OrganizationMemberRolesCatalog? roleType = null, bool verifyActiveMember = true)
        {
            List<OrganizationMemberRegionModel> orgMemberRegions = new List<OrganizationMemberRegionModel>();
            var lstOrgMembRegion = (await (from o in _context.Organizations
                                           join om in _context.OrganizationMembers on o.Id equals om.OrganizationId
                                           join m in _context.Members on om.MemberId equals m.Id
                                           join er in _context.EntityRegions on om.Id equals er.EntityId
                                           //join c in _context.Countries on er.CountryId equals c.Id into lc
                                           //from c in lc.DefaultIfEmpty()
                                           //join s in _context.States on er.StateId equals s.Id into ls
                                           //from s in ls.DefaultIfEmpty()
                                           //join d in _context.Districts on er.DistrictId equals d.Id into ld
                                           //from d in ld.DefaultIfEmpty()
                                           //join t in _context.Tehsils on er.TehsilId equals t.Id into lt
                                           //from t in lt.DefaultIfEmpty()
                                           //join uc in _context.UnionCouncils on er.UnionCouncilId equals uc.Id into luc
                                           //from uc in luc.DefaultIfEmpty()
                                           where
                                           (organizationId == null || o.Id == organizationId)
                                           && (memberId == null || m.Id == memberId)
                                           && om.IsDeleted == false
                                           && (verifyActiveMember == false || om.IsActive == true)
                                           && (roleType == null || om.Type == (int)roleType)
                                           && er.EntityType == (int)EntityRegionTypeCatalog.OrganizationMember
                                           && er.IsApproved == true
                                           && er.IsDeleted == false
                                           select new OrganizationMemberRegionModel
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
                                               Region = new EntityRegionBriefModel()
                                               {
                                                   Region = new RegionBriefModel()
                                                   {
                                                       Id = er.RegionId,
                                                   },
                                                   RegionLevel = (RegionLevelTypeCatalog)er.RegionLevel,
                                                   Country = new BaseBriefModel()
                                                   {
                                                       Id = er.CountryId ?? 0,
                                                       Name = "",
                                                       NativeName = "",
                                                       //Id = c == null ? 0 : c.Id,
                                                       //Name = c == null ? "" : c.Name,
                                                       //NativeName = c == null ? "" : c.NativeName,
                                                   },
                                                   State = new BaseBriefModel()
                                                   {
                                                       Id = er.StateId ?? 0,
                                                       Name = "",
                                                       NativeName = "",
                                                       //Id = s == null ? 0 : s.Id,
                                                       //Name = s == null ? "" : s.Name,
                                                       //NativeName = s == null ? "" : s.NativeName,
                                                   },
                                                   District = new BaseBriefModel()
                                                   {
                                                       Id = er.DistrictId ?? 0,
                                                       Name = "",
                                                       NativeName = "",
                                                       //Id = d == null ? 0 : d.Id,
                                                       //Name = d == null ? "" : d.Name,
                                                       //NativeName = d == null ? "" : d.NativeName,
                                                   },
                                                   Tehsil = new BaseBriefModel()
                                                   {
                                                       Id = er.TehsilId ?? 0,
                                                       Name = "",
                                                       NativeName = "",
                                                       //Id = t == null ? 0 : t.Id,
                                                       //Name = t == null ? "" : t.Name,
                                                       //NativeName = t == null ? "" : t.NativeName,
                                                   },
                                                   UnionCouncil = new BaseBriefModel()
                                                   {
                                                       Id = er.UnionCouncilId ?? 0,
                                                       Name = "",
                                                       NativeName = "",
                                                       //Id = uc == null ? 0 : uc.Id,
                                                       //Name = uc == null ? "" : uc.Name,
                                                       //NativeName = uc == null ? "" : uc.NativeName,
                                                   },
                                               },
                                               Role = (OrganizationMemberRolesCatalog)om.Type,
                                           }).ToListAsync()).GroupBy(x => new { x.Organization.Id, x.Role });
            foreach (var orgMemberRegion in lstOrgMembRegion)
            {
                var memberRegions = orgMemberRegion.Select(x => x.Region).ToList();
                orgMemberRegions.Add(new OrganizationMemberRegionModel()
                {
                    Organization = orgMemberRegion.FirstOrDefault().Organization,
                    Member = orgMemberRegion.FirstOrDefault().Member,
                    Role = orgMemberRegion.FirstOrDefault().Role,
                    Regions = memberRegions
                });
            }
            return orgMemberRegions;
        }
        private async Task<bool> IsOrganizationMembershipRequestAllowed(CharityContext _context, OrganizationRequestModel requestModel, OrganizationMemberModel membershipModel)
        {
            var sameTypeOfRequestInProcess = (await _context.OrganizationRequests.Where(
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
            if (!(await IsAlreadyAMember(_context, membershipModel.Organization.Id, _loggedInMemberId, membershipModel.Role)))
                return true;
            else
                return false;
        }
        private async Task<bool> IsAlreadyAMember(CharityContext _context, int organizationId, int memberId, OrganizationMemberRolesCatalog role)
        {
            var memberOrgRoles = (await GetMemberRoleForOrganization(_context, organizationId, memberId, false)).FirstOrDefault();
            if (memberOrgRoles != null && memberOrgRoles.Roles.Contains(role))
            {
                throw new KnownException($"This user is already {role} for this organization.");
            }
            return false;
        }
        private async Task<bool> IsOrganizationRequestThreadAccessible(CharityContext _context, RequestThreadModel requestModel)
        {
            var orgRequest = await _context.OrganizationRequests.Where(x => x.Id == requestModel.Entity.Id && x.IsDeleted == false).FirstOrDefaultAsync();
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
                        var memberOrgRoles = (await GetMemberRoleForOrganization(_context, orgRequest.OrganizationId, _loggedInMemberId)).FirstOrDefault();
                        if (IsOrganizationMemberOwner(memberOrgRoles))
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
