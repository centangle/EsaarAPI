using Catalogs;
using Models.Base;
using Models.BriefModel;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Models
{
    public class OrganizationMemberRegionModel : BaseModel
    {
        public OrganizationMemberRegionModel()
        {
            Organization = new BaseBriefModel();
            Member = new BaseBriefModel();
            Region = new EntityRegionBriefModel();
            Regions = new List<EntityRegionBriefModel>();
        }
        public BaseBriefModel Organization { get; set; }
        public BaseBriefModel Member { get; set; }
        public EntityRegionBriefModel Region { get; set; }
        public List<EntityRegionBriefModel> Regions { get; set; }
        public OrganizationMemberRolesCatalog Role { get; set; }
    }
}
