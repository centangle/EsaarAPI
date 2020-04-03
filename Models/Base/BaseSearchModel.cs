using Catalogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Base
{
    public abstract class BaseSearchModel
    {
        public BaseSearchModel()
        {
        }
        public int RecordsPerPage { get; set; } = 15;
        public bool CalculateTotal { get; set; } = true;
        public int CurrentPage { get; set; } = 1;
        public bool DisablePagination { get; set; } = true;
        public string OrderByColumn { get; set; } = "Name";
        public PaginationOrderCatalog OrderDir { get; set; } = PaginationOrderCatalog.Asc;
    }
}
