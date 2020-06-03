using Catalogs;
using Models.Base;
using Models.BriefModel;
using Models.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Models
{
    public class CampaignModel : BaseModel, IImage, IName
    {
        [Required]
        public string Name { get; set; }
        public string NativeName { get; set; }
        public BaseBriefModel Organization { get; set; }
        public BaseBriefModel Event { get; set; }
        [IgnoreDataMember]
        public string BaseFolder
        {
            get
            {
                return "Campaigns";
            }
        }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string ImageInBase64 { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public List<OrganizationItemModel> Items { get; set; }
    }
    public class CampaignSearchModel : BaseSearchModel, IRadiusRegionSearch
    {
        public CampaignSearchModel()
        {
            OrderByColumn = "Name";
        }
        public string Name { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public float? Radius { get; set; }
        public List<int> RootCategories { get; set; }
        public int? RegionId { get; set; }
        public RegionSearchTypeCatalog? SearchType { get; set; }
        public List<RegionLevelSearchModel> Regions { get; set; }
        public RegionRadiusTypeCatalog? RadiusType { get; set; }
        public int? OrganizationId { get; set; }
        public int? EventId { get; set; }
        public bool OwnedByMe { get; set; }
    }
}
