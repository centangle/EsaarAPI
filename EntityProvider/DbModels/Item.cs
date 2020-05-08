using System;
using System.Collections.Generic;

namespace EntityProvider.DbModels
{
    public partial class Item
    {
        public Item()
        {
            DonationRequestItems = new HashSet<DonationRequestItem>();
            InverseParent = new HashSet<Item>();
            OrganizationItems = new HashSet<OrganizationItem>();
            PackageItemItems = new HashSet<PackageItem>();
            PackageItemPackages = new HashSet<PackageItem>();
        }

        public int Id { get; set; }
        public int? OrganizationId { get; set; }
        public int? RootId { get; set; }
        public int? ParentId { get; set; }
        public string Name { get; set; }
        public string NativeName { get; set; }
        public double? Worth { get; set; }
        public int? DefaultUom { get; set; }
        public string Description { get; set; }
        public int? Type { get; set; }
        public string ImageUrl { get; set; }
        public bool IsPeripheral { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        public virtual Organization Organization { get; set; }
        public virtual Item Parent { get; set; }
        public virtual ICollection<DonationRequestItem> DonationRequestItems { get; set; }
        public virtual ICollection<Item> InverseParent { get; set; }
        public virtual ICollection<OrganizationItem> OrganizationItems { get; set; }
        public virtual ICollection<PackageItem> PackageItemItems { get; set; }
        public virtual ICollection<PackageItem> PackageItemPackages { get; set; }
    }
}
