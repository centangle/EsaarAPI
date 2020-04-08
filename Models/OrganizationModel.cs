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
    public class OrganizationModel : BaseModel, IImage, ITree<OrganizationModel>, IPeripheral
    {
        public OrganizationModel()
        {
            children = new List<OrganizationModel>();
        }
        [Required]
        public string Name { get; set; }
        public string NativeName { get; set; }
        [IgnoreDataMember]
        public int? ParentId
        {
            get
            {
                if (Parent != null)
                    return Parent.Id;
                else
                    return 0;
            }
            set { }
        }
        public BaseBriefModel Parent { get; set; }
        public OrganizationTypeCatalog Type { get; set; }
        [IgnoreDataMember]
        public string BaseFolder
        {
            get
            {
                return "Organizations";
            }
        }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string ImageInBase64 { get; set; }
        public MemberBriefModel OwnedBy { get; set; }
        public bool IsPeripheral { get; set; }
        public bool IsVerified { get; set; }
        public ICollection<OrganizationModel> children { get; set; }
    }
}
