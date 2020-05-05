
using Models;
using Models.Interfaces;
using System.Collections.Generic;

namespace EntityProvider.DbModels
{
    public partial class Item : ITree<Item>, IBase, IName
    {
        public ICollection<Item> children
        {
            get
            {
                return InverseParent;
            }
            set
            {
                InverseParent = value;
            }
        }
    }
}
