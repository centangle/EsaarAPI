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

        public async Task<int> CreateUOM(UOMModel model)
        {
            return await _dataAccess.CreateUOM(model);
        }
        public async Task<bool> UpdateUOM(UOMModel model)
        {
            return await _dataAccess.UpdateUOM(model);
        }
        public async Task<bool> CreateSingleUOMWithChildrens(UOMModel model)
        {
            return await _dataAccess.CreateSingleUOMWithChildrens(model);
        }
        public async Task<bool> UpdateSingleUOMWithChildren(UOMModel model)
        {
            return await _dataAccess.UpdateSingleUOMWithChildren(model);
        }
        public async Task<bool> CreateMultipleUOMsWithChildrens(List<UOMModel> uOMs)
        {
            return await _dataAccess.CreateMultipleUOMsWithChildrens(uOMs);
        }
        public async Task<bool> UpdateMultipleUOMsWithChildrens(List<UOMModel> uOMs)
        {
            return await _dataAccess.UpdateMultipleUOMsWithChildrens(uOMs);
        }
        public async Task<UOMModel> GetUOM(int id)
        {
            return await _dataAccess.GetUOM(id);
        }
        public async Task<IEnumerable<UOMModel>> GetSingleUOMHierarchy(int id, bool getHierarchicalData)
        {
            return await _dataAccess.GetSingleTreeUOM(id, getHierarchicalData);
        }
        public async Task<IEnumerable<UOMModel>> GetPeripheralUOMs()
        {
            return await _dataAccess.GetPeripheralUOMs();
        }
        public async Task<IEnumerable<UOMModel>> GetRootUOMs()
        {
            return await _dataAccess.GetRootUOMs();
        }
        public async Task<IEnumerable<UOMModel>> GetAllUOMs(bool getHierarchicalData)
        {
            return await _dataAccess.GetAllUOMs(getHierarchicalData);
        }
        public async Task<bool> DeleteUOM(int Id)
        {
            return await _dataAccess.DeleteUOM(Id);
        }

        public async Task<PaginatedResultModel<UOMModel>> GetUOM(UOMSearchModel filters)
        {
            return await _dataAccess.GetUOM(filters);
        }

        public async Task<PaginatedResultModel<UOMModel>> GetUOMForDD(UOMSearchModel filters)
        {
            return await _dataAccess.GetUOMForDD(filters);
        }

    }
}
