using Catalogs;
using Models.Base;
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
            Childrens = new List<ItemModel>();
        }
        [Required]
        public string Name { get; set; }
        public string NativeName { get; set; }
        public int? ParentId { get; set; }
        public BriefModel DefaultUOM { get; set; }
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
        public bool IsCartItem { get; set; }

        public ICollection<ItemModel> Childrens { get; set; }
    }
}
