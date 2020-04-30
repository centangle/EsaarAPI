﻿
using Models;
using Models.Interfaces;
using System.Collections.Generic;

namespace DataProvider
{
    public partial class Item : ITree<Item>, IBase, IName
    {
        public ICollection<Item> children
        {
            get
            {
                return Item1;
            }
            set
            {
                Item1 = value;
            }
        }
    }
}
