﻿using Models.Base;
using Models.BriefModel;
using System.CodeDom;
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
        [IgnoreDataMember]
        public string ItemName { get; set; }
        [IgnoreDataMember]
        public string ItemNativeName { get; set; }
    }
    public class OrganizationItemSearchModel : BaseSearchModel
    {
        public OrganizationItemSearchModel()
        {
            OrderByColumn = "ItemName";
        }
        public int OrganizationId { get; set; }
        public string ItemName { get; set; }
    }
}
