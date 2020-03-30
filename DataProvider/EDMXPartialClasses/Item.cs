
using Models.Interfaces;
using System.Collections.Generic;

namespace DataProvider
{
    public partial class Item : ITree<Item>, IBase
    {
        public ICollection<Item> Childrens
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
