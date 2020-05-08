
using Models;
using Models.Interfaces;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityProvider.DbModels
{
    public partial class Item : ITree<Item>, IBase, IName
    {
        [NotMapped]
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
