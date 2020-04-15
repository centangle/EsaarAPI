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
    public class CampaignSearchModel : BaseSearchModel
    {
        public CampaignSearchModel()
        {
            OrderByColumn = "Name";
        }
        public string Name { get; set; }
        public int? OrganizationId { get; set; }

    }
}
