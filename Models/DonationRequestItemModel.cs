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
            SelectedUnit = new UOMBriefModel();
        }
        public int DonationRequestId { get; set; }
        public BaseBriefModel Item { get; set; }
        public int Quantity { get; set; }
        public UOMBriefModel SelectedUnit { get; set; }
        public DateTime DueDate { get; set; }
        public string Note { get; set; }
    }
}
