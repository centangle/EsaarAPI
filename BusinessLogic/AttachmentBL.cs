using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public partial class Logic
    {
        public async Task<int> CreateAttachment(AttachmentModel model)
        {
            return await _dataAccess.CreateAttachment(model);
        }
        public async Task<bool> DeleteAttachment(string url)
        {
            return await _dataAccess.DeleteAttachment(url);
        }
    }
}
