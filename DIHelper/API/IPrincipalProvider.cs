using System.Security.Principal;

namespace DIHelper.API
{
    public interface IPrincipalProvider
    {
        IPrincipal User { get; }
        int LoggedInPersonId { get; }
    }
    
}
