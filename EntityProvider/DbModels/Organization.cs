using System;
using System.Collections.Generic;

namespace EntityProvider.DbModels
{
    public partial class Organization
    {
        public Organization()
        {
            Campaigns = new HashSet<Campaign>();
            DonationRequestOrganizations = new HashSet<DonationRequestOrganization>();
            InverseParent = new HashSet<Organization>();
            Items = new HashSet<Item>();
            OrganizationAccounts = new HashSet<OrganizationAccount>();
            OrganizationItems = new HashSet<OrganizationItem>();
            OrganizationMembers = new HashSet<OrganizationMember>();
            OrganizationOffices = new HashSet<OrganizationOffice>();
            OrganizationRequests = new HashSet<OrganizationRequest>();
        }

        public int Id { get; set; }
        public int? RootId { get; set; }
        public int? ParentId { get; set; }
        public string Name { get; set; }
        public string NativeName { get; set; }
        public string Description { get; set; }
        public string LogoUrl { get; set; }
        public int? Type { get; set; }
        public int OwnedBy { get; set; }
        public bool IsVerified { get; set; }
        public bool IsPeripheral { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        public virtual Organization Parent { get; set; }
        public virtual ICollection<Campaign> Campaigns { get; set; }
        public virtual ICollection<DonationRequestOrganization> DonationRequestOrganizations { get; set; }
        public virtual ICollection<Organization> InverseParent { get; set; }
        public virtual ICollection<Item> Items { get; set; }
        public virtual ICollection<OrganizationAccount> OrganizationAccounts { get; set; }
        public virtual ICollection<OrganizationItem> OrganizationItems { get; set; }
        public virtual ICollection<OrganizationMember> OrganizationMembers { get; set; }
        public virtual ICollection<OrganizationOffice> OrganizationOffices { get; set; }
        public virtual ICollection<OrganizationRequest> OrganizationRequests { get; set; }
    }
}
