using Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProvider
{
    public partial class UOM : ITree<UOM>, IBase
    {
        public ICollection<UOM> children
        {
            get
            {
                return UOM1;
            }
            set
            {
                UOM1 = value;
            }
        }
    }
}
