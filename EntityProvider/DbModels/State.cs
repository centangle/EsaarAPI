using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace EntityProvider.DbModels
{
    public partial class State
    {
        public State()
        {
            Addresses = new HashSet<Address>();
            Districts = new HashSet<District>();
            DonationRequests = new HashSet<DonationRequest>();
            EntityRegions = new HashSet<EntityRegion>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string NativeName { get; set; }
        public int CountryId { get; set; }
        public Geometry Geometry { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        public virtual Country Country { get; set; }
        public virtual ICollection<Address> Addresses { get; set; }
        public virtual ICollection<District> Districts { get; set; }
        public virtual ICollection<DonationRequest> DonationRequests { get; set; }
        public virtual ICollection<EntityRegion> EntityRegions { get; set; }
    }
}
