using AutoMapper;
using DataProvider.Helpers;
using Models;
using Models.Interfaces;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DataProvider
{
    public partial class DataAccess
    {
        private D SetTreeNode<D, M>(D dbModel, M model)
            where M : class, ITree<M>
            where D : class
        {
            if (typeof(D) == typeof(Item))
            {
                return (SetItem(dbModel as Item, model as ItemModel) as D);
            }
            else if (typeof(D) == typeof(Organization))
            {
                return (SetOrganization(dbModel as Organization, model as OrganizationModel) as D);
            }
            else if (typeof(D) == typeof(UOM))
            {
                return (SetUOM(dbModel as UOM, model as UOMModel) as D);
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
        private async Task ModifyTreeNodes<D, M>(CharityEntities context, DbSet<D> newDbNodes, List<D> currentDbNodes, List<TreeTraversal<M>> allNodes)
            where M : class, ITree<M>
            where D : class, ITree<D>, new()
        {
            var newNodes = allNodes.Where(x => x.Node.Id == 0);
            foreach (var newNode in newNodes)
            {
                var dbModel = SetTreeNode(new D(), newNode.Node);

                newDbNodes.Add(dbModel);
                currentDbNodes.Add(dbModel);
                await context.SaveChangesAsync();
                var currentNode = allNodes.Where(x => x.Id == newNode.Id).FirstOrDefault();
                newNode.Node.Id = dbModel.Id;
            }
            foreach (var dbNode in currentDbNodes)
            {
                int? parentId = null;
                int? rootId = null;
                var node = allNodes.Where(x => x.Node.Id == dbNode.Id).FirstOrDefault();

                if (node != null)
                {
                    if (node.Node.ParentId != null)
                        parentId = allNodes.Where(x => x.Id == node.ParentId).Select(x => x.Node.Id).FirstOrDefault();
                    if (node.Node.RootId != null)
                        rootId = allNodes.Where(x => x.Id == node.RootId).Select(x => x.Node.Id).FirstOrDefault();
                    var dbModel = SetTreeNode(dbNode, node.Node);
                    if (parentId == null || parentId == 0)
                    {
                        dbModel.ParentId = null;
                    }
                    else
                    {
                        dbModel.ParentId = parentId;
                    }
                    if (rootId == null || rootId == 0)
                    {
                        dbModel.RootId = null;
                    }
                    else
                    {
                        dbModel.RootId = rootId;
                    }
                }
            }
            var deletedNodes = currentDbNodes.Where(m => !allNodes.Any(s => m.Id == s.Node.Id)).ToList();
            foreach (var node in deletedNodes)
            {
                node.IsDeleted = true;
            }
            await context.SaveChangesAsync();
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
