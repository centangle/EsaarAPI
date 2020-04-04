using Catalogs;
using DataProvider.Helpers;
using Helpers;
using Models;
using Models.BriefModel;
using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DataProvider
{
    public partial class DataAccess
    {
        public async Task<int> AddRequestThread(RequestThreadModel model)
        {
            using (CharityEntities context = new CharityEntities())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var modelId = await AddRequestThread(context, model);

                        foreach (var newAttachment in model.Attachments)
                        {
                            var attachment = await context.Attachments.Where(x => x.Url == newAttachment.Url).FirstOrDefaultAsync();
                            if (attachment != null)
                            {
                                attachment.EntityId = model.Id;
                                attachment.EntityType = (int)AttachmentEntityTypeCatalog.Request;
                            }
                        }
                        await context.SaveChangesAsync();
                        transaction.Commit();
                        return modelId;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
        }
        private async Task<int> AddRequestThread(CharityEntities context, RequestThreadModel model)
        {
            var dbModel = SetRequestThread(new RequestThread(), model);
            context.RequestThreads.Add(dbModel);
            await context.SaveChangesAsync();
            model.Id = dbModel.Id;
            return model.Id;
        }
        public async Task<bool> UpdateRequestThread(RequestThreadModel model)
        {
            using (CharityEntities context = new CharityEntities())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        RequestThread dbModel = await context.RequestThreads.Where(x => x.Id == model.Id && x.IsDeleted == false).FirstOrDefaultAsync();
                        if (dbModel != null)
                        {
                            SetRequestThread(dbModel, model);
                            var currentAttachments = await context.Attachments.Where(x => x.EntityId == model.Id && x.IsDeleted == false).ToListAsync();
                            var deletedAttachments = currentAttachments.Where(ca => !model.Attachments.Any(na => na.Url == ca.Url));
                            var newAtatchments = model.Attachments.Where(ca => !currentAttachments.Any(na => na.Url == ca.Url));
                            foreach (var newAttachment in newAtatchments)
                            {
                                var attachment = await context.Attachments.Where(x => x.Url == newAttachment.Url && x.IsDeleted == false).FirstOrDefaultAsync();
                                if (attachment != null)
                                {
                                    attachment.EntityId = model.Id;
                                    attachment.EntityType = (int)AttachmentEntityTypeCatalog.Request;
                                }
                            }
                            foreach (var attachment in deletedAttachments)
                            {
                                attachment.IsDeleted = true;
                            }
                            await context.SaveChangesAsync();
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
        }
        public async Task<bool> DeleteRequestThread(int id)
        {
            using (CharityEntities context = new CharityEntities())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        RequestThread dbModel = await context.RequestThreads.Where(x => x.Id == id && x.IsDeleted == false).FirstOrDefaultAsync();
                        if (dbModel != null)
                        {
                            dbModel.IsDeleted = true;
                            var currentAttachments = await context.Attachments.Where(x => x.EntityId == id && x.IsDeleted == false).ToListAsync();
                            foreach (var attachment in currentAttachments)
                            {
                                attachment.IsDeleted = true;
                            }
                            await context.SaveChangesAsync();
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
            SetBaseProperties(dbModel, model);
            return dbModel;
        }
        private RequestThreadModel GetRequestThreadModel(OrganizationRequestModel model)
        {
            RequestThreadModel requestThreadModel = new RequestThreadModel();
            requestThreadModel.Entity.Id = model.Id;
            requestThreadModel.EntityType = RequestThreadEntityTypeCatalog.Organization;
            requestThreadModel.Status = RequestThreadStatusCatalog.Initiated;
            requestThreadModel.Note = model.Note;
            requestThreadModel.Type = RequestThreadTypeCatalog.General;
            requestThreadModel.IsSystemGenerated = true;
            return requestThreadModel;
        }
        public async Task<RequestThreadModel> GetRequestThread(int id)
        {
            using (CharityEntities context = new CharityEntities())
            {
                var requestThread = await (from rt in context.RequestThreads
                                           join m in context.Members on rt.CreatedBy equals m.Id
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
                                               Status = (RequestThreadStatusCatalog)rt.Status,
                                               Note = rt.Note,
                                               Type = (RequestThreadTypeCatalog)rt.Type,
                                               IsSystemGenerated = rt.IsSystemGenerated,
                                               CreatedDate = rt.CreatedDate
                                           }).FirstOrDefaultAsync();
                requestThread.Attachments = await (from a in context.Attachments
                                                   where a.EntityId == requestThread.Entity.Id
                                                   && a.EntityType == (int)requestThread.EntityType
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
                return requestThread;

            }
        }
        public async Task<PaginatedResultModel<RequestThreadModel>> GetRequestThreads(RequestThreadSearchModel filters)
        {
            using (CharityEntities context = new CharityEntities())
            {
                var orgItemQueryable = (from rt in context.RequestThreads
                                        join m in context.Members on rt.CreatedBy equals m.Id
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
                                            },
                                            EntityType = (RequestThreadEntityTypeCatalog)rt.EntityType,
                                            Status = (RequestThreadStatusCatalog)rt.Status,
                                            Note = rt.Note,
                                            Type = (RequestThreadTypeCatalog)rt.Type,
                                            IsSystemGenerated = rt.IsSystemGenerated,
                                            CreatedDate = rt.CreatedDate
                                        }).AsQueryable();
                return await orgItemQueryable.Paginate(filters);
            }

        }
    }
}
