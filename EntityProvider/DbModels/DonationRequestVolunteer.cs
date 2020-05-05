using System;
using System.Collections.Generic;

namespace EntityProvider.DbModels
{
    public partial class DonationRequestVolunteer
    {
        public int Id { get; set; }
        public int DonationRequestId { get; set; }
        public int VolunteerId { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        public virtual DonationRequest DonationRequest { get; set; }
        public virtual Member Volunteer { get; set; }
    }
}
