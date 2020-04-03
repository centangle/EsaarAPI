using BusinessLogic;
using Models.BriefModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataManager.Controllers
{
    public class PersonController : BaseController
    {
        public async Task<List<PersonBriefModel>> GetPersonForDD(string filter)
        {
            var _logic = new Logic(CurrentPersonId);
            return await _logic.GetPersonForDD(filter);
        }
    }
}
