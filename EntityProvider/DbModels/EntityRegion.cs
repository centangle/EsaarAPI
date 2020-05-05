using System;
using System.Collections.Generic;

namespace EntityProvider.DbModels
{
    public partial class EntityRegion
    {
        public int Id { get; set; }
        public int EntityId { get; set; }
        public int EntityType { get; set; }
        public int RegionLevel { get; set; }
        public int RegionId { get; set; }
        public bool IsActive { get; set; }
        public bool IsApproved { get; set; }
        public int? ApprovedBy { get; set; }
        public int? CountryId { get; set; }
        public int? StateId { get; set; }
        public int? DistrictId { get; set; }
        public int? TehsilId { get; set; }
        public int? UnionCouncilId { get; set; }
        public int? RequestId { get; set; }
        public int? RequestType { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        public virtual Country Country { get; set; }
        public virtual District District { get; set; }
        public virtual State State { get; set; }
        public virtual Tehsil Tehsil { get; set; }
        public virtual UnionCouncil UnionCouncil { get; set; }
    }
}
