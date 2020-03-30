using Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public partial class Logic
    {
        public async Task<int> AddSingleItem(ItemModel model)
        {
            return await DA.AddSingleItem(model);
        }
        public async Task<bool> UpdateSingleItem(ItemModel model)
        {
            return await DA.UpdateSingleItem(model);
        }
        public async Task<int> AddMultipleChildItem(ItemModel model)
        {
            return await DA.AddMultipleChildItem(model);
        }

        public async Task<bool> UpdateMultipleChildItem(ItemModel model)
        {
            return await DA.UpdateMultipleChildItem(model);
        }

        public async Task<IEnumerable<ItemModel>> GetSingleItemHierarchy(int id)
        {
            return await DA.GetSingleItemTree<ItemModel,ItemModel>(id);
        }
    }
}
