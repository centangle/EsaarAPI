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
            Moderator = new BaseBriefModel();
        }
        public BaseBriefModel Organization { get; set; }
        public BaseBriefModel Entity { get; set; }
        public OrganizationRequestEntityTypeCatalog EntityType { get; set; } = OrganizationRequestEntityTypeCatalog.Member;
        public OrganizationRequestTypeCatalog Type { get; set; } = OrganizationRequestTypeCatalog.Volunteer;
        public StatusCatalog Status { get; set; }
        public BaseBriefModel Moderator { get; set; }
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
                if (Moderator == null || Moderator.Id == 0)
                    return true;
                else
                    return false;
            }
        }
        public bool CanAccessRequestThread
        {
            get
            {
                if (LoggedInMemberId != 0 && ((Moderator != null && Moderator.Id == LoggedInMemberId) || (CreatedBy == LoggedInMemberId)))
                    return true;
                else
                    return false;
            }
        }
        public bool CanUpdateStatus
        {
            get
            {
                if ((Moderator != null && Moderator.Id == LoggedInMemberId) && (Status == StatusCatalog.Approved || Status == StatusCatalog.Rejected))
                    return false;
                else
                    return true;
            }
        }
        public bool CanAddMessageToThread
        {
            get
            {
                if (Status == StatusCatalog.Approved || Status == StatusCatalog.Rejected)
                    return false;
                else
                    return true;
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
