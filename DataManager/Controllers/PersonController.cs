using BusinessLogic;
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
    public class PersonController : BaseController
    {
        public async Task<List<BriefModel>> GetPersonForDD(string name)
        {
            var _logic = new Logic(CurrentPersonId);
            return await _logic.GetPersonForDD(name);
        }
    }
}
