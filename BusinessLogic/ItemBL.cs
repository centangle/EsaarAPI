using Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public partial class Logic
    {
        public async Task<int> CreateItem(ItemModel model)
        {
            return await DA.CreateItem(model);
        }
        public async Task<bool> UpdateItem(ItemModel model)
        {
            return await DA.UpdateItem(model);
        }
        public async Task<int> CreateSingleItemWithChildrens(ItemModel model)
        {
            return await DA.CreateSingleItemWithChildrens(model);
        }

        public async Task<bool> UpdateSingleItemWithChildren(ItemModel model)
        {
            return await DA.UpdateSingleItemWithChildren(model);
        }
        public async Task<bool> CreateMultipleItemsWithChildrens(List<ItemModel> items)
        {
            return await DA.CreateMultipleItemsWithChildrens(items);
        }

        public async Task<bool> UpdateMultipleItemsWithChildrens(List<ItemModel> items)
        {
            return await DA.UpdateMultipleItemsWithChildrens(items);
        }

        public async Task<ItemModel> GetItem(int id)
        {
            return await DA.GetItem(id);
        }
        public async Task<IEnumerable<ItemModel>> GetSingleItemHierarchy(int id, bool getHierarchicalData)
        {
            return await DA.GetSingleTreeItem(id, getHierarchicalData);
        }
        public async Task<IEnumerable<ItemModel>> GetPeripheralItems()
        {
            return await DA.GetPeripheralItems();
        }
        public async Task<IEnumerable<ItemModel>> GetRootItems()
        {
            return await DA.GetRootItems();
        }
        public async Task<IEnumerable<ItemModel>> GetAllItems(bool getHierarchicalData)
        {
            return await DA.GetAllItems(getHierarchicalData);
        }
        public async Task<bool> DeleteItem(int id)
        {
            return await DA.DeleteItem(id);
        }
    }

    public class DataStructureCatalog
    {
    }
}