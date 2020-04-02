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
            if (model.children != null && model.children.Count > 0)
            {
                foreach (var item in model.children)
                {
                    if (item != null)
                    {
                        if (item.children != null && item.children.Count > 0)
                        {
                            dbModel.children.Add(AddTreeNode<D, M>(item as M, isNew));
                        }
                        else
                        {
                            D addedItem = SetTreeNode(new D(), item, isNew);
                            if (addedItem != null)
                            {
                                dbModel.children.Add(addedItem);
                            }
                        }
                    }
                }
            }
            return dbModel;
        }
        private void UpdateTreeNode<D, M>(CharityEntities context, D dbModel, M model, Dictionary<int, int> updatedParents)
            where M : class, ITree<M>
            where D : class, ITree<D>, new()
        {
            SetTreeNode(dbModel, model, false);
            var masterList = dbModel.children;
            var newItems = UpdatedListItem.NewItems(model.children);
            var newAddedChild = UpdatedListItem.NodeNewChilds(model.Id, model.children);
            var updatedItems = UpdatedListItem.UpdatedItems(masterList, model.children);
            var deletedItems = UpdatedListItem.DeletedItems(masterList, model.children);
            foreach (var item in newItems)
            {
                dbModel.children.Add(AddTreeNode<D, M>(item, true));
            }
            foreach (var item in newAddedChild)
            {
                updatedParents.Add(dbModel.Id, item.Id);
            }
            foreach (var dbItem in updatedItems)
            {
                var item = model.children.Where(x => x.Id == dbItem.Id).FirstOrDefault();
                SetTreeNode(dbItem, item, false);
                // Update Hierarchy
                foreach (var dbChildItem in dbItem.children)
                {
                    var childItem = item.children.Where(x => x.Id == dbChildItem.Id).FirstOrDefault();
                    UpdateTreeNode(context, dbChildItem, childItem,updatedParents);
                }
            }
            foreach (var dbItem in deletedItems)
            {
                DeleteTreeNode(dbItem);
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
        private void DeleteTreeNode<D>(D dbModel) where D : ITree<D>
        {
            dbModel.IsDeleted = true;
            if (dbModel.children != null && dbModel.children.Count > 0)
            {
                foreach (var item in dbModel.children)
                {
                    DeleteTreeNode(item);
                }
            }
        }

        private static IEnumerable<T> GetSingleNodeTree<T, D, M>(int id, MapperConfiguration mapperConfig, IEnumerable<D> itemsDBList, bool returnViewModel = true, bool getHierarchicalData = true)
            where T : class, IBase
            where D : class, ITree<D>
            where M : class, ITree<M>
        {
            var items = TreeHelper.GetTreeData<T, D, M>(id, mapperConfig, itemsDBList, returnViewModel, getHierarchicalData);
            return items;
        }
        private IEnumerable<T> GetAllNodes<T, D, M>(MapperConfiguration mapperConfig, IEnumerable<D> itemsDBList, bool returnViewModel = true, bool getHierarchicalData = true)
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
