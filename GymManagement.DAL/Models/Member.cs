using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagement.DAL.Models
{
    public class Member : GymUser
    {
        public string? Photo { get; set; }

        //JoinDate == CreatedAt of BaseEntity

    }
}
