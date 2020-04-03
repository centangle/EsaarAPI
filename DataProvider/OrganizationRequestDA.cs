using Helpers;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProvider
{
    public partial class DataAccess
    {
        private async Task<int> AddOrganizationRequest(OrganizationRequestModel model)
        {

            using (CharityEntities context = new CharityEntities())
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
            SetBaseProperties(dbModel, model);
            return dbModel;
        }
    }
}
