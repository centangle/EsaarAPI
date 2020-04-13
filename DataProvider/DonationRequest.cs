//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataProvider
{
    using System;
    using System.Collections.Generic;
    
    public partial class DonationRequest
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DonationRequest()
        {
            this.DonationRequestItems = new HashSet<DonationRequestItem>();
            this.DonationRequestOrganizations = new HashSet<DonationRequestOrganization>();
            this.DonationRequestVolunteers = new HashSet<DonationRequestVolunteer>();
        }
    
        public int Id { get; set; }
        public int MemberId { get; set; }
        public Nullable<int> Type { get; set; }
        public string Note { get; set; }
        public System.DateTime Date { get; set; }
        public Nullable<System.DateTime> DueDate { get; set; }
        public string PrefferedCollectionTime { get; set; }
        public string Address { get; set; }
        public string AddressLatLong { get; set; }
        public Nullable<int> CountryId { get; set; }
        public Nullable<int> StateId { get; set; }
        public Nullable<int> DistrictId { get; set; }
        public Nullable<int> TehsilId { get; set; }
        public Nullable<int> UnionCouncilId { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public int UpdatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public Nullable<int> CampaignId { get; set; }
    
        public virtual Country Country { get; set; }
        public virtual District District { get; set; }
        public virtual Member Member { get; set; }
        public virtual State State { get; set; }
        public virtual Tehsil Tehsil { get; set; }
        public virtual UnionCouncil UnionCouncil { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DonationRequestItem> DonationRequestItems { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DonationRequestOrganization> DonationRequestOrganizations { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DonationRequestVolunteer> DonationRequestVolunteers { get; set; }
    }
}
