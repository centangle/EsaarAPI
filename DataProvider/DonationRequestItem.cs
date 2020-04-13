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
    
    public partial class DonationRequestItem
    {
        public int Id { get; set; }
        public int DonationRequestId { get; set; }
        public int ItemId { get; set; }
        public double Quantity { get; set; }
        public int QuantityUOM { get; set; }
        public string Note { get; set; }
        public Nullable<System.DateTime> DueDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public int UpdatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
    
        public virtual DonationRequest DonationRequest { get; set; }
        public virtual Item Item { get; set; }
        public virtual UOM UOM { get; set; }
    }
}
