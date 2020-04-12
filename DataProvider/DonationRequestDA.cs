using Catalogs;
using DataProvider.Helpers;
using Helpers;
using Models;
using Models.BriefModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DataProvider
{
    public partial class DataAccess
    {
        public async Task<int> AddDonationRequest(DonationRequestModel model)
        {
            using (CharityEntities context = new CharityEntities())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        DonationRequest dbModel = new DonationRequest();
                        SetRequest(dbModel, model);
                        context.DonationRequests.Add(dbModel);
                        await context.SaveChangesAsync();
                        model.Id = dbModel.Id;
                        AddDonationRequestItems(context, model.Items, model.Id);
                        var donationRequestOrganizations = GetDonationRequestOrganization(model);
                        await AddDonationRequestOrganizations(context, donationRequestOrganizations, model.Id);
                        foreach (var org in donationRequestOrganizations)
                        {
                            var requestThreadModel = GetDonationRequestThreadModel(org.Id);
                            await AddRequestThread(context, requestThreadModel);
                        }
                        await context.SaveChangesAsync();
                        transaction.Commit();
                        return model.Id;
                    }
                    catch (DataException ex)
                    {
                        transaction.Rollback();
                        throw new KnownException(ExceptionHelper.GetDataExceptionMessage(ex));
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
        }
        public async Task<bool> UpdateDonationRequest(DonationRequestModel model)
        {
            using (CharityEntities context = new CharityEntities())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        DonationRequest dbModel = new DonationRequest();
                        SetRequest(dbModel, model);
                        context.DonationRequests.Add(dbModel);
                        await context.SaveChangesAsync();
                        model.Id = dbModel.Id;
                        await UpdateDonationRequestItems(context, model.Items, model.Id);
                        await context.SaveChangesAsync();
                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
            }
        }
        private DonationRequest SetRequest(DonationRequest dbModel, DonationRequestModel model)
        {
            model.Member = SetEntityId(model.Member, "Member is required");
            dbModel.MemberId = model.Member.Id;
            dbModel.Type = (int)model.Type;
            dbModel.Note = model.Note;
            dbModel.PrefferedCollectionTime = model.PrefferedCollectionTime;
            dbModel.Address = model.Address;
            dbModel.AddressLatLong = model.AddressLatLong;
            if (model.Campaign != null && model.Campaign.Id > 0)
            {
                dbModel.CampaignId = model.Campaign.Id;
            }
            else
            {
                dbModel.CampaignId = null;
            }
            dbModel.Date = DateTime.UtcNow;
            SetBaseProperties(dbModel, model);
            return dbModel;
        }

        public async Task<PaginatedResultModel<PaginatedDonationRequestModel>> GetDonationRequests(DonationRequestSearchModel filters)
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
                var requestQueryable = (from dr in context.DonationRequests
                                        join m in context.Members on dr.CreatedBy equals m.Id
                                        join dro in context.DonationRequestOrganizations on dr.Id equals dro.DonationRequestId
                                        join o in context.Organizations on dro.OrganizationId equals o.Id
                                        join am in context.Members on dro.AssignedTo equals am.Id into tam
                                        from am in tam.DefaultIfEmpty()
                                        where
                                        (filters.OrganizationId == null || dro.OrganizationId == filters.OrganizationId)
                                        && (filters.Type == null || dr.Type == (int)filters.Type.Value)
                                        && dr.IsDeleted == false
                                        &&
                                        (
                                             dr.MemberId == _loggedInMemberId
                                             ||
                                             o.OwnedBy == _loggedInMemberId
                                             ||
                                             memberModeratorOrgz.Any(x => x == o.Id)
                                        )
                                        select new PaginatedDonationRequestModel
                                        {
                                            Id = dr.Id,
                                            Member = new BaseBriefModel()
                                            {
                                                Id = m.Id,
                                                Name = m.Name,
                                                NativeName = m.NativeName,
                                            },

                                            //Campaign = new BaseBriefModel()
                                            //{
                                            //    Id = m.Id,
                                            //    Name = m.Name,
                                            //    NativeName = m.NativeName,
                                            //},
                                            DonationRequestOrganization = new DonationRequestOrganizationModel()
                                            {
                                                Id = dro.Id,
                                                Organization = new BaseBriefModel()
                                                {
                                                    Id = o.Id,
                                                    Name = o.Name,
                                                    NativeName = o.NativeName,
                                                },
                                                AssignedTo = new BaseBriefModel()
                                                {
                                                    Id = am == null ? 0 : am.Id,
                                                    Name = am == null ? "" : am.Name,
                                                    NativeName = am == null ? "" : am.NativeName,
                                                },
                                                Status = (StatusCatalog)dro.Status,
                                            },
                                            Type=(DonationRequestTypeCatalog)dr.Type,
                                            LoggedInMemberId = _loggedInMemberId,
                                            CreatedDate = dr.CreatedDate
                                        }).AsQueryable();

                return await requestQueryable.Paginate(filters);
            }
        }


    }
}
