using DataProvider;

namespace BusinessLogic
{
    public partial class Logic
    {
        DataAccess _dataAccess;
        int _currentPersonId;
        public Logic()
        {
            SetDataAccess();
        }

        public Logic(int currentPersonId)
        {
            _currentPersonId = currentPersonId;
            SetDataAccess();

        }
        private void SetDataAccess()
        {
            _dataAccess = new DataAccess(_currentPersonId);
        }


    }
}
