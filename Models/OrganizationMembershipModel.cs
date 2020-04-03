using Catalogs;
using Models.Base;
using Models.BriefModel;

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
        public OrganizationMemberTypeCatalog Type { get; set; }
    }
}
