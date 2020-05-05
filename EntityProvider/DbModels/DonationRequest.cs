using System;
using System.Collections.Generic;

namespace EntityProvider.DbModels
{
    public partial class DonationRequest
    {
        public DonationRequest()
        {
            DonationRequestItems = new HashSet<DonationRequestItem>();
            DonationRequestOrganizations = new HashSet<DonationRequestOrganization>();
            DonationRequestVolunteers = new HashSet<DonationRequestVolunteer>();
        }

        public int Id { get; set; }
        public int MemberId { get; set; }
        public int? Type { get; set; }
        public string Note { get; set; }
        public DateTime Date { get; set; }
        public DateTime? DueDate { get; set; }
        public string PrefferedCollectionTime { get; set; }
        public string Address { get; set; }
        public string AddressLatLong { get; set; }
        public int? CountryId { get; set; }
        public int? StateId { get; set; }
        public int? DistrictId { get; set; }
        public int? TehsilId { get; set; }
        public int? UnionCouncilId { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int? CampaignId { get; set; }

        public virtual Country Country { get; set; }
        public virtual District District { get; set; }
        public virtual Member Member { get; set; }
        public virtual State State { get; set; }
        public virtual Tehsil Tehsil { get; set; }
        public virtual UnionCouncil UnionCouncil { get; set; }
        public virtual ICollection<DonationRequestItem> DonationRequestItems { get; set; }
        public virtual ICollection<DonationRequestOrganization> DonationRequestOrganizations { get; set; }
        public virtual ICollection<DonationRequestVolunteer> DonationRequestVolunteers { get; set; }
    }
}
