using System;
using System.Collections.Generic;

namespace EntityProvider.DbModels
{
    public partial class RefreshToken
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public DateTime? IssuedTime { get; set; }
        public DateTime? ExpiredTime { get; set; }
        public string ProtectedTicket { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
