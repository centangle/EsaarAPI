using System;
using System.Collections.Generic;

namespace EntityProvider.DbModels
{
    public partial class Member
    {
        public Member()
        {
            DonationRequestVolunteers = new HashSet<DonationRequestVolunteer>();
            DonationRequests = new HashSet<DonationRequest>();
            OrganizationMembers = new HashSet<OrganizationMember>();
        }

        public int Id { get; set; }
        public string AuthUserId { get; set; }
        public string Name { get; set; }
        public string NativeName { get; set; }
        public string IdentificationNo { get; set; }
        public int Type { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        public virtual ICollection<DonationRequestVolunteer> DonationRequestVolunteers { get; set; }
        public virtual ICollection<DonationRequest> DonationRequests { get; set; }
        public virtual ICollection<OrganizationMember> OrganizationMembers { get; set; }
    }
}
