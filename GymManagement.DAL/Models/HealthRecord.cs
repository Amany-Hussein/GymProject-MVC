using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagement.DAL.Models
{
    public class HealthRecord : BaseEntity
    {
        public decimal Height { get; set; }
        public decimal Weight { get; set; }

        public decimal BloodType { get; set; }

        public string? Note { get; set; }

        //UpdatedAt of BaseEntity => LastUpdated 
    }
}
