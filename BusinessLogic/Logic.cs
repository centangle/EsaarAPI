//using DataProvider;
using EntityProvider;
using Microsoft.AspNetCore.Http;
using System.Linq;

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
            _dataAccess = dataAccess;
            var strLoggedInMemberId = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "memberId")?.Value;
            if (strLoggedInMemberId != null)
                _loggedInMemberId = int.Parse(strLoggedInMemberId);
        }
        private void SetDataAccess()
        {
            _dataAccess = new DataAccess(_loggedInMemberId);
        }
    }
}
