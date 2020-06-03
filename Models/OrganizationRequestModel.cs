using Catalogs;
using Models.Base;
using Models.BriefModel;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Helpers;

namespace Models
{
    public class OrganizationRequestModel : BaseModel
    {
        public OrganizationRequestModel()
        {
            Organization = new BaseImageBriefModel();
            Entity = new BaseBriefModel();
            Moderator = new BaseBriefModel();
            Regions = new List<EntityRegionModel>();
        }
        public BaseImageBriefModel Organization { get; set; }
        public BaseBriefModel Entity { get; set; }
        public OrganizationRequestEntityTypeCatalog EntityType { get; set; } = OrganizationRequestEntityTypeCatalog.Member;
        public OrganizationRequestTypeCatalog Type { get; set; } = OrganizationRequestTypeCatalog.Volunteer;
        public StatusCatalog Status { get; set; }
        public BaseBriefModel Moderator { get; set; }
        public List<EntityRegionModel> Regions { get; set; }
        public string Note { get; set; }
    }

    public class PaginatedOrganizationRequestModel : OrganizationRequestModel
    {
        public List<OrganizationMemberRolesCatalog> CurrentMemberRoles { get; set; }
        [IgnoreDataMember]
        public int LoggedInMemberId { get; set; }
        public bool IsLoggedInMemberOrganizationOwner { get; set; }
        public bool IsLoggedInMemberOrganizationModerator { get; set; }
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
                if (
                        LoggedInMemberId != 0 &&
                        (
                            (Moderator != null && Moderator.Id == LoggedInMemberId)
                            || IsLoggedInMemberOrganizationOwner
                            || (CreatedBy == LoggedInMemberId)
                        )
                    )
                    return true;
                else
                    return false;
            }
        }
        public bool CanSelfAssign
        {
            get
            {
                if (IsOpenRequest && (IsLoggedInMemberOrganizationModerator || IsLoggedInMemberOrganizationOwner))
                    return true;
                else
                    return false;
            }
        }
        public bool CanUpdateStatus
        {
            get
            {
                if (
                        (
                            (Moderator != null && Moderator.Id == LoggedInMemberId)
                            ||
                            IsLoggedInMemberOrganizationOwner
                        )
                        && (Status != StatusCatalog.Approved && Status != StatusCatalog.Rejected)
                    )
                    return true;
                else
                    return false;
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
        public bool ShowRegionsInRequestThread
        {
            get
            {
                if (Type == OrganizationRequestTypeCatalog.Volunteer || Type == OrganizationRequestTypeCatalog.Moderator)
                    return true;
                else
                    return false;
            }
        }
        public bool CanUpdateRegions
        {
            get
            {
                if (LoggedInMemberId != 0
                    &&
                    (
                        CreatedBy == LoggedInMemberId
                        || IsLoggedInMemberOrganizationOwner
                        || IsLoggedInMemberOrganizationModerator
                     )
                  )
                    return true;
                else
                    return false;
            }
        }
        public StatusCatalog NextStatus
        {
            get
            {
                var nextStatus = Status.Next();
                if (nextStatus > StatusCatalog.Approved)
                {
                    return Status;
                }
                else
                    return nextStatus;
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
        public string MemberName { get; set; }
        public List<OrganizationRequestTypeCatalog> Types { get; set; }
        public TimePeriodCatalog? TimePeriod { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<StatusCatalog> Statuses { get; set; }

    }
}
