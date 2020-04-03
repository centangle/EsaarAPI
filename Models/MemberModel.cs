using Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class MemberModel : BaseModel
    {
        public MemberModel()
        {
            Address = new AddressModel();
        }
        public string AuthUserId { get; set; }
        public string Name { get; set; }
        public string NativeName { get; set; }
        public string IdentificationNo { get; set; }
        public int Type { get; set; }
        public AddressModel Address { get; set; }
    }
}
