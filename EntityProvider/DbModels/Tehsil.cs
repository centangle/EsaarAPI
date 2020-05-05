using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace EntityProvider.DbModels
{
    public partial class Tehsil
    {
        public Tehsil()
        {
            Addresses = new HashSet<Address>();
            DonationRequests = new HashSet<DonationRequest>();
            EntityRegions = new HashSet<EntityRegion>();
            UnionCouncils = new HashSet<UnionCouncil>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string NativeName { get; set; }
        public int DistrictId { get; set; }
        public Geometry Geometry { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        public virtual District District { get; set; }
        public virtual ICollection<Address> Addresses { get; set; }
        public virtual ICollection<DonationRequest> DonationRequests { get; set; }
        public virtual ICollection<EntityRegion> EntityRegions { get; set; }
        public virtual ICollection<UnionCouncil> UnionCouncils { get; set; }
    }
}
