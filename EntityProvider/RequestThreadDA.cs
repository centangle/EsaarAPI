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
        public async Task<int> AddRequestThread(RequestThreadModel model)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    int id = await ProcessRequestThread(_context, model);
                    transaction.Commit();
                    return id;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }

            }
        }
        public async Task<int> ProcessRequestThread(CharityContext _context, RequestThreadModel model)
        {
            try
            {
                var accessGranted = await IsRequestThreadAccessible(_context, model);
                if (accessGranted)
                {
                    try
                    {
                        int? currentStatus = await GetRequestThreadCurrentStatus(_context, model);
                        model.Id = await AddRequestThread(_context, model);
                        bool statusUpdated = await CheckStatusUpdation(_context, currentStatus, model);
                        await _context.SaveChangesAsync();
                        return model.Id;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                return 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private async Task<int?> GetRequestThreadCurrentStatus(CharityContext _context, RequestThreadModel model)
        {
            int? currentStatus = null;
            if (model.Status == null)
            {
                var lastThread = await _context.RequestThreads.Where(x => x.EntityId == model.Entity.Id && x.IsDeleted == false).OrderByDescending(x => x.CreatedDate).FirstOrDefaultAsync();
                if (lastThread != null)
                {
                    currentStatus = lastThread.Status;
                    model.Status = (StatusCatalog)currentStatus;
                }
            }
            return currentStatus;
        }
        private async Task<int> AddRequestThread(CharityContext _context, RequestThreadModel model)
        {
            var dbModel = SetRequestThread(new RequestThread(), model);
            _context.RequestThreads.Add(dbModel);
            await _context.SaveChangesAsync();
            model.Id = dbModel.Id;
            await AssignAttachments(_context, model.Attachments, model.Id, AttachmentEntityTypeCatalog.Request, true);
            return model.Id;
        }
        public async Task<bool> UpdateRequestThread(RequestThreadModel model)
        {
            var accessGranted = await IsRequestThreadAccessible(_context, model);
            if (accessGranted)
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        RequestThread dbModel = await _context.RequestThreads.Where(x => x.Id == model.Id && x.IsDeleted == false).FirstOrDefaultAsync();
                        if (dbModel != null)
                        {
                            var currentStatus = dbModel.Status;
                            SetRequestThread(dbModel, model);
                            await AssignAttachments(_context, model.Attachments, dbModel.Id, AttachmentEntityTypeCatalog.Request, false);
                            bool statusUpdated = await CheckStatusUpdation(_context, currentStatus, model);
                            await _context.SaveChangesAsync();
                        }
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
            return false;
        }
        public async Task<bool> DeleteRequestThread(int id)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    RequestThread dbModel = await _context.RequestThreads.Where(x => x.Id == id && x.IsDeleted == false).FirstOrDefaultAsync();
                    if (dbModel != null)
                    {
                        dbModel.IsDeleted = true;
                        var currentAttachments = await _context.Attachments.Where(x => x.EntityId == id && x.IsDeleted == false).ToListAsync();
                        foreach (var attachment in currentAttachments)
                        {
                            attachment.IsDeleted = true;
                        }
                        await _context.SaveChangesAsync();
                    }
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
        private RequestThread SetRequestThread(RequestThread dbModel, RequestThreadModel model)
        {
            if (dbModel.Id == 0)
            {
                if (model.Entity == null || model.Entity.Id < 1)
                {
                    throw new KnownException("Entity is required.");
                }
                dbModel.EntityId = model.Entity.Id;
                dbModel.EntityType = (int)model.EntityType;
                dbModel.Type = (int)model.Type;
                dbModel.IsSystemGenerated = model.IsSystemGenerated;
            }
            if (model.Status != null)
                dbModel.Status = (int)model.Status;
            dbModel.Note = model.Note;
            SetAndValidateBaseProperties(dbModel, model);
            return dbModel;
        }
        public async Task<RequestThreadModel> GetRequestThread(int id)
        {
            var requestThread = await (from rt in _context.RequestThreads
                                       join m in _context.Members on rt.CreatedBy equals m.Id
                                       where rt.Id == id
                                       && rt.IsDeleted == false
                                       select new RequestThreadModel
                                       {
                                           Id = rt.Id,
                                           Creator = new BaseBriefModel()
                                           {
                                               Id = m.Id,
                                               Name = m.Name,
                                               NativeName = m.NativeName,
                                           },
                                           Entity = new BaseBriefModel()
                                           {
                                               Id = rt.EntityId,
                                           },
                                           EntityType = (RequestThreadEntityTypeCatalog)rt.EntityType,
                                           Status = (StatusCatalog)rt.Status,
                                           Note = rt.Note,
                                           Type = (RequestThreadTypeCatalog)rt.Type,
                                           IsSystemGenerated = rt.IsSystemGenerated,
                                           CreatedDate = rt.CreatedDate
                                       }).FirstOrDefaultAsync();
            requestThread.Attachments = await (from a in _context.Attachments
                                               where a.EntityId == requestThread.Id
                                               && a.EntityType == (int)AttachmentEntityTypeCatalog.Request
                                               && a.IsDeleted == false
                                               select new AttachmentModel
                                               {
                                                   Id = a.Id,
                                                   Entity = new BaseBriefModel()
                                                   {
                                                       Id = a.EntityId,
                                                   },
                                                   Url = a.Url,
                                                   Note = a.Note,
                                                   OriginalFileName = a.OriginalFileName,
                                                   SystemFileName = a.SystemFileName,
                                                   FileExtension = a.FileExtension,
                                               }).ToListAsync();
            if (await IsRequestThreadAccessible(_context, requestThread))
            {
                return requestThread;
            }
            else
            {
                throw new KnownException("You are not authorized");
            }

        }
        public async Task<PaginatedResultModel<RequestThreadModel>> GetRequestThreads(RequestThreadSearchModel filters)
        {
            var requestQueryable = (from rt in _context.RequestThreads select rt).AsQueryable();
            if (filters.EntityType == RequestThreadEntityTypeCatalog.Organization)
            {
                requestQueryable = (from rt in requestQueryable
                                    join ort in _context.OrganizationRequests on rt.EntityId equals ort.Id
                                    join o in _context.Organizations on ort.OrganizationId equals o.Id
                                    where
                                         ort.CreatedBy == _loggedInMemberId
                                         ||
                                         o.OwnedBy == _loggedInMemberId
                                         ||
                                         ort.ModeratorId == _loggedInMemberId
                                    select rt).AsQueryable();
            }
            else if (filters.EntityType == RequestThreadEntityTypeCatalog.Donation)
            {
                requestQueryable = (from rt in requestQueryable
                                    join dro in _context.DonationRequestOrganizations on rt.EntityId equals dro.Id
                                    join dr in _context.DonationRequests on dro.DonationRequestId equals dr.Id
                                    join o in _context.Organizations on dro.OrganizationId equals o.Id
                                    where
                                         dr.MemberId == _loggedInMemberId
                                         ||
                                         o.OwnedBy == _loggedInMemberId
                                         ||
                                         dro.ModeratorId == _loggedInMemberId
                                    select rt).AsQueryable();
            }
            var requestThreadQueryable = (from rt in requestQueryable
                                          join m in _context.Members on rt.CreatedBy equals m.Id
                                          where rt.EntityId == filters.EntityId
                                          && rt.EntityType == (int)filters.EntityType
                                          && rt.Type == (int)filters.Type
                                          && rt.IsDeleted == false
                                          select new RequestThreadModel
                                          {
                                              Id = rt.Id,
                                              Creator = new BaseBriefModel()
                                              {
                                                  Id = m.Id,
                                                  Name = m.Name,
                                                  NativeName = m.NativeName,
                                              },
                                              Entity = new BaseBriefModel()
                                              {
                                                  Id = rt.EntityId,
                                                  Name = "",
                                                  NativeName = "",
                                              },
                                              EntityType = (RequestThreadEntityTypeCatalog)rt.EntityType,
                                              Status = (StatusCatalog)rt.Status,
                                              Note = rt.Note,
                                              Type = (RequestThreadTypeCatalog)rt.Type,
                                              IsSystemGenerated = rt.IsSystemGenerated,
                                              CreatedDate = rt.CreatedDate
                                          }).AsQueryable();
            return await requestThreadQueryable.Paginate(filters);
        }
        private async Task<bool> CheckStatusUpdation(CharityContext _context, int? currentStatus, RequestThreadModel model)
        {
            bool result = false;
            if (model.Status != null && currentStatus != (int)model.Status)
            {
                result = await ChangeRequestEntityStatus(_context, model);
            }
            return result;
        }
        private async Task<bool> ChangeRequestEntityStatus(CharityContext _context, RequestThreadModel model)
        {
            bool result = false;
            switch (model.EntityType)
            {
                case RequestThreadEntityTypeCatalog.Organization:
                    result = await ChangeOrganizationRequestStatus(_context, model);
                    break;
                case RequestThreadEntityTypeCatalog.Donation:
                    result = await ChangeDonationRequestStatus(_context, model);
                    break;

            }
            return result;
        }
        private async Task<bool> IsRequestThreadAccessible(CharityContext _context, RequestThreadModel model)
        {
            bool result = false;
            switch (model.EntityType)
            {
                case RequestThreadEntityTypeCatalog.Organization:
                    result = await IsOrganizationRequestThreadAccessible(_context, model);
                    break;
                case RequestThreadEntityTypeCatalog.Donation:
                    result = await IsDonationRequestThreadAccessible(_context, model);
                    break;
            }
            return result;
        }
    }
}
