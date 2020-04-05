using BusinessLogic;
using Catalogs;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace DataManager.Controllers
{
    [Authorize]
    public class RequestThreadController : BaseController
    {
        [HttpGet]
        public async Task<RequestThreadModel> Get(int id)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.GetRequestThread(id);
        }
        [HttpGet]
        public async Task<PaginatedResultModel<RequestThreadModel>> GetPaginated(int recordsPerPage, int currentPage, PaginationOrderCatalog orderDir, bool disablePagination, int entityId, RequestThreadEntityTypeCatalog entityType, RequestThreadTypeCatalog type, string orderByColumn = null, bool calculateTotal = true)
        {
            RequestThreadSearchModel filters = new RequestThreadSearchModel();
            filters.EntityId = entityId;
            filters.EntityType = entityType;
            filters.Type = type;
            SetPaginationProperties(filters, recordsPerPage, currentPage, orderDir, orderByColumn, disablePagination, calculateTotal);
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.GetRequestThreads(filters);
        }

        [HttpPost]
        public async Task<int> AddRequestThread(RequestThreadModel model)
        {
            var _logic = new Logic(LoggedInMemberId);
            model.IsSystemGenerated = false;
            return await _logic.AddRequestThread(model);
        }

        [HttpPut]
        public async Task<bool> UpdateRequestThread(RequestThreadModel model)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.UpdateRequestThread(model);
        }
        [HttpDelete]
        public async Task<bool> DeleteRequestThread(int id)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.DeleteRequestThread(id);
        }
    }
}
