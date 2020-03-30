using Models;
using System.Threading.Tasks;
using System.Web.Http;

namespace DataManager.Controllers
{
    [Authorize]
    public class UOMController : BaseController
    {
        public async Task<PaginatedResultModel<UOMModel>> Get(string name = null, bool forDropDown = false)
        {
            if (forDropDown)
                return await logic.GetUOMForDD(new UOMSearchModel { Name = name });
            else
                return await logic.GetUOM(new UOMSearchModel { Name = name });
        }

        public async Task<int> Post(UOMModel model)
        {
            return await logic.AddUOM(model);
        }

        public async Task<bool> Put(UOMModel model)
        {
            return await logic.UpdateUOM(model);
        }

        public async Task<bool> Delete(int id)
        {
            return await logic.DeleteUOM(id);
        }
    }
}
