namespace DataProvider
{
    public partial class DataAccess
    {
        int _loggedInMemberId;
        public DataAccess(int loggedInMemberId)
        {
            _loggedInMemberId = loggedInMemberId;
        }
    }
}
