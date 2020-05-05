using System;
using System.Collections.Generic;

namespace EntityProvider.DbModels
{
    public partial class Event
    {
        public Event()
        {
            Campaigns = new HashSet<Campaign>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string NativeName { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        public virtual ICollection<Campaign> Campaigns { get; set; }
    }
}
