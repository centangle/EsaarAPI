using AuditProvider.DbModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace AuditProvider
{
    public partial class Audit
    {
        private readonly CharityAuditContext _context;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        int _loggedInMemberId;
        public Audit(CharityAuditContext context, IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _config = config;
            _httpContextAccessor = httpContextAccessor;
            var strLoggedInMemberId = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "memberId")?.Value;
            if (strLoggedInMemberId != null)
                _loggedInMemberId = int.Parse(strLoggedInMemberId);
        }
    }
}
