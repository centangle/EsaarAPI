using Catalogs;
using System.Collections.Generic;

namespace Models
{
    public class PackageModel : ItemBaseModel
    {
        public PackageModel()
        {
            Type = ItemTypeCatalog.Package;
            IsPeripheral = true;
        }
        public List<PackageItemModel> Items { get; set; }
    }
}
