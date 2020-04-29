using Catalogs;
using Models.Base;
using Models.BriefModel;
using Models.Interfaces;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Models
{
    public class UOMModel : BaseModel, IName, ITree<UOMModel>, IPeripheral
    {
        public UOMModel()
        {
            NoOfBaseUnit = 1;
            children = new List<UOMModel>();
            Type = UnitTypeCatalog.Item;
            Root = new BaseBriefModel();
            Parent = new BaseBriefModel();
        }
        [IgnoreDataMember]
        public int? ParentId
        {
            get
            {
                if (Parent != null)
                    return Parent.Id;
                else
                    return 0;
            }
            set { }
        }
        [IgnoreDataMember]
        public int? RootId
        {
            get
            {
                if (Root != null)
                    return Root.Id;
                else
                    return 0;
            }
            set { }
        }
        public BaseBriefModel Parent { get; set; }
        public BaseBriefModel Root { get; set; }
        [Required]
        public string Name { get; set; }
        public string NativeName { get; set; }
        public string Abbreviation { get; set; }
        public double NoOfBaseUnit { get; set; }
        public ICollection<UOMModel> children { get; set; }
        public bool IsPeripheral { get; set; }
        [IgnoreDataMember]
        public UnitTypeCatalog Type { get; set; }
    }
    public class UOMSearchModel : BaseSearchModel
    {
        public string Name { get; set; }
        public int? ParentId { get; set; }
    }
}
