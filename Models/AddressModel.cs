using Catalogs;
using Models.Base;
using Models.BriefModel;

namespace Models
{
    public class AddressModel : BaseModel
    {
        public AddressModel()
        {
            Entity = new BaseBriefModel();
            Country = new BaseBriefModel();
            State = new BaseBriefModel();
            District = new BaseBriefModel();
            Tehsil = new BaseBriefModel();
            UnionCouncil = new BaseBriefModel();
        }
        public string Name { get; set; }
        public string NativeName { get; set; }
        public BaseBriefModel Entity { get; set; }
        public MemberTypeCatalog EntityType { get; set; }
        public AddressTypeCatalog Type { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string ZipCode { get; set; }
        public string PhoneNo { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public BaseBriefModel Country { get; set; }
        public BaseBriefModel State { get; set; }
        public BaseBriefModel District { get; set; }
        public BaseBriefModel Tehsil { get; set; }
        public BaseBriefModel UnionCouncil { get; set; }
        
    }
}
