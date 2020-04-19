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
    
    public partial class EntityRegion
    {
        public int Id { get; set; }
        public int EntityId { get; set; }
        public int EntityType { get; set; }
        public int RegionLevel { get; set; }
        public int RegionId { get; set; }
        public bool IsActive { get; set; }
        public bool IsApproved { get; set; }
        public Nullable<int> ApprovedBy { get; set; }
        public Nullable<int> CountryId { get; set; }
        public Nullable<int> StateId { get; set; }
        public Nullable<int> DistrictId { get; set; }
        public Nullable<int> TehsilId { get; set; }
        public Nullable<int> UnionCouncilId { get; set; }
        public Nullable<int> RequestId { get; set; }
        public Nullable<int> RequestType { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public int UpdatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
    
        public virtual Country Country { get; set; }
        public virtual District District { get; set; }
        public virtual State State { get; set; }
        public virtual Tehsil Tehsil { get; set; }
        public virtual UnionCouncil UnionCouncil { get; set; }
    }
}
