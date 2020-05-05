using System;
using System.Collections.Generic;

namespace EntityProvider.DbModels
{
    public partial class DonationRequestItem
    {
        public int Id { get; set; }
        public int DonationRequestId { get; set; }
        public int ItemId { get; set; }
        public double Quantity { get; set; }
        public int QuantityUom { get; set; }
        public string Note { get; set; }
        public DateTime? DueDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        public virtual DonationRequest DonationRequest { get; set; }
        public virtual Item Item { get; set; }
        public virtual Uom QuantityUomNavigation { get; set; }
    }
}
