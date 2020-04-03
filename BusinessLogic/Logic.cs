using DataProvider;

namespace BusinessLogic
{
    public partial class Logic
    {
        DataAccess _dataAccess;
        int _loggedInMemberId;
        public Logic()
        {
            SetDataAccess();
        }

        public Logic(int loggedInMemberId)
        {
            _loggedInMemberId = loggedInMemberId;
            SetDataAccess();

        }
        private void SetDataAccess()
        {
            _dataAccess = new DataAccess(_loggedInMemberId);
        }


    }
}
