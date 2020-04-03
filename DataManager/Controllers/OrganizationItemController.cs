using BusinessLogic;
using Catalogs;
using DataProvider;
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
    public class OrganizationItemController : BaseController
    {
        [HttpGet]
        public async Task<OrganizationItemModel> Get(int id)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.GetOrganizationItem(id);
        }
        public async Task<PaginatedResultModel<OrganizationItemPaginationModel>> GetPaginated(int recordsPerPage, int currentPage, PaginationOrderCatalog orderDir, bool disablePagination, string orderByColumn = null, bool calculateTotal = true, string itemName = null)
        {
            var _logic = new Logic(LoggedInMemberId);
            OrganizationItemSearchModel filters = new OrganizationItemSearchModel();
            filters.ItemName = itemName;
            SetPaginationProperties(filters, recordsPerPage, currentPage, orderDir, orderByColumn, disablePagination, calculateTotal);
            return await _logic.GetOrganizationItems(filters);
        }

        [HttpPost]
        public async Task<int> Create(OrganizationItemModel model)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.CreateOrganizationItem(model);
        }
        [HttpPut]
        public async Task<bool> Update(OrganizationItemModel model)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.UpdateOrganizationItem(model);
        }
        [HttpPost]
        public async Task<bool> CreateMultipleOrganizationItem(List<OrganizationItemModel> items)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.CreateMultipleOrganizationItem(items);
        }
        [HttpPut]
        public async Task<bool> UpdateMultipleOrganizationItem(List<OrganizationItemModel> items)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.UpdateMultipleOrganizationItem(items);
        }
        [HttpDelete]
        public async Task<bool> Delete(int id)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.DeleteOrganizationItems(new List<int> { id });
        }
        [HttpDelete]
        public async Task<bool> DeleteMultipleOrganizationItem(List<int> ids)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.DeleteOrganizationItems(ids);
        }


    }
}
