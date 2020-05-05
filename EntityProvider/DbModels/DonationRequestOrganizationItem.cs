using System;
using System.Collections.Generic;

namespace EntityProvider.DbModels
{
    public partial class DonationRequestOrganizationItem
    {
        public int Id { get; set; }
        public int RequestOrganizationId { get; set; }
        public int RequestItemId { get; set; }
        public double Quantity { get; set; }
        public int QuantityUom { get; set; }
        public double? CollectedQuantity { get; set; }
        public int? CollectedQuantityUom { get; set; }
        public double? DeliveredQuantity { get; set; }
        public int? DeliveredQuantityUom { get; set; }
        public int? Status { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int? CollectionVolunteerId { get; set; }
        public DateTime? CollectionDate { get; set; }
        public int? DeliveryVolunteerId { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string CollectionLatLong { get; set; }
        public string DeliveryLatLong { get; set; }
    }
}
