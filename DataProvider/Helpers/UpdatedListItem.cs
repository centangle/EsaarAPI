﻿using Models.Base;
using Models.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace DataProvider.Helpers
{
    public static class UpdatedListItem
    {
        public static List<T> NewItems<T>(ICollection<T> modifiedList) where T : ITree<T>
        {
            return modifiedList.Where(s => s.Id == 0).ToList();
        }
        public static List<M> UpdatedItems<T, M>(ICollection<M> masterList, ICollection<T> modifiedList) where T : ITree<T> where M : IBase
        {
            return masterList.Where(m => modifiedList.Any(s => m.Id == s.Id)).ToList();
        }
        public static List<M> DeletedItems<T, M>(ICollection<M> masterList, ICollection<T> modifiedList) where T : ITree<T> where M : IBase
        {
            return masterList.Where(m => !modifiedList.Any(s => m.Id == s.Id)).ToList();
        }
    }
}