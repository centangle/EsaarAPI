using Models.Base;
using Models.BriefModel;
using System.Collections.Generic;

namespace Models
{
    public class DonationRequestOrganizationItemModel : BaseModel
    {
        public DonationRequestOrganizationItemModel()
        {
            Item = new BaseBriefModel();
            QuantityUOM = new UOMBriefModel();
            ApprovedQuantityUOM = new UOMBriefModel();
            CollectedQuantityUOM = new UOMBriefModel();
            DeliveredQuantityUOM = new UOMBriefModel();
            ItemDefaultUOM = new UOMBriefParentModel();
            ItemUOMs = new List<UOMBriefModel>();
        }
        public BaseBriefModel Item { get; set; }
        public UOMBriefParentModel ItemDefaultUOM { get; set; }
        public List<UOMBriefModel> ItemUOMs { get; set; }
        public bool IsApproved { get; set; }
        public double? Quantity { get; set; }
        public UOMBriefModel QuantityUOM { get; set; }
        public double? ApprovedQuantity { get; set; }
        public UOMBriefModel ApprovedQuantityUOM { get; set; }
        public double? CollectedQuantity { get; set; }
        public UOMBriefModel CollectedQuantityUOM { get; set; }
        public double? DeliveredQuantity { get; set; }
        public UOMBriefModel DeliveredQuantityUOM { get; set; }

    }
}
