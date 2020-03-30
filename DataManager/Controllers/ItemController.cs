using DataManager.Catalogs;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace DataManager.Controllers
{
    public class ItemController : BaseController
    {

        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Item/5
        public async Task<IEnumerable<ItemModel>> Get(int id)
        {
            return await logic.GetSingleItemHierarchy(id);
        }

        public async Task<int> Post(ItemModel model, ItemUpdationMethodCatalog creationMethod)
        {
            if (creationMethod == ItemUpdationMethodCatalog.Single)
                return await logic.AddSingleItem(model);
            else if (creationMethod == ItemUpdationMethodCatalog.SingleWithMultipleChild)
                return await logic.AddMultipleChildItem(model);
            else
                return 0;
        }

        public async Task<bool> Put(ItemModel model, ItemUpdationMethodCatalog updationMethod)
        {
            if (updationMethod == ItemUpdationMethodCatalog.Single)
                return await logic.UpdateSingleItem(model);
            else if (updationMethod == ItemUpdationMethodCatalog.SingleWithMultipleChild)
                return await logic.UpdateMultipleChildItem(model);
            else
                return false;
        }

        // DELETE: api/Item/5
        public void Delete(int id)
        {
        }
    }
}
