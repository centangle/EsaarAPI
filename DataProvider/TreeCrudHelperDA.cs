using AutoMapper;
using DataProvider.Helpers;
using Models;
using Models.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace DataProvider
{
    public partial class DataAccess
    {
        private D AddTreeItem<D, M>(M model, bool isNew)
            where M : class, ITree<M>
            where D : class, ITree<D>, new()
        {
            var dbModel = SetTreeItem(new D(), model, isNew);
            if (model.Childrens != null && model.Childrens.Count > 0)
            {
                foreach (var item in model.Childrens)
                {
                    if (item != null)
                    {
                        if (item.Childrens != null && item.Childrens.Count > 0)
                        {
                            dbModel.Childrens.Add(AddTreeItem<D, M>(item as M, isNew));
                        }
                        else
                        {
                            D addedItem = SetTreeItem(new D(), item, isNew);
                            if (addedItem != null)
                            {
                                dbModel.Childrens.Add(addedItem);
                            }
                        }
                    }
                }
            }
            return dbModel;
        }
        private void UpdateTreeItem<D, M>(CharityEntities context, D dbModel, M model)
            where M : class, ITree<M>
            where D : class, ITree<D>, new()
        {
            SetTreeItem(dbModel, model, false);
            var masterList = dbModel.Childrens;
            var newItems = UpdatedListItem.NewItems(model.Childrens);
            var updatedItems = UpdatedListItem.UpdatedItems(masterList, model.Childrens);
            var deletedItems = UpdatedListItem.DeletedItems(masterList, model.Childrens);
            foreach (var item in newItems)
            {
                dbModel.Childrens.Add(AddTreeItem<D, M>(item, true));
            }
            foreach (var dbItem in updatedItems)
            {
                var item = model.Childrens.Where(x => x.Id == dbItem.Id).FirstOrDefault();
                SetTreeItem(dbItem, item, false);
                // Update Hierarchy
                foreach (var dbChildItem in dbItem.Childrens)
                {
                    var childItem = item.Childrens.Where(x => x.Id == dbChildItem.Id).FirstOrDefault();
                    UpdateTreeItem(context, dbChildItem, childItem);
                }
            }
            foreach (var dbItem in deletedItems)
            {
                DeleteItemTree(dbItem);
            }
        }
        private D SetTreeItem<D, M>(D dbModel, M model, bool isNew)
            where M : class, ITree<M>
            where D : class
        {
            if (typeof(D) == typeof(Item))
            {
                return (SetItem(dbModel as Item, model as ItemModel, isNew) as D);
            }
            return null;
        }
        private void DeleteItemTree<D>(D dbModel) where D : ITree<D>
        {
            dbModel.IsDeleted = true;
            if (dbModel.Childrens != null && dbModel.Childrens.Count > 0)
            {
                foreach (var item in dbModel.Childrens)
                {
                    DeleteItemTree(item);
                }
            }
        }

        public static IEnumerable<T> GetSingleNodeTree<T, D, M>(int id, MapperConfiguration mapperConfig, IEnumerable<D> itemsDBList, bool returnViewModel = true, bool getHierarchicalData = true)
            where T : class, IBase
            where D : class, ITree<D>
            where M : class, ITree<M>
        {
            var items= TreeHelper.GetTreeData<T, D, M>(id, mapperConfig, itemsDBList, returnViewModel, getHierarchicalData);
            return items;
        }
        public IEnumerable<T> GetAllNodes<T, D, M>(MapperConfiguration mapperConfig, IEnumerable<D> itemsDBList, bool returnViewModel = true, bool getHierarchicalData = true)
             where T : class, IBase
            where D : class, ITree<D>
            where M : class, ITree<M>
        {
            if (getHierarchicalData)
            {
                List<T> items = new List<T>();
                var rootNodes = itemsDBList.Where(x => x.ParentId == null || x.ParentId == 0).ToList();
                foreach (var rootNode in rootNodes)
                {
                    var nodeItems = TreeHelper.GetTreeData<T, D, M>(rootNode.Id, mapperConfig, itemsDBList, returnViewModel, getHierarchicalData);
                    items.Add(nodeItems.FirstOrDefault());
                }
                return items;
            }
            else
            {
                return TreeHelper.GetTreeData<T, D, M>(0, mapperConfig, itemsDBList, returnViewModel, getHierarchicalData);
            }
        }
    }
}
