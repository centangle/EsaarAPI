using Catalogs;
using Helpers;
using Models;
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
        public async Task<int> CreateAttachment(AttachmentModel model)
        {
            using (CharityEntities context = new CharityEntities())
            {
                Attachment dbModel = new Attachment();
                SetAttachment(dbModel, model);
                context.Attachments.Add(dbModel);
                bool result = await context.SaveChangesAsync() > 0;
                model.Id = dbModel.Id;
                return model.Id;
            }
        }
        public async Task<bool> UpdateAttachment(AttachmentModel model)
        {
            using (CharityEntities context = new CharityEntities())
            {
                try
                {
                    Attachment dbModel = await context.Attachments.Where(x => x.Id == model.Id && x.IsDeleted == false).FirstOrDefaultAsync();
                    if (dbModel != null)
                    {
                        SetAttachment(dbModel, model);
                        return await context.SaveChangesAsync() > 0;
                    }
                    return false;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

        public async Task<bool> DeleteAttachment(string url)
        {
            using (CharityEntities context = new CharityEntities())
            {
                var attachment = await context.Attachments.Where(x => x.Url == url).FirstOrDefaultAsync();
                if (attachment != null)
                {
                    attachment.IsDeleted = true;
                    return await context.SaveChangesAsync() > 0;
                }
                return false;
            }
        }
        private Attachment SetAttachment(Attachment dbModel, AttachmentModel model)
        {
            SetBaseProperties(dbModel, model);
            if (dbModel.Id != 0)
            {
                if (model.Entity == null || model.Entity.Id < 1)
                {
                    throw new KnownException("Entity is required.");
                }
                dbModel.EntityId = model.Entity.Id;
                dbModel.EntityType = (int)model.EntityType;
            }
            else
            {
                dbModel.EntityId = 0;
                dbModel.EntityType = 0;
            }

            model.Note = dbModel.Note;
            dbModel.Url = model.Url;
            dbModel.SystemFileName = model.SystemFileName;
            dbModel.OriginalFileName = model.OriginalFileName;
            dbModel.FileExtension = model.FileExtension;
            return dbModel;
        }

        private async Task AssignAttachments(CharityEntities context, List<AttachmentModel> attachments, int entityId, bool isNew)
        {
            if (attachments != null)
            {
                if (isNew)
                {

                    if (attachments != null)
                    {
                        foreach (var newAttachment in attachments)
                        {
                            var attachment = await context.Attachments.Where(x => x.Url == newAttachment.Url).FirstOrDefaultAsync();
                            if (attachment != null)
                            {
                                attachment.EntityId = entityId;
                                attachment.EntityType = (int)AttachmentEntityTypeCatalog.Request;
                            }
                        }
                    }
                }
                else
                {
                    var currentAttachments = await context.Attachments.Where(x => x.EntityId == entityId && x.IsDeleted == false).ToListAsync();
                    var deletedAttachments = currentAttachments.Where(ca => !attachments.Any(na => na.Url == ca.Url));
                    var newAtatchments = attachments.Where(ca => !currentAttachments.Any(na => na.Url == ca.Url));
                    foreach (var newAttachment in newAtatchments)
                    {
                        var attachment = await context.Attachments.Where(x => x.Url == newAttachment.Url && x.IsDeleted == false).FirstOrDefaultAsync();
                        if (attachment != null)
                        {
                            attachment.EntityId = entityId;
                            attachment.EntityType = (int)AttachmentEntityTypeCatalog.Request;
                        }
                    }
                    foreach (var attachment in deletedAttachments)
                    {
                        attachment.IsDeleted = true;
                    }
                }
            }
        }
    }
}
