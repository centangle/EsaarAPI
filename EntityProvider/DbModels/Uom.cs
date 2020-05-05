using System;
using System.Collections.Generic;

namespace EntityProvider.DbModels
{
    public partial class Uom
    {
        public Uom()
        {
            DonationRequestItems = new HashSet<DonationRequestItem>();
            InverseParent = new HashSet<Uom>();
            PackageItems = new HashSet<PackageItem>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string NativeName { get; set; }
        public string Abbreviation { get; set; }
        public double NoOfBaseUnit { get; set; }
        public int? ParentId { get; set; }
        public int? RootId { get; set; }
        public bool IsPeripheral { get; set; }
        public string Description { get; set; }
        public int Type { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        public virtual Uom Parent { get; set; }
        public virtual ICollection<DonationRequestItem> DonationRequestItems { get; set; }
        public virtual ICollection<Uom> InverseParent { get; set; }
        public virtual ICollection<PackageItem> PackageItems { get; set; }
    }
}
