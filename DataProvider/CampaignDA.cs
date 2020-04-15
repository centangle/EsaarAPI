using DataProvider.Helpers;
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
        public async Task<int> CreateCampaign(CampaignModel model)
        {
            using (CharityEntities context = new CharityEntities())
            {
                try
                {
                    var dbModel = SetCampaign(new Campaign(), model);
                    context.Campaigns.Add(dbModel);
                    await context.SaveChangesAsync();
                    model.Id = dbModel.Id;
                    return model.Id;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        public async Task<bool> UpdateCampaign(CampaignModel model)
        {
            using (CharityEntities context = new CharityEntities())
            {
                Campaign dbModel = await context.Campaigns.Where(x => x.Id == model.Id && x.IsDeleted == false).FirstOrDefaultAsync();
                if (dbModel != null)
                {
                    SetCampaign(dbModel, model);
                    return await context.SaveChangesAsync() > 0;
                }
                return false;
            }

        }
        private Campaign SetCampaign(Campaign dbModel, CampaignModel model)
        {
            dbModel.Name = model.Name;
            dbModel.NativeName = model.NativeName;
            dbModel.Description = model.Description;
            dbModel.StartDate = model.StartDate;
            dbModel.EndDate = model.EndDate;
            ImageHelper.Save(model);
            dbModel.ImageUrl = model.ImageUrl;
            SetBaseProperties(dbModel, model);
            if (dbModel.Id == 0)
            {
                dbModel.IsActive = false;
            }
            return dbModel;

        }
    }
}
