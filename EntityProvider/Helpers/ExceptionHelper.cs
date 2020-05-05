using System;
using System.Data;

namespace EntityProvider.Helpers
{
    public static class ExceptionHelper
    {
        public static string GetDataExceptionMessage(DataException ex)
        {
            try
            {
                return ex.InnerException.InnerException.Message.Split(new string[] { "statement conflicted with the" }, StringSplitOptions.None)[1].Split('.')[0].ToString();
            }
            catch
            {
                return "Foreign Key Constraint";
            }
        }
    }
}
