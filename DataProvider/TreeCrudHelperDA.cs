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
        private D AddTreeNode<D, M>(M model, bool isNew)
            where M : class, ITree<M>
            where D : class, ITree<D>, new()
        {
            var dbModel = SetTreeNode(new D(), model, isNew);
            if (model.Children != null && model.Children.Count > 0)
            {
                foreach (var item in model.Children)
                {
                    if (item != null)
                    {
                        if (item.Children != null && item.Children.Count > 0)
                        {
                            dbModel.Children.Add(AddTreeNode<D, M>(item as M, isNew));
                        }
                        else
                        {
                            D addedItem = SetTreeNode(new D(), item, isNew);
                            if (addedItem != null)
                            {
                                dbModel.Children.Add(addedItem);
                            }
                        }
                    }
                }
            }
            return dbModel;
        }
        private void UpdateTreeNode<D, M>(CharityEntities context, D dbModel, M model)
            where M : class, ITree<M>
            where D : class, ITree<D>, new()
        {
            SetTreeNode(dbModel, model, false);
            var masterList = dbModel.Children;
            var newItems = UpdatedListItem.NewItems(model.Children);
            var updatedItems = UpdatedListItem.UpdatedItems(masterList, model.Children);
            var deletedItems = UpdatedListItem.DeletedItems(masterList, model.Children);
            foreach (var item in newItems)
            {
                dbModel.Children.Add(AddTreeNode<D, M>(item, true));
            }
            foreach (var dbItem in updatedItems)
            {
                var item = model.Children.Where(x => x.Id == dbItem.Id).FirstOrDefault();
                SetTreeNode(dbItem, item, false);
                // Update Hierarchy
                foreach (var dbChildItem in dbItem.Children)
                {
                    var childItem = item.Children.Where(x => x.Id == dbChildItem.Id).FirstOrDefault();
                    UpdateTreeNode(context, dbChildItem, childItem);
                }
            }
            foreach (var dbItem in deletedItems)
            {
                DeleteItemNode(dbItem);
            }
        }
        private D SetTreeNode<D, M>(D dbModel, M model, bool isNew)
            where M : class, ITree<M>
            where D : class
        {
            if (typeof(D) == typeof(Item))
            {
                return (SetItem(dbModel as Item, model as ItemModel, isNew) as D);
            }
            return null;
        }
        private void DeleteItemNode<D>(D dbModel) where D : ITree<D>
        {
            dbModel.IsDeleted = true;
            if (dbModel.Children != null && dbModel.Children.Count > 0)
            {
                foreach (var item in dbModel.Children)
                {
                    DeleteItemNode(item);
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
