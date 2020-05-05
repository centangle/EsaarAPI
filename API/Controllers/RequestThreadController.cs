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
    public class RequestThreadController : BaseController
    {
        private readonly Logic _logic;

        public RequestThreadController(Logic logic)
        {
            _logic = logic;
        }
        [HttpGet]
        [Route("Get")]
        public async Task<RequestThreadModel> Get(int id)
        {
            
            return await _logic.GetRequestThread(id);
        }
        [HttpGet]
        [Route("GetPaginated")]
        public async Task<PaginatedResultModel<RequestThreadModel>> GetPaginated(int recordsPerPage, int currentPage, PaginationOrderCatalog orderDir, bool disablePagination, int entityId, RequestThreadEntityTypeCatalog entityType, RequestThreadTypeCatalog type, string orderByColumn = null, bool calculateTotal = true)
        {
            RequestThreadSearchModel filters = new RequestThreadSearchModel();
            filters.EntityId = entityId;
            filters.EntityType = entityType;
            filters.Type = type;
            SetPaginationProperties(filters, recordsPerPage, currentPage, orderDir, orderByColumn, disablePagination, calculateTotal);
            
            return await _logic.GetRequestThreads(filters);
        }

        [HttpPost]
        [Route("AddRequestThread")]
        public async Task<int> AddRequestThread(RequestThreadModel model)
        {
            
            model.IsSystemGenerated = false;
            return await _logic.AddRequestThread(model);
        }

        [HttpPut]
        [Route("UpdateRequestThread")]
        public async Task<bool> UpdateRequestThread(RequestThreadModel model)
        {
            
            return await _logic.UpdateRequestThread(model);
        }
        [HttpDelete]
        [Route("DeleteRequestThread")]
        public async Task<bool> DeleteRequestThread(int id)
        {
            
            return await _logic.DeleteRequestThread(id);
        }
    }
}