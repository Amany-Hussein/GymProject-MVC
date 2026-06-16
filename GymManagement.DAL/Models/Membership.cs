using GymProject.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagement.DAL.Models
{
    public class Membership : BaseEntity
    {

        public Member Member { get; set; }
        public Plan Plan { get; set; }
        public int MemberId { get; set; }
        public int PlanId { get; set; }

        //StartDate == CreatedAt of BaseEntity

        public DateTime EndDate { get; set; }

        public string Status => EndDate > DateTime.Now ? "Active" : "Expired"; 

        public bool IsActive => EndDate > DateTime.Now;

    }
}

