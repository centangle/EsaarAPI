using DataProvider.Helpers;
using Models;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DataProvider
{
    public partial class DataAccess
    {
        public async Task<int> CreateEvent(EventModel model)
        {
            using (CharityEntities context = new CharityEntities())
            {
                var dbModel = SetEvent(new Event(), model);
                context.Events.Add(dbModel);
                await context.SaveChangesAsync();
                model.Id = dbModel.Id;
                return model.Id;
            }
        }
        public async Task<bool> UpdateEvent(EventModel model)
        {
            using (CharityEntities context = new CharityEntities())
            {
                Event dbModel = await context.Events.Where(x => x.Id == model.Id && x.IsDeleted == false).FirstOrDefaultAsync();
                if (dbModel != null)
                {
                    SetEvent(dbModel, model);
                    return await context.SaveChangesAsync() > 0;
                }
                return false;
            }

        }
        public async Task<bool> DeleteEvent(int id)
        {
            using (CharityEntities context = new CharityEntities())
            {
                var dbModel = await context.Events.Where(x => x.Id == id && x.IsDeleted == false).FirstOrDefaultAsync();
                if (dbModel != null)
                {
                    dbModel.IsDeleted = true;
                    return await context.SaveChangesAsync() > 0;
                }
                return false;
            }
        }
        public async Task<EventModel> GetEvent(int id)
        {
            using (CharityEntities context = new CharityEntities())
            {
                return await (from e in context.Events
                              where e.Id == id
                              && e.IsDeleted == false
                              select new EventModel
                              {
                                  Id = e.Id,
                                  Name = e.Name,
                                  NativeName = e.NativeName,
                                  Description = e.Description,
                                  ImageUrl = e.ImageUrl,
                                  IsActive = e.IsActive,
                                  StartDate = e.StartDate,
                                  EndDate = e.EndDate,
                              }).FirstOrDefaultAsync();
            }
        }
        public async Task<PaginatedResultModel<EventModel>> GetEvents(EventSearchModel filters)
        {
            using (CharityEntities context = new CharityEntities())
            {
                var eventQueryable = (from e in context.Events
                                      where
                                      (
                                        string.IsNullOrEmpty(filters.Name)
                                        || e.Name.Contains(filters.Name)
                                        || e.NativeName.Contains(filters.Name)
                                      )
                                      && (filters.IsActive == null || e.IsActive == filters.IsActive)
                                      && e.IsDeleted == false
                                      select new EventModel
                                      {
                                          Id = e.Id,
                                          Name = e.Name,
                                          NativeName = e.NativeName,
                                          Description = e.Description,
                                          ImageUrl = e.ImageUrl,
                                          IsActive = e.IsActive,
                                          StartDate = e.StartDate,
                                          EndDate = e.EndDate,
                                      }).AsNoTracking().AsQueryable();
                return await eventQueryable.Paginate(filters);
            }
        }
        private Event SetEvent(Event dbModel, EventModel model)
        {
            dbModel.Name = model.Name;
            dbModel.NativeName = model.NativeName;
            dbModel.Description = model.Description;
            dbModel.StartDate = model.StartDate;
            dbModel.EndDate = model.EndDate;
            ImageHelper.Save(model);
            dbModel.ImageUrl = model.ImageUrl;
            SetAndValidateBaseProperties(dbModel, model);
            if (dbModel.Id == 0)
            {
                dbModel.IsActive = false;
            }
            return dbModel;
        }
    }
}
