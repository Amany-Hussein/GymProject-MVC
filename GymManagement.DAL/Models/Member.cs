using GymProject.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagement.DAL.Models
{
    public class Member : GymUser
    {
        public string? Photo { get; set; }

        //JoinDate == CreatedAt of BaseEntity

        #region Relationships

        public HealthRecord HealthRecord { get; set; } = default;


        #region Relationships

        public ICollection<Membership> Plans { get; set; }

        public ICollection<Booking> Sessions { get; set; }


        #endregion


        #endregion

    }
}
