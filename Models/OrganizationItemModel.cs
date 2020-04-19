using Catalogs;
using Models.Base;
using Models.BriefModel;
using System.CodeDom;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Models
{
    public class OrganizationItemModel : BaseModel
    {
        public OrganizationItemModel()
        {
            Organization = new BaseBriefModel();
            Item = new BaseBriefModel();
            Campaign = new BaseBriefModel();
            CampaignItemUOM = new UOMBriefModel();
        }
        public BaseBriefModel Organization { get; set; }
        public BaseBriefModel Item { get; set; }
        public BaseBriefModel Campaign { get; set; }
        public double CampaignItemTarget { get; set; }
        public UOMBriefModel CampaignItemUOM { get; set; }
        public bool IsCampaignItemOnly { get; set; }
    }
    public class OrganizationItemPaginationModel : OrganizationItemModel
    {
        public OrganizationItemPaginationModel()
        {
            ItemUOMs = new List<UOMBriefModel>();
        }
        [IgnoreDataMember]
        public string ItemName { get; set; }
        [IgnoreDataMember]
        public string ItemNativeName { get; set; }
        public UOMBriefParentModel ItemDefaultUOM { get; set; }
        public List<UOMBriefModel> ItemUOMs { get; set; }
    }
    public class OrganizationItemSearchModel : BaseSearchModel
    {
        public OrganizationItemSearchModel()
        {
            OrderByColumn = "ItemName";
        }
        public int OrganizationId { get; set; }
        public int? CampaignId { get; set; }
        public string ItemName { get; set; }
        public SearchItemTypeCatalog Type { get; set; }
    }
}
