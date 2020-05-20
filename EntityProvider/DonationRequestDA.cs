using Catalogs;
using EntityProvider.Helpers;
using Helpers;
using Models;
using Models.BriefModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using EntityProvider.DbModels;
using Microsoft.EntityFrameworkCore;

namespace EntityProvider
{
    public partial class DataAccess
    {
        public async Task<int> AddDonationRequest(DonationRequestModel model)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    DonationRequest dbModel = new DonationRequest();
                    SetRequest(dbModel, model);
                    _context.DonationRequests.Add(dbModel);
                    await _context.SaveChangesAsync();
                    model.Id = dbModel.Id;
                    AddDonationRequestItems(_context, model.Items, model.Id);
                    var donationRequestOrganizations = GetDonationRequestOrganization(model);
                    await AddDonationRequestOrganizations(_context, donationRequestOrganizations, model.Id);
                    foreach (var org in donationRequestOrganizations)
                    {
                        var requestThreadModel = GetDonationRequestThreadModel(org.Id);
                        await AddRequestThread(_context, requestThreadModel);
                    }
                    await _context.SaveChangesAsync();
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
        public async Task<bool> UpdateDonationRequest(DonationRequestModel model)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    DonationRequest dbModel = new DonationRequest();
                    SetRequest(dbModel, model);
                    _context.DonationRequests.Add(dbModel);
                    await _context.SaveChangesAsync();
                    model.Id = dbModel.Id;
                    await UpdateDonationRequestItems(_context, model.Items, model.Id);
                    await _context.SaveChangesAsync();
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
        public async Task<bool> AssignModeratorToDonationRequest(int organizationId, int donationRequestId, int? moderatorId)
        {
            var orgMemberRoles = (await GetMemberRoleForOrganization(_context, organizationId, _loggedInMemberId)).FirstOrDefault();
            if (IsOrganizationMemberModerator(orgMemberRoles))
            {
                var donationRequest = await _context.DonationRequests.Where(x => x.Id == donationRequestId).FirstOrDefaultAsync();
                {
                    if (donationRequest != null)
                    {
                        if (donationRequest.IsDeleted)
                        {
                            throw new KnownException("This request has been deleted");
                        }
                        var donationRequestOrganization = await _context.DonationRequestOrganizations.Where(x => x.DonationRequestId == donationRequestId && x.OrganizationId == organizationId && x.IsDeleted == false).FirstOrDefaultAsync();
                        if (donationRequestOrganization == null || donationRequestOrganization.ModeratorId != null && donationRequestOrganization.ModeratorId > 0)
                        {
                            throw new KnownException("This request has already been assigned");
                        }
                        using (var transaction = _context.Database.BeginTransaction())
                        {
                            try
                            {
                                var requestThreadModel = GetDonationRequestThreadModel(donationRequestOrganization.Id);
                                requestThreadModel.Status = StatusCatalog.ModeratorAssigned;
                                await AddRequestThread(_context, requestThreadModel);
                                if (moderatorId == null || moderatorId < 1)
                                {
                                    moderatorId = _loggedInMemberId;
                                }
                                donationRequestOrganization.ModeratorId = moderatorId;
                                await _context.SaveChangesAsync();
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
        public async Task<bool> AssignVolunteerToDonationRequest(int organizationId, int donationRequestId, int? volunteerId)
        {
            var orgaMemberRole = (await GetMemberRoleForOrganization(_context, organizationId, _loggedInMemberId)).FirstOrDefault();
            if (IsOrganizationMemberModerator(orgaMemberRole) || IsOrganizationMemberVolunteer(orgaMemberRole))
            {
                var donationRequest = await _context.DonationRequests.Where(x => x.Id == donationRequestId).FirstOrDefaultAsync();
                {
                    if (donationRequest != null)
                    {

                        if (donationRequest.IsDeleted)
                        {
                            throw new KnownException("This request has been deleted");
                        }
                        var donationRequestOrganization = await _context.DonationRequestOrganizations.Where(x => x.DonationRequestId == donationRequestId && x.OrganizationId == organizationId && x.IsDeleted == false).FirstOrDefaultAsync();
                        if (donationRequestOrganization == null || donationRequestOrganization.VolunteerId != null && donationRequestOrganization.VolunteerId > 0)
                        {
                            throw new KnownException("This request has already been assigned");
                        }
                        using (var transaction = _context.Database.BeginTransaction())
                        {
                            try
                            {
                                var requestThreadModel = GetDonationRequestThreadModel(donationRequestOrganization.Id);
                                requestThreadModel.Status = StatusCatalog.VolunteerAssigned;
                                await AddRequestThread(_context, requestThreadModel);
                                if (volunteerId == null || volunteerId < 1)
                                {
                                    volunteerId = _loggedInMemberId;
                                }
                                else
                                {
                                    if (IsOrganizationMemberModerator(orgaMemberRole) == false)
                                    {
                                        throw new KnownException("You are not authorized to perform this action");
                                    }
                                }

                                donationRequestOrganization.VolunteerId = volunteerId;
                                await _context.SaveChangesAsync();
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
            SetAndValidateBaseProperties(dbModel, model);
            return dbModel;
        }
        public async Task<bool> UpdateDonationRequestStatus(int donationRequestOrganizationId, string note, List<DonationRequestOrganizationItemModel> items, StatusCatalog status)
        {
            try
            {
                var requestThreadModel = GetRequestThreadModel(donationRequestOrganizationId, StatusCatalog.Approved, note);
                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        int id = await ProcessRequestThread(_context, requestThreadModel);
                        if (status == StatusCatalog.Approved)
                        {
                            await AddDonationRequestOrganizationItems(_context, items, donationRequestOrganizationId);
                        }
                        else
                        {
                            await UpdateDonationRequestOrganizationItems(_context, items, donationRequestOrganizationId, status);
                        }
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }

                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private RequestThreadModel GetRequestThreadModel(int donationRequestOrganizationId, StatusCatalog status, string note)
        {
            RequestThreadModel requestThreadModel = new RequestThreadModel();
            requestThreadModel.Entity.Id = donationRequestOrganizationId;
            requestThreadModel.EntityType = RequestThreadEntityTypeCatalog.Donation;
            requestThreadModel.Status = status;
            requestThreadModel.Note = note;
            requestThreadModel.Type = RequestThreadTypeCatalog.General;
            requestThreadModel.IsSystemGenerated = true;
            return requestThreadModel;
        }
        public async Task<PaginatedDonationRequestModel> GetDonationRequestDetail(int organizationRequestId)
        {
            var memberOrgRoles = await GetMemberRoleForOrganization(_context, null, _loggedInMemberId);
            List<int> memberModeratorOrgz = new List<int>();
            List<int> memberOwnedOrgz = new List<int>();
            if (memberOrgRoles != null)
            {
                foreach (var memberOrgRole in memberOrgRoles)
                {
                    if (IsOrganizationMemberModerator(memberOrgRole))
                    {
                        memberModeratorOrgz.Add(memberOrgRole.Organization.Id);
                    }
                    if (IsOrganizationMemberOwner(memberOrgRole))
                    {
                        memberOwnedOrgz.Add(memberOrgRole.Organization.Id);
                    }
                }
            }
            var donationRequest = await (from dr in _context.DonationRequests
                                         join m in _context.Members on dr.CreatedBy equals m.Id
                                         join dro in _context.DonationRequestOrganizations on dr.Id equals dro.DonationRequestId
                                         join o in _context.Organizations on dro.OrganizationId equals o.Id
                                         join am in _context.Members on dro.ModeratorId equals am.Id into tam
                                         from am in tam.DefaultIfEmpty()
                                         join v in _context.Members on dro.VolunteerId equals v.Id into tv
                                         from v in tam.DefaultIfEmpty()
                                         let isLoggedInMemberOrgOwner = memberOwnedOrgz.Any(x => x == o.Id)
                                         let isLoggedInMemberOrgModerator = memberModeratorOrgz.Any(x => x == o.Id)
                                         where
                                         dro.Id == organizationRequestId
                                         && dr.IsDeleted == false
                                         &&
                                         (
                                              dr.MemberId == _loggedInMemberId
                                              ||
                                              dro.ModeratorId == _loggedInMemberId
                                              ||
                                              dro.VolunteerId == _loggedInMemberId
                                              ||
                                              isLoggedInMemberOrgOwner
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

                                             Campaign = new BaseBriefModel()
                                             {
                                                 Id = m.Id,
                                                 Name = m.Name,
                                                 NativeName = m.NativeName,
                                             },
                                             DonationRequestOrganization = new DonationRequestOrganizationModel()
                                             {
                                                 Id = dro.Id,
                                                 Organization = new BaseBriefModel()
                                                 {
                                                     Id = o.Id,
                                                     Name = o.Name,
                                                     NativeName = o.NativeName,
                                                 },
                                                 Moderator = new BaseBriefModel()
                                                 {
                                                     Id = am == null ? 0 : am.Id,
                                                     Name = am == null ? "" : am.Name,
                                                     NativeName = am == null ? "" : am.NativeName,
                                                 },
                                                 Volunteer = new BaseBriefModel()
                                                 {
                                                     Id = v == null ? 0 : v.Id,
                                                     Name = v == null ? "" : v.Name,
                                                     NativeName = v == null ? "" : v.NativeName,
                                                 },
                                                 Status = (StatusCatalog)dro.Status,
                                             },
                                             Type = (DonationRequestTypeCatalog)dr.Type,
                                             LoggedInMemberId = _loggedInMemberId,
                                             IsLoggedInMemberOrganizationOwner = isLoggedInMemberOrgOwner,
                                             IsLoggedInMemberOrganizationModerator = isLoggedInMemberOrgModerator,
                                             CreatedDate = dr.CreatedDate,
                                             CreatedBy = dr.CreatedBy,
                                         }).FirstOrDefaultAsync();
            return donationRequest;
        }
        public async Task<List<DonationRequestOrganizationItemModel>> GetDonationRequestItems(int organizationRequestId)
        {
            var memberOrgRoles = await GetMemberRoleForOrganization(_context, null, _loggedInMemberId);

            List<int> memberModeratorOrgz = new List<int>();
            List<int> memberOwnedOrgz = new List<int>();
            if (memberOrgRoles != null)
            {
                foreach (var memberOrgRole in memberOrgRoles)
                {
                    if (IsOrganizationMemberModerator(memberOrgRole))
                    {
                        memberModeratorOrgz.Add(memberOrgRole.Organization.Id);
                    }
                    if (IsOrganizationMemberOwner(memberOrgRole))
                    {
                        memberOwnedOrgz.Add(memberOrgRole.Organization.Id);
                    }
                }
            }
            var items = await (from dr in _context.DonationRequests
                               join dri in _context.DonationRequestItems on dr.Id equals dri.DonationRequestId
                               join i in _context.Items on dri.ItemId equals i.Id
                               join uom in _context.Uoms on dri.QuantityUom equals uom.Id
                               join iuom in _context.Uoms on i.DefaultUom equals iuom.Id
                               join dro in _context.DonationRequestOrganizations on dr.Id equals dro.DonationRequestId
                               join droi in _context.DonationRequestOrganizationItems on
                                 new { orgId = dro.Id, itemId = dri.ItemId }
                                 equals
                                 new { orgId = droi.RequestOrganizationId, itemId = droi.RequestItemId }
                                 into ldroi
                               from droi in ldroi.DefaultIfEmpty()
                               join aiuom in _context.Uoms on droi.QuantityUom equals aiuom.Id into laiuom
                               from aiuom in laiuom.DefaultIfEmpty()
                               join ciuom in _context.Uoms on droi.QuantityUom equals ciuom.Id into lciuom
                               from ciuom in lciuom.DefaultIfEmpty()
                               join diuom in _context.Uoms on droi.QuantityUom equals diuom.Id into ldiuom
                               from diuom in ldiuom.DefaultIfEmpty()
                               let isLoggedInMemberOrgOwner = memberOwnedOrgz.Any(x => x == dro.OrganizationId)
                               let isLoggedInMemberOrgModerator = memberModeratorOrgz.Any(x => x == dro.OrganizationId)
                               where
                               dro.Id == organizationRequestId
                               && dr.IsDeleted == false
                               &&
                               (
                                    dr.MemberId == _loggedInMemberId
                                    ||
                                    dro.ModeratorId == _loggedInMemberId
                                    ||
                                    isLoggedInMemberOrgOwner
                               )
                               select new DonationRequestOrganizationItemModel
                               {
                                   Id = dr.Id,
                                   Item = new BaseBriefModel()
                                   {
                                       Id = i.Id,
                                       Name = i.Name,
                                       NativeName = i.NativeName,
                                   },
                                   ItemDefaultUOM = new UOMBriefParentModel()
                                   {
                                       Id = iuom.Id,
                                       Name = iuom.Name,
                                       NativeName = iuom.NativeName,
                                       ParentId = iuom.ParentId,
                                   },
                                   Quantity = dri.Quantity,
                                   QuantityUOM = new UOMBriefModel()
                                   {
                                       Id = uom.Id,
                                       Name = uom.Name,
                                       NativeName = uom.NativeName,
                                       NoOfBaseUnit = uom.NoOfBaseUnit,
                                   },
                                   IsApproved = droi == null ? false : true,
                                   ApprovedQuantity = droi == null ? 0 : droi.Quantity,
                                   ApprovedQuantityUOM = new UOMBriefModel()
                                   {
                                       Id = aiuom == null ? 0 : aiuom.Id,
                                       Name = aiuom == null ? "" : aiuom.Name,
                                       NativeName = aiuom == null ? "" : aiuom.NativeName,
                                       NoOfBaseUnit = aiuom == null ? 0 : aiuom.NoOfBaseUnit,
                                   },
                                   CollectedQuantity = droi == null ? 0 : droi.CollectedQuantity,
                                   CollectedQuantityUOM = new UOMBriefModel()
                                   {
                                       Id = ciuom == null ? 0 : ciuom.Id,
                                       Name = ciuom == null ? "" : ciuom.Name,
                                       NativeName = ciuom == null ? "" : ciuom.NativeName,
                                       NoOfBaseUnit = ciuom == null ? 0 : ciuom.NoOfBaseUnit,
                                   },
                                   DeliveredQuantity = droi == null ? 0 : droi.DeliveredQuantity,
                                   DeliveredQuantityUOM = new UOMBriefModel()
                                   {
                                       Id = diuom == null ? 0 : diuom.Id,
                                       Name = diuom == null ? "" : diuom.Name,
                                       NativeName = diuom == null ? "" : diuom.NativeName,
                                       NoOfBaseUnit = diuom == null ? 0 : diuom.NoOfBaseUnit,
                                   },

                               }).ToListAsync();
            List<UOMModel> availableUOM = new List<UOMModel>();
            foreach (var item in items)
            {
                int itemDefaultUOMRootId = (item.ItemDefaultUOM.ParentId == null ?
                    item.ItemDefaultUOM.Id :
                    (item.ItemDefaultUOM.ParentId ?? 0));
                var existingUOM = availableUOM.Where(x => x.Id == itemDefaultUOMRootId).FirstOrDefault();
                if (existingUOM == null)
                {
                    var uom = await GetUOM(itemDefaultUOMRootId);
                    if (uom != null)
                    {
                        availableUOM.Add(uom);
                        item.ItemUOMs.AddRange(GetUOMTreeFlatList(uom));
                    }
                }

            }
            return items;
        }
        public async Task<PaginatedResultModel<PaginatedDonationRequestModel>> GetDonationRequests(DonationRequestSearchModel filters)
        {
            DateTime startDateFilter = TimePeriodHelper.GetStartDate(filters.TimePeriod, filters.StartDate);
            DateTime endDateFilter = TimePeriodHelper.GetEndDate(filters.TimePeriod, filters.EndDate);
            var memberOrgRoles = await GetMemberRoleForOrganization(_context, null, _loggedInMemberId);
            List<int> memberModeratorOrgz = new List<int>();
            List<int> memberOwnedOrgz = new List<int>();
            if (memberOrgRoles != null)
            {
                foreach (var memberOrg in memberOrgRoles)
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
            var requestQueryable = (from dr in _context.DonationRequests
                                    join m in _context.Members on dr.CreatedBy equals m.Id
                                    join dro in _context.DonationRequestOrganizations on dr.Id equals dro.DonationRequestId
                                    join o in _context.Organizations on dro.OrganizationId equals o.Id
                                    join am in _context.Members on dro.ModeratorId equals am.Id into tam
                                    from am in tam.DefaultIfEmpty()
                                    join v in _context.Members on dro.VolunteerId equals v.Id into tv
                                    from v in tv.DefaultIfEmpty()
                                    join ov in _context.OrganizationMembers on
                                        new { orgId = o.Id, type = (int)OrganizationMemberRolesCatalog.Volunteer, memberId = _loggedInMemberId }
                                        equals
                                        new { orgId = ov.Organization.Id, type = ov.Type, memberId = ov.MemberId }
                                        into lov
                                    from ov in lov.DefaultIfEmpty()
                                    let isLoggedInMemberOrgOwner = memberOwnedOrgz.Any(x => x == o.Id)
                                    let isLoggedInMemberOrgModerator = memberModeratorOrgz.Any(x => x == o.Id)
                                    let isLoggedInMemberOrgVolunteer = ov != null
                                    where
                                    (filters.OrganizationId == null || dro.OrganizationId == filters.OrganizationId)
                                    && (filters.Types.Count == 0 || filters.Types.Contains((DonationRequestTypeCatalog)dr.Type))
                                    && (filters.Statuses.Count == 0 || filters.Statuses.Contains((StatusCatalog)dro.Status))
                                    && (string.IsNullOrEmpty(filters.MemberName) || m.Name.Contains(filters.MemberName) || m.NativeName.Contains(filters.MemberName))
                                    && (dr.CreatedDate >= startDateFilter && dr.CreatedDate <= endDateFilter)
                                    && dr.IsDeleted == false
                                    &&
                                    (
                                         dr.MemberId == _loggedInMemberId
                                         ||
                                         isLoggedInMemberOrgOwner
                                         ||
                                         isLoggedInMemberOrgModerator
                                         ||
                                         (// Volunteer Region Check
                                             isLoggedInMemberOrgVolunteer
                                             &&
                                             (
                                                (_context.EntityRegions.Where(x => x.RegionLevel == (int)RegionLevelTypeCatalog.UnionCouncil
                                                    && x.EntityType == (int)EntityRegionTypeCatalog.OrganizationMember
                                                    && x.UnionCouncilId == dr.UnionCouncilId).Count() > 0)
                                                ||
                                                (_context.EntityRegions.Where(x => x.RegionLevel == (int)RegionLevelTypeCatalog.Tehsil
                                                   && x.EntityType == (int)EntityRegionTypeCatalog.OrganizationMember
                                                   && x.TehsilId == dr.TehsilId).Count() > 0)
                                                ||
                                                (_context.EntityRegions.Where(x => x.RegionLevel == (int)RegionLevelTypeCatalog.District
                                                   && x.EntityType == (int)EntityRegionTypeCatalog.OrganizationMember
                                                   && x.DistrictId == dr.DistrictId).Count() > 0)
                                                ||
                                                (_context.EntityRegions.Where(x => x.RegionLevel == (int)RegionLevelTypeCatalog.State
                                                   && x.EntityType == (int)EntityRegionTypeCatalog.OrganizationMember
                                                   && x.StateId == dr.StateId).Count() > 0)
                                                ||
                                                (_context.EntityRegions.Where(x => x.RegionLevel == (int)RegionLevelTypeCatalog.Country
                                                   && x.EntityType == (int)EntityRegionTypeCatalog.OrganizationMember
                                                   && x.CountryId == dr.CountryId).Count() > 0)
                                             )
                                             &&
                                             (dro.Status >= (int)StatusCatalog.Approved && dro.Status < (int)StatusCatalog.Rejected)
                                         )
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

                                        Campaign = new BaseBriefModel()
                                        {
                                            Id = m.Id,
                                            Name = m.Name,
                                            NativeName = m.NativeName,
                                        },
                                        DonationRequestOrganization = new DonationRequestOrganizationModel()
                                        {
                                            Id = dro.Id,
                                            Organization = new BaseBriefModel()
                                            {
                                                Id = o.Id,
                                                Name = o.Name,
                                                NativeName = o.NativeName,
                                            },
                                            Moderator = new BaseBriefModel()
                                            {
                                                Id = am == null ? 0 : am.Id,
                                                Name = am == null ? "" : am.Name,
                                                NativeName = am == null ? "" : am.NativeName,
                                            },
                                            Volunteer = new BaseBriefModel()
                                            {
                                                Id = v == null ? 0 : v.Id,
                                                Name = v == null ? "" : v.Name,
                                                NativeName = v == null ? "" : v.NativeName,
                                            },
                                            Status = (StatusCatalog)dro.Status,
                                        },
                                        Type = (DonationRequestTypeCatalog)dr.Type,
                                        LoggedInMemberId = _loggedInMemberId,
                                        IsLoggedInMemberOrganizationOwner = isLoggedInMemberOrgOwner,
                                        IsLoggedInMemberOrganizationModerator = isLoggedInMemberOrgModerator,
                                        IsLoggedInMemberOrganizationVolunteer = isLoggedInMemberOrgVolunteer,
                                        CreatedDate = dr.CreatedDate,
                                        CreatedBy = dr.CreatedBy,
                                    }).AsQueryable();

            return await requestQueryable.Paginate(filters);
        }
        public async Task<PaginatedResultModel<PaginatedDonationRequestModel>> GetDonationRequestsForVolunteer(DonationRequestSearchModel filters)
        {
            var memberOrgRoles = await GetMemberRoleForOrganization(_context, null, _loggedInMemberId);

            List<int> memberVolunteerOrgz = new List<int>();
            if (memberVolunteerOrgz != null)
            {
                foreach (var memberOrgRole in memberOrgRoles)
                {
                    if (IsOrganizationMemberVolunteer(memberOrgRole))
                    {
                        memberVolunteerOrgz.Add(memberOrgRole.Organization.Id);
                    }
                }
            }
            var requestQueryable = (from dr in _context.DonationRequests
                                    join m in _context.Members on dr.CreatedBy equals m.Id
                                    join dro in _context.DonationRequestOrganizations on dr.Id equals dro.DonationRequestId
                                    join o in _context.Organizations on dro.OrganizationId equals o.Id
                                    join am in _context.Members on dro.ModeratorId equals am.Id into tam
                                    from am in tam.DefaultIfEmpty()
                                    join v in _context.Members on dro.VolunteerId equals v.Id into tv
                                    from v in tam.DefaultIfEmpty()
                                    where
                                    (filters.OrganizationId == null || dro.OrganizationId == filters.OrganizationId)
                                    && (filters.Types.Count == 0 || filters.Types.Contains((DonationRequestTypeCatalog)dr.Type))
                                    && (filters.Statuses.Count == 0 || filters.Statuses.Contains((StatusCatalog)dro.Status))
                                    && dr.IsDeleted == false
                                    && memberVolunteerOrgz.Any(x => x == o.Id)
                                    && (dro.Status >= (int)StatusCatalog.Approved)
                                    && (dro.VolunteerId == null || dro.VolunteerId == 0 || dro.VolunteerId == _loggedInMemberId)
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
                                            Moderator = new BaseBriefModel()
                                            {
                                                Id = am == null ? 0 : am.Id,
                                                Name = am == null ? "" : am.Name,
                                                NativeName = am == null ? "" : am.NativeName,
                                            },
                                            Volunteer = new BaseBriefModel()
                                            {
                                                Id = v == null ? 0 : v.Id,
                                                Name = v == null ? "" : v.Name,
                                                NativeName = v == null ? "" : v.NativeName,
                                            },
                                            Status = (StatusCatalog)dro.Status,
                                        },
                                        Type = (DonationRequestTypeCatalog)dr.Type,
                                        LoggedInMemberId = _loggedInMemberId,
                                        CreatedDate = dr.CreatedDate
                                    }).AsQueryable();

            return await requestQueryable.Paginate(filters);
        }
    }
}
