using Catalogs;
using Models.Base;
using Models.Interfaces;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Models
{
    public class UOMModel : BaseModel, IName, IOneLevelTree<UOMModel>
    {
        public UOMModel()
        {
            NoOfBaseUnit = 1;
            children = new List<UOMModel>();
            Type = UnitTypeCatalog.Item;
        }
        public int? ParentId { get; set; }

        [Required]
        public string Name { get; set; }
        public string NativeName { get; set; }
        public string Abbreviation { get; set; }
        public double NoOfBaseUnit { get; set; }
        public ICollection<UOMModel> children { get; set; }
        [IgnoreDataMember]
        public UnitTypeCatalog Type { get; set; }
    }
    public class UOMSearchModel : BaseSearchModel
    {
        public string Name { get; set; }
        public int? ParentId { get; set; }
    }
}
