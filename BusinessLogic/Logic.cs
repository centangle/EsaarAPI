using DataProvider;

namespace BusinessLogic
{
    public partial class Logic
    {
        DataAccess _dataAccess;
        int _currentPersonId;
        public Logic()
        {
            _dataAccess = new DataAccess();
        }

        public Logic(int currentPersonId) : this()
        {
            _currentPersonId = currentPersonId;

        }


    }
}
