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
        public async Task<RequestThreadModel> GetRequestThread(int id)
        {
            return await _dataAccess.GetRequestThread(id);
        }
        public async Task<PaginatedResultModel<RequestThreadModel>> GetRequestThreads(RequestThreadSearchModel filters)
        {
            return await _dataAccess.GetRequestThreads(filters);
        }
        public async Task<int> AddRequestThread(RequestThreadModel model)
        {
            return await _dataAccess.AddRequestThread(model);
        }

        public async Task<bool> UpdateRequestThread(RequestThreadModel model)
        {
            return await _dataAccess.UpdateRequestThread(model);
        }

        public async Task<bool> DeleteRequestThread(int id)
        {
            return await _dataAccess.DeleteRequestThread(id);
        }
    }
}
