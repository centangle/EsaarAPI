using Catalogs;
using Models.Base;
using Models.BriefModel;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Models
{
    public class OrganizationMembershipModel : BaseModel
    {
        public OrganizationMembershipModel()
        {
            Organization = new BaseBriefModel();
            Member = new BaseBriefModel();
        }
        public BaseBriefModel Organization { get; set; }
        public BaseBriefModel Member { get; set; }
        public List<OrganizationMemberRolesCatalog> Roles { get; set; }
        public OrganizationMemberRolesCatalog Role { get; set; }
    }
}
