using Catalogs;
using Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DataManager.Controllers
{
    public class ItemController : BaseController
    {
        [HttpGet]
        public async Task<ItemModel> Get(int id)
        {
            return await logic.GetItem(id);
        }
        [HttpGet]
        public async Task<IEnumerable<ItemModel>> GetSingleItemTree(int id, DataStructureCatalog dataStructure)
        {
            bool getHierarichalData = (dataStructure == DataStructureCatalog.List ? false : true);
            return await logic.GetSingleItemHierarchy(id, getHierarichalData);
        }

        [HttpGet]
        public async Task<IEnumerable<ItemModel>> GetPeripheralItems()
        {
            return await logic.GetPeripheralItems();
        }

        [HttpGet]
        public async Task<IEnumerable<ItemModel>> GetRootItems()
        {
            return await logic.GetRootItems();
        }

        [HttpGet]
        public async Task<IEnumerable<ItemModel>> GetAllItems(DataStructureCatalog dataStructure)
        {
            bool getHierarichalData = (dataStructure == DataStructureCatalog.List ? false : true);
            return await logic.GetAllItems(getHierarichalData);
        }
        [HttpPost]
        public async Task<int> Create(ItemModel model)
        {
            return await logic.CreateItem(model);
        }
        [HttpPut]
        public async Task<bool> Update(ItemModel model)
        {
            return await logic.UpdateItem(model);
        }
        [HttpPost]
        public async Task<int> CreateSingleItemWithChildrens(ItemModel model)
        {
            return await logic.CreateSingleItemWithChildrens(model);
        }
        [HttpPut]
        public async Task<bool> UpdateSingleItemWithChildren(ItemModel model)
        {
            return await logic.UpdateSingleItemWithChildren(model);
        }
        [HttpPost]
        public async Task<bool> CreateMultipleItemsWithChildrens(List<ItemModel> items)
        {
            return await logic.CreateMultipleItemsWithChildrens(items);
        }
        [HttpPut]
        public async Task<bool> UpdateMultipleItemsWithChildrens(List<ItemModel> items)
        {
            return await logic.UpdateMultipleItemsWithChildrens(items);
        }

        [HttpDelete]
        public async Task<bool> DeleteItemWithChildren(int id)
        {
            return await logic.DeleteItem(id);
        }
    }
}
