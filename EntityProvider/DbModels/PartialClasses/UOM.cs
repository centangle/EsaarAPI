using Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityProvider.DbModels
{
    public partial class Uom : ITree<Uom>, IBase, IName
    {
        public ICollection<Uom> children
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
