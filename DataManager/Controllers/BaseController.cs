using BusinessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DataManager.Controllers
{
    public class BaseController : ApiController
    {
        protected Logic logic = new Logic();
    }
}
