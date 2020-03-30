using Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Base
{
    public abstract class BaseModel: IBase
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
    }
}
