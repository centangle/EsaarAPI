using Catalogs;
using Models.Base;
using Models.BriefModel;

namespace Models
{
    public class OrganizationRequestModel : BaseModel
    {
        public OrganizationRequestModel()
        {
            Organization = new BaseBriefModel();
            Entity = new BaseBriefModel();
            AssignedTo = new BaseBriefModel();
        }
        public BaseBriefModel Organization { get; set; }
        public BaseBriefModel Entity { get; set; }
        public OrganizationRequestEntityTypeCatalog EntityType { get; set; } = OrganizationRequestEntityTypeCatalog.Member;
        public OrganizationRequestTypeCatalog Type { get; set; } = OrganizationRequestTypeCatalog.Volunteer;
        public BaseBriefModel AssignedTo { get; set; }
        public string Note { get; set; }
    }

    public class OrganizationRequestSearchModel : BaseSearchModel
    {
        public OrganizationRequestSearchModel()
        {
            OrderByColumn = "CreatedDate";
        }
        public int OrganizationId { get; set; }
        public OrganizationRequestTypeCatalog? Type { get; set; }

    }
}
