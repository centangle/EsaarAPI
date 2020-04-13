//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataProvider
{
    using System;
    using System.Collections.Generic;
    
    public partial class Item
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Item()
        {
            this.DonationRequestItems = new HashSet<DonationRequestItem>();
            this.Item1 = new HashSet<Item>();
            this.OrganizationItems = new HashSet<OrganizationItem>();
        }
    
        public int Id { get; set; }
        public Nullable<int> OrganizationId { get; set; }
        public Nullable<int> RootId { get; set; }
        public Nullable<int> ParentId { get; set; }
        public string Name { get; set; }
        public string NativeName { get; set; }
        public Nullable<int> DefaultUOM { get; set; }
        public string Description { get; set; }
        public Nullable<int> Type { get; set; }
        public string ImageUrl { get; set; }
        public bool IsPeripheral { get; set; }
        public Nullable<bool> IsCartItem { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public int UpdatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DonationRequestItem> DonationRequestItems { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Item> Item1 { get; set; }
        public virtual Item Item2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrganizationItem> OrganizationItems { get; set; }
    }
}
