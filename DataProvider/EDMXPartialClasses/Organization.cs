using Models.Interfaces;
using System.Collections.Generic;

namespace DataProvider
{
    public partial class Organization : ITree<Organization>, IBase
    {
        public ICollection<Organization> Children
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
