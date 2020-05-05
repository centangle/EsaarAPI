using EntityProvider.DbModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace EntityProvider
{
    public partial class DataAccess
    {
        private readonly CharityContext _context;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        int _loggedInMemberId = 0;

        public DataAccess(int loggedInMemberId)
        {
            _loggedInMemberId = loggedInMemberId;
        }
        public DataAccess(CharityContext context, IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _config = config;
            _httpContextAccessor = httpContextAccessor;
            var strLoggedInMemberId = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "memberId")?.Value;
            if (strLoggedInMemberId != null)
                _loggedInMemberId = int.Parse(strLoggedInMemberId);
        }
        private string GetBaseUrl()
        {

            var Current = _httpContextAccessor.HttpContext;
            return $"{Current.Request.Scheme}://{Current.Request.Host}{Current.Request.PathBase}";
        }
    }
}
