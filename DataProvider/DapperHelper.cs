using System.Configuration;

namespace DataProvider
{
    public static class DapperHelper
    {
        public static string ConnectionStringValue()
        {
            return ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }
    }
}
