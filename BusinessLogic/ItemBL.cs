﻿using Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public partial class Logic
    {
        public async Task<int> CreateItem(ItemModel model)
        {
            return await _dataAccess.CreateItem(model);
        }
        public async Task<bool> UpdateItem(ItemModel model)
        {
            return await _dataAccess.UpdateItem(model);
        }
        public async Task<bool> CreateSingleItemWithChildrens(ItemModel model, int? organizationId = null)
        {
            return await _dataAccess.CreateSingleItemWithChildrens(model, organizationId);
        }
        public async Task<bool> UpdateSingleItemWithChildren(ItemModel model, int? organizationId = null)
        {
            return await _dataAccess.UpdateSingleItemWithChildren(model, organizationId);
        }
        public async Task<bool> CreateMultipleItemsWithChildrens(List<ItemModel> items, int? organizationId = null)
        {
            return await _dataAccess.CreateMultipleItemsWithChildrens(items, organizationId);
        }
        public async Task<bool> UpdateMultipleItemsWithChildrens(List<ItemModel> items, int? organizationId = null)
        {
            return await _dataAccess.UpdateMultipleItemsWithChildrens(items, organizationId);
        }
        public async Task<ItemModel> GetItem(int id)
        {
            return await _dataAccess.GetItem(id);
        }
        public async Task<IEnumerable<ItemModel>> GetSingleItemHierarchy(int id, bool getHierarchicalData)
        {
            return await _dataAccess.GetSingleTreeItem(id, getHierarchicalData);
        }
        public async Task<IEnumerable<ItemModel>> GetPeripheralItems()
        {
            return await _dataAccess.GetPeripheralItems();
        }
        public async Task<IEnumerable<ItemModel>> GetRootItems()
        {
            return await _dataAccess.GetRootItems();
        }
        public async Task<IEnumerable<ItemModel>> GetAllItems(bool getHierarchicalData)
        {
            return await _dataAccess.GetAllItems(getHierarchicalData);
        }
        public async Task<bool> DeleteItem(int id, int? organizationId = null)
        {
            return await _dataAccess.DeleteItem(id, organizationId);
        }
    }
}