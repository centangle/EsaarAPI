using Catalogs;
using Helpers;
using Models;
using System.Threading.Tasks;

namespace DataProvider
{
    public partial class DataAccess
    {
        private async Task AddRequestThread(CharityEntities context, RequestThreadModel model)
        {
            var dbModel = SetRequestThread(new RequestThread(), model);
            context.RequestThreads.Add(dbModel);
            await context.SaveChangesAsync();
            model.Id = dbModel.Id;
        }
        private RequestThread SetRequestThread(RequestThread dbModel, RequestThreadModel model)
        {
            if (model.Entity == null || model.Entity.Id < 1)
            {
                throw new KnownException("Entity is required.");
            }
            dbModel.EntityId = model.Entity.Id;
            dbModel.EntityType = (int)model.EntityType;
            dbModel.Type = (int)model.Type;
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
            return requestThreadModel;
        }
    }
}
