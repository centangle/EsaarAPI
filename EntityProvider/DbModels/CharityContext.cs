using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EntityProvider.DbModels
{
    public partial class CharityContext : DbContext
    {
        public CharityContext()
        {
        }

        public CharityContext(DbContextOptions<CharityContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Address> Addresses { get; set; }
        public virtual DbSet<Attachment> Attachments { get; set; }
        public virtual DbSet<Campaign> Campaigns { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<District> Districts { get; set; }
        public virtual DbSet<DonationRequest> DonationRequests { get; set; }
        public virtual DbSet<DonationRequestItem> DonationRequestItems { get; set; }
        public virtual DbSet<DonationRequestOrganization> DonationRequestOrganizations { get; set; }
        public virtual DbSet<DonationRequestOrganizationItem> DonationRequestOrganizationItems { get; set; }
        public virtual DbSet<DonationRequestVolunteer> DonationRequestVolunteers { get; set; }
        public virtual DbSet<EntityRegion> EntityRegions { get; set; }
        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<ExceptionLog> ExceptionLogs { get; set; }
        public virtual DbSet<Item> Items { get; set; }
        public virtual DbSet<Log> Logs { get; set; }
        public virtual DbSet<Member> Members { get; set; }
        public virtual DbSet<MigrationHistory> MigrationHistories { get; set; }
        public virtual DbSet<Organization> Organizations { get; set; }
        public virtual DbSet<OrganizationAccount> OrganizationAccounts { get; set; }
        public virtual DbSet<OrganizationItem> OrganizationItems { get; set; }
        public virtual DbSet<OrganizationMember> OrganizationMembers { get; set; }
        public virtual DbSet<OrganizationOffice> OrganizationOffices { get; set; }
        public virtual DbSet<OrganizationRequest> OrganizationRequests { get; set; }
        public virtual DbSet<PackageItem> PackageItems { get; set; }
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
        public virtual DbSet<RequestThread> RequestThreads { get; set; }
        public virtual DbSet<State> States { get; set; }
        public virtual DbSet<Tehsil> Tehsils { get; set; }
        public virtual DbSet<UnionCouncil> UnionCouncils { get; set; }
        public virtual DbSet<Uom> Uoms { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=localhost;Database=Charity;Trusted_Connection=True;", x => x.UseNetTopologySuite());
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Address>(entity =>
            {
                entity.ToTable("Address");

                entity.Property(e => e.AddressLine1).HasMaxLength(1000);

                entity.Property(e => e.AddressLine2).HasMaxLength(1000);

                entity.Property(e => e.Email).HasMaxLength(100);

                entity.Property(e => e.MobileNo)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Name).HasMaxLength(250);

                entity.Property(e => e.NativeName).HasMaxLength(250);

                entity.Property(e => e.PhoneNo)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.ZipCode).HasMaxLength(250);

                entity.HasOne(d => d.Country)
                    .WithMany(p => p.Addresses)
                    .HasForeignKey(d => d.CountryId)
                    .HasConstraintName("FK_Address_Country");

                entity.HasOne(d => d.District)
                    .WithMany(p => p.Addresses)
                    .HasForeignKey(d => d.DistrictId)
                    .HasConstraintName("FK_Address_District");

                entity.HasOne(d => d.State)
                    .WithMany(p => p.Addresses)
                    .HasForeignKey(d => d.StateId)
                    .HasConstraintName("FK_Address_State");

                entity.HasOne(d => d.Tehsil)
                    .WithMany(p => p.Addresses)
                    .HasForeignKey(d => d.TehsilId)
                    .HasConstraintName("FK_Address_Tehsil");

                entity.HasOne(d => d.UnionCouncil)
                    .WithMany(p => p.Addresses)
                    .HasForeignKey(d => d.UnionCouncilId)
                    .HasConstraintName("FK_Address_UnionCouncil");
            });

            modelBuilder.Entity<Attachment>(entity =>
            {
                entity.ToTable("Attachment");

                entity.Property(e => e.FileExtension)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Note).HasColumnType("text");

                entity.Property(e => e.OriginalFileName)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.SystemFileName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Url)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Campaign>(entity =>
            {
                entity.ToTable("Campaign");

                entity.Property(e => e.ImageUrl)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.NativeName).HasMaxLength(100);

                entity.HasOne(d => d.Event)
                    .WithMany(p => p.Campaigns)
                    .HasForeignKey(d => d.EventId)
                    .HasConstraintName("FK_Campaign_Event");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.Campaigns)
                    .HasForeignKey(d => d.OrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Campaign_Organization");
            });

            modelBuilder.Entity<Country>(entity =>
            {
                entity.ToTable("Country");

                entity.Property(e => e.Geometry).HasColumnType("geometry");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.NativeName).HasMaxLength(100);
            });

            modelBuilder.Entity<District>(entity =>
            {
                entity.ToTable("District");

                entity.Property(e => e.Geometry).HasColumnType("geometry");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.NativeName).HasMaxLength(100);

                entity.HasOne(d => d.State)
                    .WithMany(p => p.Districts)
                    .HasForeignKey(d => d.StateId)
                    .HasConstraintName("FK_District_State");
            });

            modelBuilder.Entity<DonationRequest>(entity =>
            {
                entity.ToTable("DonationRequest");

                entity.Property(e => e.AddressLatLong)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.DueDate).HasColumnType("datetime");

                entity.Property(e => e.PrefferedCollectionTime).HasMaxLength(100);

                entity.HasOne(d => d.Country)
                    .WithMany(p => p.DonationRequests)
                    .HasForeignKey(d => d.CountryId)
                    .HasConstraintName("FK_Request_Country");

                entity.HasOne(d => d.District)
                    .WithMany(p => p.DonationRequests)
                    .HasForeignKey(d => d.DistrictId)
                    .HasConstraintName("FK_Request_District");

                entity.HasOne(d => d.Member)
                    .WithMany(p => p.DonationRequests)
                    .HasForeignKey(d => d.MemberId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Request_Beneficiary");

                entity.HasOne(d => d.State)
                    .WithMany(p => p.DonationRequests)
                    .HasForeignKey(d => d.StateId)
                    .HasConstraintName("FK_Request_State");

                entity.HasOne(d => d.Tehsil)
                    .WithMany(p => p.DonationRequests)
                    .HasForeignKey(d => d.TehsilId)
                    .HasConstraintName("FK_Request_Tehsil");

                entity.HasOne(d => d.UnionCouncil)
                    .WithMany(p => p.DonationRequests)
                    .HasForeignKey(d => d.UnionCouncilId)
                    .HasConstraintName("FK_Request_UnionCouncil");
            });

            modelBuilder.Entity<DonationRequestItem>(entity =>
            {
                entity.ToTable("DonationRequestItem");

                entity.Property(e => e.DueDate).HasColumnType("datetime");

                entity.Property(e => e.QuantityUom).HasColumnName("QuantityUOM");

                entity.HasOne(d => d.DonationRequest)
                    .WithMany(p => p.DonationRequestItems)
                    .HasForeignKey(d => d.DonationRequestId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RequestItem_Request");

                entity.HasOne(d => d.Item)
                    .WithMany(p => p.DonationRequestItems)
                    .HasForeignKey(d => d.ItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RequestItem_Item");

                entity.HasOne(d => d.QuantityUomNavigation)
                    .WithMany(p => p.DonationRequestItems)
                    .HasForeignKey(d => d.QuantityUom)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RequestItem_UOM");
            });

            modelBuilder.Entity<DonationRequestOrganization>(entity =>
            {
                entity.ToTable("DonationRequestOrganization");

                entity.HasOne(d => d.DonationRequest)
                    .WithMany(p => p.DonationRequestOrganizations)
                    .HasForeignKey(d => d.DonationRequestId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RequestOrganization_Request");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.DonationRequestOrganizations)
                    .HasForeignKey(d => d.OrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RequestOrganization_Organization");
            });

            modelBuilder.Entity<DonationRequestOrganizationItem>(entity =>
            {
                entity.ToTable("DonationRequestOrganizationItem");

                entity.Property(e => e.CollectedQuantityUom).HasColumnName("CollectedQuantityUOM");

                entity.Property(e => e.CollectionLatLong)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.DeliveredQuantityUom).HasColumnName("DeliveredQuantityUOM");

                entity.Property(e => e.DeliveryLatLong)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.QuantityUom).HasColumnName("QuantityUOM");
            });

            modelBuilder.Entity<DonationRequestVolunteer>(entity =>
            {
                entity.ToTable("DonationRequestVolunteer");

                entity.HasOne(d => d.DonationRequest)
                    .WithMany(p => p.DonationRequestVolunteers)
                    .HasForeignKey(d => d.DonationRequestId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RequestVolunteer_RequestVolunteer");

                entity.HasOne(d => d.Volunteer)
                    .WithMany(p => p.DonationRequestVolunteers)
                    .HasForeignKey(d => d.VolunteerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RequestVolunteer_Member");
            });

            modelBuilder.Entity<EntityRegion>(entity =>
            {
                entity.ToTable("EntityRegion");

                entity.HasOne(d => d.Country)
                    .WithMany(p => p.EntityRegions)
                    .HasForeignKey(d => d.CountryId)
                    .HasConstraintName("FK_EntityRegion_Country");

                entity.HasOne(d => d.District)
                    .WithMany(p => p.EntityRegions)
                    .HasForeignKey(d => d.DistrictId)
                    .HasConstraintName("FK_EntityRegion_District");

                entity.HasOne(d => d.State)
                    .WithMany(p => p.EntityRegions)
                    .HasForeignKey(d => d.StateId)
                    .HasConstraintName("FK_EntityRegion_State");

                entity.HasOne(d => d.Tehsil)
                    .WithMany(p => p.EntityRegions)
                    .HasForeignKey(d => d.TehsilId)
                    .HasConstraintName("FK_EntityRegion_Tehsil");

                entity.HasOne(d => d.UnionCouncil)
                    .WithMany(p => p.EntityRegions)
                    .HasForeignKey(d => d.UnionCouncilId)
                    .HasConstraintName("FK_EntityRegion_UnionCouncil");
            });

            modelBuilder.Entity<Event>(entity =>
            {
                entity.ToTable("Event");

                entity.Property(e => e.ImageUrl)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.NativeName).HasMaxLength(100);
            });

            modelBuilder.Entity<ExceptionLog>(entity =>
            {
                entity.ToTable("ExceptionLog");

                entity.Property(e => e.Exception).HasColumnType("text");

                entity.Property(e => e.FilePath).HasMaxLength(1000);

                entity.Property(e => e.MethodName).HasMaxLength(250);

                entity.Property(e => e.SerializedModel).HasColumnType("text");
            });

            modelBuilder.Entity<Item>(entity =>
            {
                entity.ToTable("Item");

                entity.Property(e => e.DefaultUom).HasColumnName("DefaultUOM");

                entity.Property(e => e.ImageUrl)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.NativeName).HasMaxLength(100);

                entity.HasOne(d => d.Parent)
                    .WithMany(p => p.InverseParent)
                    .HasForeignKey(d => d.ParentId)
                    .HasConstraintName("FK_ChildItem_ParentItem");
            });

            modelBuilder.Entity<Log>(entity =>
            {
                entity.ToTable("Log");

                entity.Property(e => e.Pk)
                    .HasColumnName("PK")
                    .HasMaxLength(128);

                entity.Property(e => e.SerializedModel).HasColumnType("text");

                entity.Property(e => e.TableName).HasMaxLength(100);
            });

            modelBuilder.Entity<Member>(entity =>
            {
                entity.ToTable("Member");

                entity.Property(e => e.AuthUserId).HasMaxLength(128);

                entity.Property(e => e.IdentificationNo)
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.NativeName).HasMaxLength(100);
            });

            modelBuilder.Entity<MigrationHistory>(entity =>
            {
                entity.HasKey(e => new { e.MigrationId, e.ContextKey })
                    .HasName("PK_dbo.__MigrationHistory");

                entity.ToTable("__MigrationHistory");

                entity.Property(e => e.MigrationId).HasMaxLength(150);

                entity.Property(e => e.ContextKey).HasMaxLength(300);

                entity.Property(e => e.Model).IsRequired();

                entity.Property(e => e.ProductVersion)
                    .IsRequired()
                    .HasMaxLength(32);
            });

            modelBuilder.Entity<Organization>(entity =>
            {
                entity.ToTable("Organization");

                entity.Property(e => e.LogoUrl)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.NativeName).HasMaxLength(500);

                entity.HasOne(d => d.Parent)
                    .WithMany(p => p.InverseParent)
                    .HasForeignKey(d => d.ParentId)
                    .HasConstraintName("FK_Organization_Organization");
            });

            modelBuilder.Entity<OrganizationAccount>(entity =>
            {
                entity.ToTable("OrganizationAccount");

                entity.Property(e => e.AccountNo)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(e => e.NativeName)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.OrganizationAccounts)
                    .HasForeignKey(d => d.OrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrganizationAccount_Organization");
            });

            modelBuilder.Entity<OrganizationItem>(entity =>
            {
                entity.ToTable("OrganizationItem");

                entity.Property(e => e.CampaignItemUom).HasColumnName("CampaignItemUOM");

                entity.HasOne(d => d.Item)
                    .WithMany(p => p.OrganizationItems)
                    .HasForeignKey(d => d.ItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrganizationItem_Item");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.OrganizationItems)
                    .HasForeignKey(d => d.OrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrganizationItem_Organization");
            });

            modelBuilder.Entity<OrganizationMember>(entity =>
            {
                entity.ToTable("OrganizationMember");

                entity.HasOne(d => d.Member)
                    .WithMany(p => p.OrganizationMembers)
                    .HasForeignKey(d => d.MemberId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrganizationMember_Member");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.OrganizationMembers)
                    .HasForeignKey(d => d.OrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrganizationMember_Organization");
            });

            modelBuilder.Entity<OrganizationOffice>(entity =>
            {
                entity.ToTable("OrganizationOffice");

                entity.Property(e => e.AddressLatLong)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.NativeName).HasMaxLength(250);

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.OrganizationOffices)
                    .HasForeignKey(d => d.OrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrganizationOffice_Organization");
            });

            modelBuilder.Entity<OrganizationRequest>(entity =>
            {
                entity.ToTable("OrganizationRequest");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.OrganizationRequests)
                    .HasForeignKey(d => d.OrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrganizationRequest_Organization");
            });

            modelBuilder.Entity<PackageItem>(entity =>
            {
                entity.ToTable("PackageItem");

                entity.Property(e => e.ItemUom).HasColumnName("ItemUOM");

                entity.HasOne(d => d.Item)
                    .WithMany(p => p.PackageItemItems)
                    .HasForeignKey(d => d.ItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PackageItems_Item");

                entity.HasOne(d => d.ItemUomNavigation)
                    .WithMany(p => p.PackageItems)
                    .HasForeignKey(d => d.ItemUom)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PackageItems_UOM");

                entity.HasOne(d => d.Package)
                    .WithMany(p => p.PackageItemPackages)
                    .HasForeignKey(d => d.PackageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PackageItems_Package");
            });

            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("RefreshToken");

                entity.Property(e => e.ExpiredTime).HasColumnType("datetime");

                entity.Property(e => e.Id)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.IssuedTime).HasColumnType("datetime");

                entity.Property(e => e.ProtectedTicket).HasMaxLength(4000);

                entity.Property(e => e.UserId).HasMaxLength(128);
            });

            modelBuilder.Entity<RequestThread>(entity =>
            {
                entity.ToTable("RequestThread");
            });

            modelBuilder.Entity<State>(entity =>
            {
                entity.ToTable("State");

                entity.Property(e => e.Geometry).HasColumnType("geometry");

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.NativeName).HasMaxLength(100);

                entity.HasOne(d => d.Country)
                    .WithMany(p => p.States)
                    .HasForeignKey(d => d.CountryId)
                    .HasConstraintName("FK_State_Country");
            });

            modelBuilder.Entity<Tehsil>(entity =>
            {
                entity.ToTable("Tehsil");

                entity.Property(e => e.Geometry).HasColumnType("geometry");

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.NativeName).HasMaxLength(100);

                entity.HasOne(d => d.District)
                    .WithMany(p => p.Tehsils)
                    .HasForeignKey(d => d.DistrictId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Tehsil_District");
            });

            modelBuilder.Entity<UnionCouncil>(entity =>
            {
                entity.ToTable("UnionCouncil");

                entity.Property(e => e.Geometry).HasColumnType("geometry");

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.NativeName).HasMaxLength(100);

                entity.HasOne(d => d.Tehsil)
                    .WithMany(p => p.UnionCouncils)
                    .HasForeignKey(d => d.TehsilId)
                    .HasConstraintName("FK_UnionCouncil_Tehsil");
            });

            modelBuilder.Entity<Uom>(entity =>
            {
                entity.ToTable("UOM");

                entity.Property(e => e.Abbreviation).HasMaxLength(30);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.NativeName).HasMaxLength(100);

                entity.HasOne(d => d.Parent)
                    .WithMany(p => p.InverseParent)
                    .HasForeignKey(d => d.ParentId)
                    .HasConstraintName("FK_UOM_UOM");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
