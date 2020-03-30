using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public partial class Logic
    {
        public async Task<int> AddUOM(UOMModel model)
        {
            return await DA.AddUOM(model);
        }

        public async Task<bool> UpdateUOM(UOMModel model)
        {
            return await DA.UpdateUOM(model);
        }

        public async Task<bool> DeleteUOM(int Id)
        {
            return await DA.DeleteUOM(Id);
        }

        public async Task<PaginatedResultModel<UOMModel>> GetUOM(UOMSearchModel filters)
        {
            return await DA.GetUOM(filters);
        }

        public async Task<PaginatedResultModel<UOMModel>> GetUOMForDD(UOMSearchModel filters)
        {
            return await DA.GetUOMForDD(filters);
        }

    }
}
