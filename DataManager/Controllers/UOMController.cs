using BusinessLogic;
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
            var _logic = new Logic(CurrentPersonId);
            if (forDropDown)
                return await _logic.GetUOMForDD(new UOMSearchModel { Name = name });
            else
                return await _logic.GetUOM(new UOMSearchModel { Name = name });
        }

        public async Task<int> Post(UOMModel model)
        {
            var _logic = new Logic(CurrentPersonId);
            return await _logic.AddUOM(model);
        }

        public async Task<bool> Put(UOMModel model)
        {
            var _logic = new Logic(CurrentPersonId);
            return await _logic.UpdateUOM(model);
        }

        public async Task<bool> Delete(int id)
        {
            var _logic = new Logic(CurrentPersonId);
            return await _logic.DeleteUOM(id);
        }
    }
}
