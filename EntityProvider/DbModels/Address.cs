using System;
using System.Collections.Generic;

namespace EntityProvider.DbModels
{
    public partial class Address
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string NativeName { get; set; }
        public int? EntityId { get; set; }
        public int? EntityType { get; set; }
        public int? Type { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string ZipCode { get; set; }
        public string PhoneNo { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
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

        public virtual Country Country { get; set; }
        public virtual District District { get; set; }
        public virtual State State { get; set; }
        public virtual Tehsil Tehsil { get; set; }
        public virtual UnionCouncil UnionCouncil { get; set; }
    }
}
