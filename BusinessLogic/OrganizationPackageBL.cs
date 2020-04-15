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
        public async Task<int> AddPackage(PackageModel model)
        {
            return await _dataAccess.AddPackage(model);
        }
        public async Task<bool> UpdatePackage(PackageModel model)
        {
            return await _dataAccess.UpdatePackage(model);
        }
        public async Task<bool> DeletePackage(int id)
        {
            return await _dataAccess.DeletePackage(id);
        }
        public async Task<PackageModel> GetPackage(int id)
        {
            return await _dataAccess.GetPackage(id);
        }
    }
}
