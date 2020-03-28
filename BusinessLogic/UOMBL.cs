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

        public async Task<bool> DeleteUOMById(int Id)
        {
            return await DA.DeleteUOMById(Id);
        }

        public async Task<PaginatedResultModel<UOMModel>> GetUOMByFilter(UOMSearchModel filters)
        {
            return await DA.GetUOMByFilter(filters);
        }

        public async Task<PaginatedResultModel<UOMModel>> GetUOMsForDDAsync(UOMSearchModel filters)
        {
            return await DA.GetUOMsForDDAsync(filters);
        }

    }
}
