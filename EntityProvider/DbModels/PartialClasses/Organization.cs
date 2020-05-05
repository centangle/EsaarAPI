using Models.Interfaces;
using System.Collections.Generic;

namespace EntityProvider.DbModels
{
    public partial class Organization : ITree<Organization>, IBase, IName
    {
        public ICollection<Organization> children
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
