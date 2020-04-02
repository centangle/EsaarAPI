using Models;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public partial class Logic
    {
        public Task<bool> UpdateRefreshToken(RefreshTokenModel model)
        {
            return _dataAccess.UpdateRefreshToken(model);
        }
        public Task<RefreshTokenModel> FindRefreshToken(string hashedToken)
        {
            return _dataAccess.FindRefreshToken(hashedToken);
        }
        public Task<bool> RemoveRefreshToken(RefreshTokenModel model)
        {
            return _dataAccess.RemoveRefreshToken(model);
        }
    }
}
