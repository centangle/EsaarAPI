using Catalogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class PackageModel : ItemBaseModel
    {
        public PackageModel()
        {
            Type = ItemTypeCatalog.Package;
            IsPeripheral = true;
        }
        public List<PackageItemModel> children { get; set; }
    }
}
