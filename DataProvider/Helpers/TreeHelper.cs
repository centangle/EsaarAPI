using AutoMapper;
using Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataProvider.Helpers
{
    public static class TreeHelper
    {
        public static IEnumerable<T> GetTreeData<T, D, M>(int rootNodeId, MapperConfiguration mapperConfig, IEnumerable<D> dbList, bool returnViewModel, bool getHierarchicalData)
            where D : ITree<D>
            where M : class, ITree<M>
            where T : class, IBase
        {
            List<T> items = new List<T>();
            if (getHierarchicalData)
            {
                var rootNode = dbList.Where(x => x.Id == rootNodeId).FirstOrDefault();
                if (returnViewModel)
                {
                    var item = GetMappedItem<D, M>(rootNode, mapperConfig);
                    GenerateTree(item, dbList, returnViewModel, mapperConfig);
                    items.Add(item as T);
                }
                else
                {
                    GenerateTree(rootNode as M, dbList, returnViewModel, mapperConfig);
                    items.Add(rootNode as T);
                }
            }
            else
            {
                if (returnViewModel)
                {
                    foreach (var dbitem in dbList)
                    {
                        var item = GetMappedItem<D, M>(dbitem, mapperConfig);
                        items.Add(item as T);
                    }
                }
                else
                {
                    items = dbList.Cast<T>().ToList();
                }
            }
            return items;

        }
        private static void GenerateTree<M, D>(M rootNode, IEnumerable<D> dbList, bool returnViewModel, MapperConfiguration mapperConfig)
            where D : ITree<D>
            where M : class, ITree<M>
        {
            var childItems = dbList.Where(x => x.ParentId == rootNode.Id).ToList();
            foreach (var childItem in childItems)
            {
                if (returnViewModel)
                {
                    var item = GetMappedItem<D, M>(childItem, mapperConfig);
                    rootNode.children.Add(item);
                    GenerateTree(item, dbList, returnViewModel, mapperConfig);
                }
                else
                {
                    var item = childItem as M;
                    rootNode.children.Add(item);
                    GenerateTree(item, dbList, returnViewModel, mapperConfig);
                }

            }

        }
        private static M GetMappedItem<D, M>(D dbItem, MapperConfiguration mapperConfig)
        {
            IMapper iMapper = mapperConfig.CreateMapper();
            return iMapper.Map<D, M>(dbItem);
        }

        public static List<TreeTraversal<T>> TreeToList<T>(List<T> treeNodes) where T : ITree<T>, new()
        {
            List<TreeTraversal<T>> singleNodes = new List<TreeTraversal<T>>();
            foreach (var node in treeNodes)
            {
                var traversalNode = new TreeTraversal<T>();
                traversalNode.Id = Guid.NewGuid();
                traversalNode.ParentId = null;
                traversalNode.Node = node;
                singleNodes.Add(traversalNode);
                SingleTreeNodeToList(node, singleNodes, traversalNode.Id);
            }
            return singleNodes;
        }
        public static void SingleTreeNodeToList<T>(T treeNodes, List<TreeTraversal<T>> singleNodes, Guid ParentId) where T : ITree<T>, new()
        {

            foreach (var node in treeNodes.children)
            {
                var traversalNode = new TreeTraversal<T>();
                traversalNode.Id = Guid.NewGuid();
                traversalNode.ParentId = ParentId;
                traversalNode.Node = node;
                singleNodes.Add(traversalNode);
                SingleTreeNodeToList(node, singleNodes, traversalNode.Id);
            }

        }


    }
}
