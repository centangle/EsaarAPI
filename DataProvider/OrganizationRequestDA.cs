using Catalogs;
using DataProvider.Helpers;
using Helpers;
using Models;
using Models.BriefModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProvider
{
    public partial class DataAccess
    {
        public async Task<bool> AssignRequest(int organizationId, int requestId, int? moderatorId)
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
                            else if (organizationRequest.AssignedTo != null && organizationRequest.AssignedTo > 0)
                            {
                                throw new KnownException("This request has already been assigned");
                            }
                            if (moderatorId == null || moderatorId < 1)
                            {
                                moderatorId = _loggedInMemberId;
                            }
                            organizationRequest.AssignedTo = moderatorId;
                            await context.SaveChangesAsync();
                            return true;
                        }
                    }
                }
                return false;
            }

        }
        private async Task<int> AddOrganizationRequest(CharityEntities context, OrganizationRequestModel model)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var dbModel = SetOrganizationRequest(new OrganizationRequest(), model);
                    context.OrganizationRequests.Add(dbModel);
                    await context.SaveChangesAsync();
                    model.Id = dbModel.Id;
                    var requestThreadModel = GetRequestThreadModel(model);
                    await AddRequestThread(context, requestThreadModel);
                    transaction.Commit();
                    return model.Id;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }

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
                dbModel.Status = (int)OrganizationStatusCatalog.Initiated;
            SetBaseProperties(dbModel, model);
            return dbModel;
        }
        private async Task<bool> ChangeOrganizationRequestStatus(CharityEntities context, RequestThreadModel model)
        {
            var organizationRequest = await context.OrganizationRequests.Where(x => x.Id == model.Entity.Id).FirstOrDefaultAsync();
            if (organizationRequest != null)
            {
                if (organizationRequest.IsDeleted == true)
                {
                    return false;
                }
                organizationRequest.Status = (int)model.Status;
                return true;
            }
            return false;
        }
        public async Task<PaginatedResultModel<PaginatedOrganizationRequestModel>> GetOrganizationRequests(OrganizationRequestSearchModel filters)
        {
            using (CharityEntities context = new CharityEntities())
            {
                var memberOrganizations = await GetMemberRoleForOrganization(context, null, _loggedInMemberId);

                List<int> memberModeratorOrgz = new List<int>();
                if (memberModeratorOrgz != null)
                {
                    foreach (var memberOrg in memberOrganizations)
                    {
                        if (IsOrganizationMemberModerator(memberOrg))
                        {
                            memberModeratorOrgz.Add(memberOrg.Organization.Id);
                        }
                    }
                }
                var requestQueryable = (from ort in context.OrganizationRequests
                                        join o in context.Organizations on ort.OrganizationId equals o.Id
                                        join m in context.Members on ort.CreatedBy equals m.Id
                                        join am in context.Members on ort.AssignedTo equals am.Id into tam
                                        from am in tam.DefaultIfEmpty()
                                        where
                                        (filters.OrganizationId == null || ort.OrganizationId == filters.OrganizationId)
                                        && (filters.Type == null || ort.Type == (int)filters.Type.Value)
                                        && ort.IsDeleted == false
                                        &&
                                        (
                                             ort.CreatedBy == _loggedInMemberId
                                             ||
                                             o.OwnedBy == _loggedInMemberId
                                             ||
                                             memberModeratorOrgz.Any(x => x == o.Id)
                                        )
                                        select new PaginatedOrganizationRequestModel
                                        {
                                            Id = ort.Id,
                                            Organization = new BaseBriefModel()
                                            {
                                                Id = o.Id,
                                                Name = o.Name,
                                                NativeName = o.NativeName,
                                            },
                                            Entity = new BaseBriefModel()
                                            {
                                                Id = m.Id,
                                                Name = m.Name,
                                                NativeName = m.NativeName,
                                            },
                                            AssignedTo = new BaseBriefModel()
                                            {
                                                Id = am == null ? 0 : am.Id,
                                                Name = am == null ? "" : am.Name,
                                                NativeName = am == null ? "" : am.NativeName,
                                            },
                                            EntityType = (OrganizationRequestEntityTypeCatalog)ort.EntityType,
                                            Type = (OrganizationRequestTypeCatalog)ort.Type,
                                            Status = (OrganizationStatusCatalog)ort.Status,
                                            LoggedInMemberId = _loggedInMemberId,
                                            CreatedDate = ort.CreatedDate
                                        }).AsQueryable();

                return await requestQueryable.Paginate(filters);
            }
        }
    }
}
