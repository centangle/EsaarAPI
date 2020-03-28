using Models;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public partial class Logic
    {
        public Task<bool> UpdateRefreshToken(RefreshTokenModel model)
        {
            return DA.UpdateRefreshToken(model);
        }
        public Task<RefreshTokenModel> FindRefreshToken(string hashedToken)
        {
            return DA.FindRefreshToken(hashedToken);
        }
        public Task<bool> RemoveRefreshToken(RefreshTokenModel model)
        {
            return DA.RemoveRefreshToken(model);
        }
    }
}
