using Microsoft.Extensions.Configuration;
using System.Configuration;

namespace EntityProvider
{
    public partial class DataAccess
    {
        private string DapperConnectionString()
        {
            return _config.GetConnectionString("CharityConnection");
        }
    }
}
