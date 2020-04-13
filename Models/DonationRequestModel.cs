using Catalogs;
using Models.Base;
using Models.BriefModel;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Models
{
    public class DonationRequestModel : BaseModel
    {
        public DonationRequestModel()
        {
            Member = new BaseBriefModel();
            Items = new List<DonationRequestItemModel>();
            Campaign = new BaseBriefModel();
        }
        public BaseBriefModel Member { get; set; }
        public List<DonationRequestItemModel> Items { get; set; }
        public DonationRequestTypeCatalog Type { get; set; }
        public string Note { get; set; }
        public DateTime Date { get; set; }
        public string PrefferedCollectionTime { get; set; }
        public string Address { get; set; }
        public string AddressLatLong { get; set; }
        public int OrganizationId { get; set; }
        public BaseBriefModel Campaign { get; set; }
    }
    public class PaginatedDonationRequestModel : DonationRequestModel
    {
        public PaginatedDonationRequestModel()
        {
            DonationRequestOrganization = new DonationRequestOrganizationModel();
        }
        [IgnoreDataMember]
        public int LoggedInMemberId { get; set; }
        public DonationRequestOrganizationModel DonationRequestOrganization { get; set; }

        public bool IsOpenRequestForModerator
        {
            get
            {
                if (DonationRequestOrganization != null && DonationRequestOrganization.Moderator == null || DonationRequestOrganization.Moderator.Id == 0)
                    return true;
                else
                    return false;
            }
        }
        public bool IsOpenRequestForVolunteer
        {
            get
            {
                if (DonationRequestOrganization != null && DonationRequestOrganization.Volunteer == null || DonationRequestOrganization.Volunteer.Id == 0)
                    return true;
                else
                    return false;
            }
        }
        public bool CanAccessRequestThread
        {
            get
            {
                if (LoggedInMemberId != 0 && ((DonationRequestOrganization != null && DonationRequestOrganization.Moderator != null && DonationRequestOrganization.Moderator.Id == LoggedInMemberId) || (CreatedBy == LoggedInMemberId)))
                    return true;
                else
                    return false;
            }
        }
        public bool CanUpdateStatus
        {
            get
            {
                if (DonationRequestOrganization != null && (DonationRequestOrganization.Status == StatusCatalog.Delivered || DonationRequestOrganization.Status == StatusCatalog.Rejected))
                    return false;
                else
                    return true;
            }
        }
    }

    public class DonationRequestSearchModel : BaseSearchModel
    {
        public DonationRequestSearchModel()
        {
            OrderByColumn = "CreatedDate";
        }
        public int? OrganizationId { get; set; }
        public int? CampaignId { get; set; }
        public DonationRequestTypeCatalog? Type { get; set; }

    }
}
