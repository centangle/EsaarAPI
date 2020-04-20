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
        public async Task<int> CreateEvent(EventModel model)
        {
            return await _dataAccess.CreateEvent(model);
        }
        public async Task<bool> UpdateEvent(EventModel model)
        {
            return await _dataAccess.UpdateEvent(model);
        }
        public async Task<bool> DeleteEvent(int id)
        {
            return await _dataAccess.DeleteEvent(id);
        }
        public async Task<EventModel> GetEvent(int id)
        {
            return await _dataAccess.GetEvent(id);
        }
        public async Task<PaginatedResultModel<EventModel>> GetEvents(EventSearchModel filters)
        {
            return await _dataAccess.GetEvents(filters);
        }
    }
}
