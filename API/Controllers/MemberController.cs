using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.BriefModel;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MemberController : BaseController
    {
        private readonly Logic _logic;

        public MemberController(Logic logic)
        {
            _logic = logic;
        }
        [HttpGet]
        [Route("GetMemberForDD")]
        public async Task<List<MemberBriefModel>> GetMemberForDD(string filter)
        {

            return await _logic.GetMemberForDD(filter);
        }
    }
}