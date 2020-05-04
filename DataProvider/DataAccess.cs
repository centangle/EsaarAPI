using Microsoft.AspNetCore.Http;

namespace DataProvider
{
    public partial class DataAccess
    {
        private readonly CharityEntities _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        int _loggedInMemberId;

        public DataAccess(int loggedInMemberId)
        {
            _loggedInMemberId = loggedInMemberId;
        }
        public DataAccess(CharityEntities context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
    }
}
