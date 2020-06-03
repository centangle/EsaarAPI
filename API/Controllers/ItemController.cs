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
    public class ItemController : BaseController
    {
        private readonly Logic _logic;

        public ItemController(Logic logic)
        {
            _logic = logic;
        }
        [HttpGet]
        [Route("Get")]
        [Authorize(Roles = "Admin")]
        public async Task<ItemModel> Get(int id)
        {

            return await _logic.GetItem(id);
        }
        [HttpGet]
        [Route("GetSingleItemTree")]
        [Authorize(Roles = "Admin")]
        public async Task<IEnumerable<ItemModel>> GetSingleItemTree(int id, DataStructureCatalog dataStructure)
        {

            bool getHierarichalData = (dataStructure == DataStructureCatalog.List ? false : true);
            return await _logic.GetSingleItemHierarchy(id, getHierarichalData);
        }
        [HttpGet]
        [Route("GetPeripheralItems")]
        [Authorize(Roles = "Member,Admin")]
        public async Task<IEnumerable<ItemModel>> GetPeripheralItems(int? organizationId = null)
        {
            return await _logic.GetPeripheralItems(organizationId);
        }
        [HttpGet]
        [Route("GetPeripheralItemsPaginated")]
        [Authorize(Roles = "Member,Admin")]
        public async Task<PaginatedResultModel<ItemModel>> GetPeripheralItemsPaginated(int recordsPerPage, int currentPage, PaginationOrderCatalog orderDir, bool disablePagination, string itemName = null, [FromQuery] List<int> rootCategories = null, string orderByColumn = null, bool calculateTotal = true)
        {
            ItemSearchModel filters = new ItemSearchModel();
            filters.Name = itemName;
            filters.RootCategories = rootCategories ?? new List<int>();
            SetPaginationProperties(filters, recordsPerPage, currentPage, orderDir, orderByColumn, disablePagination, calculateTotal);
            return await _logic.GetPeripheralItemsPaginated(filters);
        }
        [HttpGet]
        [Route("GetRootItems")]
        [Authorize(Roles = "Member,Admin")]
        public async Task<IEnumerable<ItemModel>> GetRootItems()
        {

            return await _logic.GetRootItems();
        }
        [HttpGet]
        [Route("GetAllItems")]
        [Authorize(Roles = "Admin")]
        public async Task<IEnumerable<ItemModel>> GetAllItems(DataStructureCatalog dataStructure)
        {

            bool getHierarichalData = (dataStructure == DataStructureCatalog.List ? false : true);
            return await _logic.GetAllItems(getHierarichalData);
        }
        [HttpPost]
        [Route("Create")]
        [Authorize(Roles = "Admin")]
        public async Task<int> Create(ItemModel model)
        {

            return await _logic.CreateItem(model);
        }
        [HttpPut]
        [Route("Update")]
        [Authorize(Roles = "Admin")]
        public async Task<bool> Update(ItemModel model)
        {

            return await _logic.UpdateItem(model);
        }
        [HttpPost]
        [Route("CreateSingleItemWithChildrens")]
        [Authorize(Roles = "Admin")]
        public async Task<bool> CreateSingleItemWithChildrens(ItemModel model)
        {

            return await _logic.CreateSingleItemWithChildrens(model);
        }
        [HttpPut]
        [Route("UpdateSingleItemWithChildren")]
        [Authorize(Roles = "Admin")]
        public async Task<bool> UpdateSingleItemWithChildren(ItemModel model)
        {

            return await _logic.UpdateSingleItemWithChildren(model);
        }
        [HttpPost]
        [Route("CreateMultipleItemsWithChildrens")]
        [Authorize(Roles = "Admin")]
        public async Task<bool> CreateMultipleItemsWithChildrens(List<ItemModel> items)
        {

            return await _logic.CreateMultipleItemsWithChildrens(items);
        }
        [HttpPut]
        [Route("UpdateMultipleItemsWithChildrens")]
        [Authorize(Roles = "Admin")]
        public async Task<bool> UpdateMultipleItemsWithChildrens(List<ItemModel> items)
        {

            return await _logic.UpdateMultipleItemsWithChildrens(items);
        }
        [HttpDelete]
        [Route("DeleteItemWithChildren")]
        [Authorize(Roles = "Admin")]
        public async Task<bool> DeleteItemWithChildren(int id)
        {

            return await _logic.DeleteItem(id);
        }
    }
}