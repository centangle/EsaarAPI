using Models.Interfaces;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityProvider.DbModels
{
    public partial class Organization : ITree<Organization>, IBase, IName
    {
        [NotMapped]// Added because of Error Unknown column OrganizationId when we write _context.Organizations.ToList()
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
