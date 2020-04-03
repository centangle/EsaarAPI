using Models;
using Models.BriefModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public partial class Logic
    {
        public async Task<bool> AddMember(MemberModel model)
        {
            return await _dataAccess.AddMember(model);
        }
        public async Task<MemberModel> GetMemberByAuthId(string AuthId)
        {
            return await _dataAccess.GetMemberByAuthId(AuthId);
        }
        public async Task<List<MemberBriefModel>> GetMemberForDD(string filter)
        {
            return await _dataAccess.GetMemberForDD(filter);
        }
    }
}
