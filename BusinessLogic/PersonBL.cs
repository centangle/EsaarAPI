using Models;
using Models.BriefModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public partial class Logic
    {
        public async Task<bool> AddPerson(PersonModel model)
        {
            return await _dataAccess.AddPerson(model);
        }
        public async Task<PersonModel> GetPersonByAuthId(string AuthId)
        {
            return await _dataAccess.GetPersonByAuthId(AuthId);
        }
        public async Task<List<PersonBriefModel>> GetPersonForDD(string filter)
        {
            return await _dataAccess.GetPersonForDD(filter);
        }
    }
}
