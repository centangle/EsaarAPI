using Models.Base;
using Models.BriefModel;
using System;

namespace Models
{
    public class DonationRequestItemModel : BaseModel
    {
        public int DonationRequestId { get; set; }
        public BaseBriefModel Item { get; set; }
        public int Quantity { get; set; }
        public BaseBriefModel SelectedUnit { get; set; }
        public DateTime DueDate { get; set; }
        public string Note { get; set; }
    }
}
