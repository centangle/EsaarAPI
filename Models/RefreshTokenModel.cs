using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class RefreshTokenModel
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public DateTime IssuedTime { get; set; }
        public DateTime ExpiredTime { get; set; }
        public string ProtectedTicket { get; set; }
    }
}
