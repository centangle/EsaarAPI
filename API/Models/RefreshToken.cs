using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public class RefreshToken
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
    }
}
