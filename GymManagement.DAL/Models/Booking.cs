using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagement.DAL.Models
{
    public class Booking : BaseEntity
    {
        public Member Member { get; set; }
        public int MemberId { get; set; }
        public Session Session { get; set; }
        public int SessionId { get; set; }

        // BookingDate == CreatedAt

        public bool IsAttented { get; set; }
    }
}


