using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic;
using Catalogs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EventController : BaseController
    {
        [HttpGet]
        [Route("Get")]
        public async Task<EventModel> Get(int id)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.GetEvent(id);
        }
        [HttpGet]
        [Route("GetPaginated")]
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
        [Route("Create")]
        public async Task<int> Create(EventModel model)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.CreateEvent(model);
        }
        [HttpPost]
        [Route("Update")]
        public async Task<bool> Update(EventModel model)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.UpdateEvent(model);
        }
        [HttpDelete]
        [Route("Delete")]
        public async Task<bool> Delete(int id)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.DeleteEvent(id);
        }
    }
}