using System;
using System.Collections.Generic;

namespace EntityProvider.DbModels
{
    public partial class PackageItem
    {
        public int Id { get; set; }
        public int PackageId { get; set; }
        public int ItemId { get; set; }
        public double ItemQuantity { get; set; }
        public int ItemUom { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        public virtual Item Item { get; set; }
        public virtual Uom ItemUomNavigation { get; set; }
        public virtual Item Package { get; set; }
    }
}
