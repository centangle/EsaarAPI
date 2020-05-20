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
        private readonly Logic _logic;

        public EventController(Logic logic)
        {
            _logic = logic;
        }
        [HttpGet]
        [Route("Get")]
        [Authorize(Roles = "Admin")]
        public async Task<EventModel> Get(int id)
        {
            
            return await _logic.GetEvent(id);
        }
        [HttpGet]
        [Route("GetPaginated")]
        [Authorize(Roles = "Member,Admin")]
        public async Task<PaginatedResultModel<EventModel>> GetPaginated(int recordsPerPage, int currentPage, PaginationOrderCatalog orderDir, bool disablePagination, string name = null, bool? isActive = null, string orderByColumn = null, bool calculateTotal = true)
        {
            
            EventSearchModel filters = new EventSearchModel();
            filters.Name = name;
            filters.IsActive = isActive;
            SetPaginationProperties(filters, recordsPerPage, currentPage, orderDir, orderByColumn, disablePagination, calculateTotal);
            return await _logic.GetEvents(filters);
        }
        [HttpPost]
        [Route("Create")]
        [Authorize(Roles = "Admin")]
        public async Task<int> Create(EventModel model)
        {
            
            return await _logic.CreateEvent(model);
        }
        [HttpPost]
        [Route("Update")]
        [Authorize(Roles = "Admin")]
        public async Task<bool> Update(EventModel model)
        {
            
            return await _logic.UpdateEvent(model);
        }
        [HttpDelete]
        [Route("Delete")]
        [Authorize(Roles = "Admin")]
        public async Task<bool> Delete(int id)
        {
            
            return await _logic.DeleteEvent(id);
        }
    }
}