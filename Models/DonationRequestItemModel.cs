using Models.Base;
using Models.BriefModel;
using System;

namespace Models
{
    public class DonationRequestItemModel : BaseModel
    {
        public DonationRequestItemModel()
        {
            Item = new BaseBriefModel();
            QuantityUOM = new UOMBriefModel();
        }
        public BaseBriefModel Item { get; set; }
        public double Quantity { get; set; }
        public UOMBriefModel QuantityUOM { get; set; }
        public DateTime DueDate { get; set; }
        public string Note { get; set; }
    }
}
