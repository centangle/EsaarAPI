using System;
using System.Collections.Generic;

namespace EntityProvider.DbModels
{
    public partial class OrganizationItem
    {
        public int Id { get; set; }
        public int OrganizationId { get; set; }
        public int ItemId { get; set; }
        public double? CampaignItemTarget { get; set; }
        public int? CampaignItemUom { get; set; }
        public int? CampaignId { get; set; }
        public bool IsCampaignItemOnly { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        public virtual Item Item { get; set; }
        public virtual Organization Organization { get; set; }
    }
}
