using System.Configuration;

namespace DataProvider
{
    public static class Helper
    {
        public static string ConnectionStringValue()
        {
            return ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }
    }
}
