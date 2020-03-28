using DataProvider;

namespace BusinessLogic
{
    public partial class Logic
    {
        private DataAccess DA;
        #region[Private Properties]
        private string CurrentUserId { get; set; }
        #endregion

        public Logic()
        {
            DA = new DataAccess();
        }

        public Logic(string UserId)
        {
            CurrentUserId = UserId;
            DA = new DataAccess(CurrentUserId);
        }
    }
}
