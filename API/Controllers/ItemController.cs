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
        public async Task<ItemModel> Get(int id)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.GetItem(id);
        }
        [HttpGet]
        [Route("GetSingleItemTree")]
        public async Task<IEnumerable<ItemModel>> GetSingleItemTree(int id, DataStructureCatalog dataStructure)
        {
            var _logic = new Logic(LoggedInMemberId);
            bool getHierarichalData = (dataStructure == DataStructureCatalog.List ? false : true);
            return await _logic.GetSingleItemHierarchy(id, getHierarichalData);
        }
        [HttpGet]
        [Route("GetPeripheralItems")]
        public async Task<IEnumerable<ItemModel>> GetPeripheralItems(int? organizationId = null)
        {
            return await _logic.GetPeripheralItems(organizationId);
        }
        [HttpGet]
        [Route("GetRootItems")]
        public async Task<IEnumerable<ItemModel>> GetRootItems()
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.GetRootItems();
        }
        [HttpGet]
        [Route("GetAllItems")]
        public async Task<IEnumerable<ItemModel>> GetAllItems(DataStructureCatalog dataStructure)
        {
            var _logic = new Logic(LoggedInMemberId);
            bool getHierarichalData = (dataStructure == DataStructureCatalog.List ? false : true);
            return await _logic.GetAllItems(getHierarichalData);
        }
        [HttpPost]
        [Route("Create")]
        public async Task<int> Create(ItemModel model)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.CreateItem(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<bool> Update(ItemModel model)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.UpdateItem(model);
        }
        [HttpPost]
        [Route("CreateSingleItemWithChildrens")]
        public async Task<bool> CreateSingleItemWithChildrens(ItemModel model)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.CreateSingleItemWithChildrens(model);
        }
        [HttpPut]
        [Route("UpdateSingleItemWithChildren")]
        public async Task<bool> UpdateSingleItemWithChildren(ItemModel model)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.UpdateSingleItemWithChildren(model);
        }
        [HttpPost]
        [Route("CreateMultipleItemsWithChildrens")]
        public async Task<bool> CreateMultipleItemsWithChildrens(List<ItemModel> items)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.CreateMultipleItemsWithChildrens(items);
        }
        [HttpPut]
        [Route("UpdateMultipleItemsWithChildrens")]
        public async Task<bool> UpdateMultipleItemsWithChildrens(List<ItemModel> items)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.UpdateMultipleItemsWithChildrens(items);
        }
        [HttpDelete]
        [Route("DeleteItemWithChildren")]
        public async Task<bool> DeleteItemWithChildren(int id)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.DeleteItem(id);
        }
    }
}