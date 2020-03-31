using Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataManager.Controllers
{
    public class ItemController : BaseController
    {
        public async Task<ItemModel> Get(int id)
        {
            return await logic.GetItem(id);
        }

        public async Task<IEnumerable<ItemModel>> GetSingleItemTree(int id)
        {
            return await logic.GetSingleItemHierarchy(id);
        }

        public async Task<int> Create(ItemModel model)
        {
            return await logic.CreateItem(model);
        }

        public async Task<bool> Update(ItemModel model)
        {
            return await logic.UpdateItem(model);
        }

        public async Task<int> CreateSingleItemWithChildrens(ItemModel model)
        {
            return await logic.CreateSingleItemWithChildrens(model);
        }

        public async Task<bool> UpdateSingleItemWithChildren(ItemModel model)
        {
            return await logic.UpdateSingleItemWithChildren(model);
        }

        public async Task<bool> CreateMultipleItemsWithChildrens(List<ItemModel> items)
        {
            return await logic.CreateMultipleItemsWithChildrens(items);
        }

        public async Task<bool> UpdateMultipleItemsWithChildrens(List<ItemModel> items)
        {
            return await logic.UpdateMultipleItemsWithChildrens(items);
        }

        // DELETE: api/Item/5
        public async Task<bool> Delete(int id)
        {
            return await logic.DeleteItem(id);
        }
    }
}
