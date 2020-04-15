using Models.Base;
using Models.BriefModel;

namespace Models
{
    public class PackageItemModel : BaseModel
    {
        public BaseBriefModel Item { get; set; }
        public double ItemQuantity { get; set; }
        public UOMBriefModel ItemUOM { get; set; }
    }
}
