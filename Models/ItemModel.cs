using Catalogs;
using Models.Base;
using Models.BriefModel;
using Models.Interfaces;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Models
{
    public class ItemModel : BaseModel, IImage, ITree<ItemModel>
    {
        public ItemModel()
        {
            children = new List<ItemModel>();
        }
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
        public BaseBriefModel Parent { get; set; }
        public BaseBriefModel DefaultUOM { get; set; }
        [IgnoreDataMember]
        public ItemTypeCatalog Type { get; set; }
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
        public bool IsPeripheralItem { get; set; }

        public ICollection<ItemModel> children { get; set; }
    }
}
