using Catalogs;
using Models.Base;
using Models.Interfaces;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Models
{
    public class UOMModel : BaseModel, ITree<UOMModel>
    {
        public UOMModel()
        {
            NoOfBaseUnit = 1;
            Children = new List<UOMModel>();
            Type = UnitTypeCatalog.Item;
        }
        public int? ParentId { get; set; }

        [Required]
        public string Name { get; set; }
        public string NativeName { get; set; }
        public string Abbreviation { get; set; }
        public double NoOfBaseUnit { get; set; }
        public ICollection<UOMModel> Children { get; set; }
        [IgnoreDataMember]
        public UnitTypeCatalog Type { get; set; }
    }
    public class UOMSearchModel : BaseSearchModel
    {
        public string Name { get; set; }
        public int? ParentId { get; set; }
    }
}
