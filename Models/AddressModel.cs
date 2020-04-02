using Catalogs;
using Models.Base;
using System;

namespace Models
{
    public class AddressModel : BaseModel
    {
        public AddressModel()
        {
            Entity = new BriefModel();
            Country = new BriefModel();
            State = new BriefModel();
            District = new BriefModel();
            Tehsil = new BriefModel();
            UnionCouncil = new BriefModel();
        }
        public string Name { get; set; }
        public string NativeName { get; set; }
        public BriefModel Entity { get; set; }
        public EntityTypeCatalog EntityType { get; set; }
        public AddressTypeCatalog Type { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string ZipCode { get; set; }
        public string PhoneNo { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public BriefModel Country { get; set; }
        public BriefModel State { get; set; }
        public BriefModel District { get; set; }
        public BriefModel Tehsil { get; set; }
        public BriefModel UnionCouncil { get; set; }
        
    }
}
