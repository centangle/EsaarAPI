using AutoMapper;
using Models.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace DataProvider.Helpers
{
    public static class TreeHelper
    {
        public static IEnumerable<T> GetTreeData<T, D, M>(IEnumerable<D> dbList, bool returnViewModel, bool getHierarchicalData, MapperConfiguration mapperConfig)
            where D : ITree<D>
            where M : class, ITree<M>
            where T : class, IBase
        {
            List<T> items = new List<T>();
            if (getHierarchicalData)
            {
                var rootNode = dbList.Where(x => x.ParentId == null || x.ParentId == 0).FirstOrDefault();
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
                    rootNode.Childrens.Add(item);
                    GenerateTree(item, dbList, returnViewModel, mapperConfig);
                }
                else
                {
                    var item = childItem as M;
                    rootNode.Childrens.Add(item);
                    GenerateTree(item, dbList, returnViewModel, mapperConfig);
                }

            }

        }
        private static M GetMappedItem<D, M>(D dbItem, MapperConfiguration mapperConfig)
        {
            IMapper iMapper = mapperConfig.CreateMapper();
            return iMapper.Map<D, M>(dbItem);
        }
    }
}
