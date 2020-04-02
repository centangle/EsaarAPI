namespace DataProvider
{
    public partial class DataAccess
    {
        #region[Private Properties]
        int _currentPersonId;
        #endregion
        public DataAccess()
        {
        }
        public DataAccess(int currentPersonId)
        {
            _currentPersonId = currentPersonId;
        }
    }
}
