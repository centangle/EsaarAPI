using Models.Interfaces;
using System.Collections.Generic;

namespace DataProvider
{
    public partial class Organization : ITree<Organization>, IBase, IName
    {
        public ICollection<Organization> children
        {
            get
            {
                return Organization1;
            }
            set
            {
                Organization1 = value;
            }
        }
    }
}
