using BusinessLogic;
using Catalogs;
using Models;
using System.Threading.Tasks;
using System.Web.Http;

namespace DataManager.Controllers
{
    [Authorize]
    public class EventController : BaseController
    {
        public async Task<EventModel> Get(int id)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.GetEvent(id);
        }
        public async Task<PaginatedResultModel<EventModel>> GetPaginated(int recordsPerPage, int currentPage, PaginationOrderCatalog orderDir, bool disablePagination, string name = null, bool? isActive = null, string orderByColumn = null, bool calculateTotal = true)
        {
            var _logic = new Logic(LoggedInMemberId);
            EventSearchModel filters = new EventSearchModel();
            filters.Name = name;
            filters.IsActive = isActive;
            SetPaginationProperties(filters, recordsPerPage, currentPage, orderDir, orderByColumn, disablePagination, calculateTotal);
            return await _logic.GetEvents(filters);
        }
        [HttpPost]
        public async Task<int> Create(EventModel model)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.CreateEvent(model);
        }
        [HttpPost]
        public async Task<bool> Update(EventModel model)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.UpdateEvent(model);
        }
        [HttpDelete]
        public async Task<bool> Delete(int id)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.DeleteEvent(id);
        }

    }
}
