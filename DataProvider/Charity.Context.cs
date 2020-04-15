﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class CharityEntities : DbContext
    {
        public CharityEntities()
            : base("name=CharityEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<C__MigrationHistory> C__MigrationHistory { get; set; }
        public virtual DbSet<Address> Addresses { get; set; }
        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<Attachment> Attachments { get; set; }
        public virtual DbSet<Campaign> Campaigns { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<District> Districts { get; set; }
        public virtual DbSet<DonationRequest> DonationRequests { get; set; }
        public virtual DbSet<DonationRequestItem> DonationRequestItems { get; set; }
        public virtual DbSet<DonationRequestOrganization> DonationRequestOrganizations { get; set; }
        public virtual DbSet<DonationRequestOrganizationItem> DonationRequestOrganizationItems { get; set; }
        public virtual DbSet<DonationRequestVolunteer> DonationRequestVolunteers { get; set; }
        public virtual DbSet<ExceptionLog> ExceptionLogs { get; set; }
        public virtual DbSet<Item> Items { get; set; }
        public virtual DbSet<Log> Logs { get; set; }
        public virtual DbSet<Member> Members { get; set; }
        public virtual DbSet<Organization> Organizations { get; set; }
        public virtual DbSet<OrganizationItem> OrganizationItems { get; set; }
        public virtual DbSet<OrganizationMember> OrganizationMembers { get; set; }
        public virtual DbSet<OrganizationRequest> OrganizationRequests { get; set; }
        public virtual DbSet<PackageItem> PackageItems { get; set; }
        public virtual DbSet<RequestThread> RequestThreads { get; set; }
        public virtual DbSet<State> States { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<Tehsil> Tehsils { get; set; }
        public virtual DbSet<UnionCouncil> UnionCouncils { get; set; }
        public virtual DbSet<UOM> UOMs { get; set; }
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
    
        public virtual ObjectResult<spGetItemWithChildren_Result> spGetItemWithChildren(Nullable<int> id)
        {
            var idParameter = id.HasValue ?
                new ObjectParameter("Id", id) :
                new ObjectParameter("Id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<spGetItemWithChildren_Result>("spGetItemWithChildren", idParameter);
        }
    }
}
