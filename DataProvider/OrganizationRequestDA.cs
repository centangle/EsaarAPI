using Catalogs;
using DataProvider.Helpers;
using Helpers;
using Models;
using Models.BriefModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace DataProvider
{
    public partial class DataAccess
    {
        public async Task<bool> AssignOrganizationRequest(int organizationId, int requestId, int? moderatorId)
        {
            using (CharityEntities context = new CharityEntities())
            {
                var organizationMember = (await GetMemberRoleForOrganization(context, organizationId, _loggedInMemberId)).FirstOrDefault();
                if (IsOrganizationMemberModerator(organizationMember))
                {
                    var organizationRequest = await context.OrganizationRequests.Where(x => x.Id == requestId).FirstOrDefaultAsync();
                    {
                        if (organizationRequest != null)
                        {
                            if (organizationRequest.IsDeleted)
                            {
                                throw new KnownException("This request has been deleted");
                            }
                            else if (organizationRequest.ModeratorId != null && organizationRequest.ModeratorId > 0)
                            {
                                throw new KnownException("This request has already been assigned");
                            }
                            using (var transaction = context.Database.BeginTransaction())
                            {
                                try
                                {
                                    var requestThreadModel = GetRequestThreadModelForOrganization(organizationRequest.Id, "Moderator Assigned");
                                    requestThreadModel.Status = StatusCatalog.ModeratorAssigned;
                                    await AddRequestThread(context, requestThreadModel);
                                    if (moderatorId == null || moderatorId < 1)
                                    {
                                        moderatorId = _loggedInMemberId;
                                    }
                                    organizationRequest.ModeratorId = moderatorId;
                                    await context.SaveChangesAsync();
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
                }
                throw new KnownException("You are not authorized to perform this action");
            }

        }
        private async Task<int> AddOrganizationRequest(CharityEntities context, OrganizationRequestModel model)
        {
            var dbModel = SetOrganizationRequest(new OrganizationRequest(), model);
            context.OrganizationRequests.Add(dbModel);
            await context.SaveChangesAsync();
            model.Id = dbModel.Id;
            var requestThreadModel = GetRequestThreadModelForOrganization(model.Id, model.Note);
            await AddRequestThread(context, requestThreadModel);
            return model.Id;
        }
        private OrganizationRequest SetOrganizationRequest(OrganizationRequest dbModel, OrganizationRequestModel model)
        {
            if (model.Organization == null || model.Organization.Id < 1)
            {
                throw new KnownException("Organization is required.");
            }
            model.Entity = SetEntityId(model.Entity, "Entity is required");
            dbModel.OrganizationId = model.Organization.Id;
            dbModel.EntityId = model.Entity.Id;
            dbModel.EntityType = (int)model.EntityType;
            dbModel.Type = (int)model.Type;
            if (dbModel.Id == 0)
                dbModel.Status = (int)StatusCatalog.Initiated;
            SetAndValidateBaseProperties(dbModel, model);
            return dbModel;
        }
        private async Task<bool> ChangeOrganizationRequestStatus(CharityEntities context, RequestThreadModel model)
        {
            var organizationRequest = await context.OrganizationRequests.Where(x => x.Id == model.Entity.Id).FirstOrDefaultAsync();
            if (organizationRequest != null)
            {
                var organizationMember = (await GetMemberRoleForOrganization(context, organizationRequest.OrganizationId, _loggedInMemberId)).FirstOrDefault();
                if (IsOrganizationMemberModerator(organizationMember))
                {
                    if (organizationRequest.IsDeleted == true)
                    {
                        return false;
                    }
                    organizationRequest.Status = (int)model.Status;
                    if (model.Status == StatusCatalog.Approved)
                    {
                        await AddOrganizationMemberForRequest(context, model);
                        var entityRegions = await context.EntityRegions.Where(x => x.RequestId == organizationRequest.Id && x.RequestType == (int)EntityRegionRequestTypeCatalog.OrganizationMember && x.IsDeleted == false).ToListAsync();
                        foreach (var entityRegion in entityRegions)
                        {
                            entityRegion.IsApproved = true;
                        }
                    }
                    return true;
                }
                else
                {
                    throw new KnownException("You are not authorized to change status.");
                }
            }
            else
            {
                return false;
            }

        }
        private RequestThreadModel GetRequestThreadModelForOrganization(int id, string note)
        {
            RequestThreadModel requestThreadModel = new RequestThreadModel();
            requestThreadModel.Entity.Id = id;
            requestThreadModel.EntityType = RequestThreadEntityTypeCatalog.Organization;
            requestThreadModel.Status = StatusCatalog.Initiated;
            requestThreadModel.Note = note;
            requestThreadModel.Type = RequestThreadTypeCatalog.General;
            requestThreadModel.IsSystemGenerated = true;
            return requestThreadModel;
        }
        public async Task<PaginatedResultModel<PaginatedOrganizationRequestModel>> GetOrganizationRequests(OrganizationRequestSearchModel filters)
        {
            using (CharityEntities context = new CharityEntities())
            {
                var memberOrganizations = await GetMemberRoleForOrganization(context, null, _loggedInMemberId);

                List<int> memberModeratorOrgz = new List<int>();
                List<int> memberOwnedOrgz = new List<int>();
                if (memberModeratorOrgz != null)
                {
                    foreach (var memberOrg in memberOrganizations)
                    {
                        if (IsOrganizationMemberModerator(memberOrg))
                        {
                            memberModeratorOrgz.Add(memberOrg.Organization.Id);
                        }
                        if (IsOrganizationMemberOwner(memberOrg))
                        {
                            memberOwnedOrgz.Add(memberOrg.Organization.Id);
                        }
                    }
                }
                var requestQueryable = (from ort in context.OrganizationRequests
                                        join o in context.Organizations on ort.OrganizationId equals o.Id
                                        join m in context.Members on ort.CreatedBy equals m.Id
                                        join am in context.Members on ort.ModeratorId equals am.Id into tam
                                        from am in tam.DefaultIfEmpty()
                                        let isLoggedInMemberOrgOwner = memberOwnedOrgz.Any(x => x == o.Id)
                                        let isLoggedInMemberOrgModerator = memberModeratorOrgz.Any(x => x == o.Id)
                                        where
                                        (filters.OrganizationId == null || ort.OrganizationId == filters.OrganizationId)
                                        && (filters.Type == null || ort.Type == (int)filters.Type.Value)
                                        && ort.IsDeleted == false
                                        &&
                                        (
                                             ort.CreatedBy == _loggedInMemberId
                                             ||
                                             isLoggedInMemberOrgOwner
                                             ||
                                             isLoggedInMemberOrgModerator
                                        )
                                        select new PaginatedOrganizationRequestModel
                                        {
                                            Id = ort.Id,
                                            Organization = new BaseImageBriefModel()
                                            {
                                                Id = o.Id,
                                                Name = o.Name,
                                                NativeName = o.NativeName,
                                                ImageUrl = o.LogoUrl,
                                            },
                                            Entity = new BaseBriefModel()
                                            {
                                                Id = m.Id,
                                                Name = m.Name,
                                                NativeName = m.NativeName,
                                            },
                                            Moderator = new BaseBriefModel()
                                            {
                                                Id = am == null ? 0 : am.Id,
                                                Name = am == null ? "" : am.Name,
                                                NativeName = am == null ? "" : am.NativeName,
                                            },
                                            EntityType = (OrganizationRequestEntityTypeCatalog)ort.EntityType,
                                            Type = (OrganizationRequestTypeCatalog)ort.Type,
                                            Status = (StatusCatalog)ort.Status,
                                            LoggedInMemberId = _loggedInMemberId,
                                            IsLoggedInMemberOrganizationOwner = isLoggedInMemberOrgOwner,
                                            IsLoggedInMemberOrganizationModerator = isLoggedInMemberOrgModerator,
                                            CreatedDate = ort.CreatedDate,
                                            CreatedBy = ort.CreatedBy,
                                        }).AsQueryable();

                var requests = await requestQueryable.Paginate(filters);
                foreach (var request in requests.Items)
                {
                    if (request.Type == OrganizationRequestTypeCatalog.Moderator || request.Type == OrganizationRequestTypeCatalog.Volunteer)
                        request.Regions = await GetRequestEntityRegions(request.Id);
                }
                return requests;
            }
        }
    }
}
