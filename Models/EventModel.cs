using Models.Base;
using Models.BriefModel;
using Models.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Models
{
    public class EventModel : BaseModel, IImage, IName
    {
        [Required]
        public string Name { get; set; }
        public string NativeName { get; set; }
        [IgnoreDataMember]
        public string BaseFolder
        {
            get
            {
                return "Events";
            }
        }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string ImageInBase64 { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public List<OrganizationItemModel> Items { get; set; }
    }
    public class EventSearchModel : BaseSearchModel
    {
        public string Name { get; set; }
        public bool? IsActive { get; set; }
    }
}
