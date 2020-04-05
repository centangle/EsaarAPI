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
            if (model.Entity == null || model.Entity.Id < 1)
            {
                if (_loggedInMemberId == 0)
                    throw new KnownException("Entity is required.");
                else
                {
                    model.Entity = new Models.BriefModel.BaseBriefModel();
                    model.Entity.Id = _loggedInMemberId;
                }
            }
            dbModel.OrganizationId = model.Organization.Id;
            dbModel.EntityId = model.Entity.Id;
            dbModel.EntityType = (int)model.EntityType;
            dbModel.Type = (int)model.Type;
            if (dbModel.Id == 0)
                dbModel.Status = (int)StatusCatalog.Initiated;
            SetBaseProperties(dbModel, model);
            return dbModel;
        }
        private async Task ChangeOrganizationRequestStatus(CharityEntities context, RequestThreadModel model)
        {
            var organizationRequest = await context.OrganizationRequests.Where(x => x.Id == model.Entity.Id).FirstOrDefaultAsync();
            if (organizationRequest != null)
            {
                organizationRequest.Status = (int)model.Status;
            }
        }
        public async Task<PaginatedResultModel<OrganizationRequestModel>> GetOrganizationRequests(OrganizationRequestSearchModel filters)
        {
            using (CharityEntities context = new CharityEntities())
            {
                var requestQueryable = (from ort in context.OrganizationRequests
                                        join om in context.OrganizationMembers on _loggedInMemberId equals om.MemberId into tom
                                        from om in tom.DefaultIfEmpty()
                                        join o in context.Organizations on ort.OrganizationId equals o.Id
                                        join m in context.Members on ort.CreatedBy equals m.Id
                                        join am in context.Members on ort.AssignedTo equals am.Id into tam
                                        from am in tam.DefaultIfEmpty()
                                        where
                                        ort.OrganizationId == filters.OrganizationId
                                        && (filters.Type == null || ort.Type == (int)filters.Type.Value)
                                        && ort.IsDeleted == false
                                        &&
                                        (
                                             om.Type == (int)OrganizationMemberTypeCatalog.Moderator
                                             ||
                                             om.Type == (int)OrganizationMemberTypeCatalog.Owner
                                             ||
                                             ort.CreatedBy == _loggedInMemberId
                                             ||
                                             o.OwnedBy == _loggedInMemberId
                                        )
                                        select new OrganizationRequestModel
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
                                            Status = (StatusCatalog)ort.Status,
                                            CreatedDate = ort.CreatedDate
                                        }).AsQueryable();

                return await requestQueryable.Paginate(filters);
            }
        }
    }
}
