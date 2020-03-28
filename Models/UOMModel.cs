using Catalogs;
using Models.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class UOMModel : BaseModel
    {
        public UOMModel()
        {
            NoOfBaseUnit = 1;
            ChildUOMS = new List<UOMModel>();
            Type = UnitTypeCatalog.Item;
        }
        public int? ParentId { get; set; }

        [Required]
        public string Name { get; set; }
        public string NativeName { get; set; }
        public string Abbreviation { get; set; }
        public double NoOfBaseUnit { get; set; }
        public List<UOMModel> ChildUOMS { get; set; }
        public UnitTypeCatalog Type { get; set; }
    }
    public class UOMSearchModel : BaseSearchModel
    {
        public string Name { get; set; }
        public int? ParentId { get; set; }
    }
}
