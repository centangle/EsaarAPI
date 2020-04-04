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
    }
}
