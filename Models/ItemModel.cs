using Catalogs;
using Models.Base;
using Models.BriefModel;
using Models.Interfaces;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Models
{
    public class ItemBaseModel : BaseModel, IImage, IName
    {
        [Required]
        public string Name { get; set; }
        public string NativeName { get; set; }
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
        public UOMBriefModel DefaultUOM { get; set; }
        public BaseBriefModel Organization { get; set; }
        [IgnoreDataMember]
        public ItemTypeCatalog Type
        {
            get; set;
        }
        public double? Worth { get; set; }
        [IgnoreDataMember]
        public string BaseFolder
        {
            get
            {
                return "Items";
            }
        }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string ImageInBase64 { get; set; }
        public bool IsPeripheral { get; set; }
    }
    public class ItemModel : ItemBaseModel, ITree<ItemModel>, IPeripheral
    {
        public ItemModel()
        {
            children = new List<ItemModel>();
            Organization = new BaseBriefModel();
            DefaultUOM = new UOMBriefModel();
            Root = new BaseBriefModel();
            Type = ItemTypeCatalog.General;
        }
        public ICollection<ItemModel> children { get; set; }
    }
}
