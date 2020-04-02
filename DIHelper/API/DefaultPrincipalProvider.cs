using DIHelper.API;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;

namespace DIHelper.API
{
    public class DefaultPrincipalProvider : IPrincipalProvider
    {
        public IPrincipal User
        {
            get
            {
                return HttpContext.Current.User;
            }
        }
        public int LoggedInPersonId
        {
            get
            {
                //int y=int.Parse(((ClaimsIdentity)HttpContext.Current.User.Identity).Claims.Where(c => c.Type == ClaimTypes.Sid).FirstOrDefault().Value);
                //var x = HttpContext.Current.User;
                return 0;
            }
        }
    }
}