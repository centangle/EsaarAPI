using BusinessLogic;
using Catalogs;
using Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;


namespace DataManager.Controllers
{
    [Authorize]
    public class ItemController : BaseController
    {
        [HttpGet]
        public async Task<ItemModel> Get(int id)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.GetItem(id);
        }
        [HttpGet]
        public async Task<IEnumerable<ItemModel>> GetSingleItemTree(int id, DataStructureCatalog dataStructure)
        {
            var _logic = new Logic(LoggedInMemberId);
            bool getHierarichalData = (dataStructure == DataStructureCatalog.List ? false : true);
            return await _logic.GetSingleItemHierarchy(id, getHierarichalData);
        }

        [HttpGet]
        public async Task<IEnumerable<ItemModel>> GetPeripheralItems()
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.GetPeripheralItems();
        }

        [HttpGet]
        public async Task<IEnumerable<ItemModel>> GetRootItems()
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.GetRootItems();
        }

        [HttpGet]
        public async Task<IEnumerable<ItemModel>> GetAllItems(DataStructureCatalog dataStructure)
        {
            var _logic = new Logic(LoggedInMemberId);
            bool getHierarichalData = (dataStructure == DataStructureCatalog.List ? false : true);
            return await _logic.GetAllItems(getHierarichalData);
        }
        [HttpPost]
        public async Task<int> Create(ItemModel model)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.CreateItem(model);
        }
        [HttpPut]
        public async Task<bool> Update(ItemModel model)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.UpdateItem(model);
        }
        [HttpPost]
        public async Task<bool> CreateSingleItemWithChildrens(ItemModel model)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.CreateSingleItemWithChildrens(model);
        }
        [HttpPut]
        public async Task<bool> UpdateSingleItemWithChildren(ItemModel model)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.UpdateSingleItemWithChildren(model);
        }
        [HttpPost]
        public async Task<bool> CreateMultipleItemsWithChildrens(List<ItemModel> items)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.CreateMultipleItemsWithChildrens(items);
        }
        [HttpPut]
        public async Task<bool> UpdateMultipleItemsWithChildrens(List<ItemModel> items)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.UpdateMultipleItemsWithChildrens(items);
        }

        [HttpDelete]
        public async Task<bool> DeleteItemWithChildren(int id)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.DeleteItem(id);
        }
    }
}
