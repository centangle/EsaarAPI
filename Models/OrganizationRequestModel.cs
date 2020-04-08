using Catalogs;
using Models.Base;
using Models.BriefModel;
using System.Runtime.Serialization;

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
        public StatusCatalog Status { get; set; }
        public BaseBriefModel AssignedTo { get; set; }
        public string Note { get; set; }
    }

    public class PaginatedOrganizationRequestModel : OrganizationRequestModel
    {
        [IgnoreDataMember]
        public int LoggedInMemberId { get; set; }

        public bool IsOpenRequest
        {
            get
            {
                if (AssignedTo == null || AssignedTo.Id == 0)
                    return true;
                else
                    return false;
            }
        }
        public bool CanAccessRequestThread
        {
            get
            {
                if (AssignedTo != null && AssignedTo.Id == LoggedInMemberId && LoggedInMemberId != 0)
                    return true;
                else
                    return false;
            }
        }
    }

    public class OrganizationRequestSearchModel : BaseSearchModel
    {
        public OrganizationRequestSearchModel()
        {
            OrderByColumn = "CreatedDate";
        }
        public int? OrganizationId { get; set; }
        public OrganizationRequestTypeCatalog? Type { get; set; }

    }
}
