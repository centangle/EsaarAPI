using DataProvider;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace BusinessLogic
{
    public partial class Logic
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        DataAccess _dataAccess;
        int _loggedInMemberId;
        public Logic()
        {
            SetDataAccess();
        }

        public Logic(int loggedInMemberId)
        {
            _loggedInMemberId = loggedInMemberId;
            SetDataAccess();
        }
        public Logic(DataAccess dataAccess, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            SetLoggedInMemberId();
            _dataAccess = dataAccess;
            SetDataAccess();
        }
        private void SetDataAccess()
        {
            _dataAccess = new DataAccess(_loggedInMemberId);
        }
        private void SetLoggedInMemberId()
        {
            //var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }


    }
}
