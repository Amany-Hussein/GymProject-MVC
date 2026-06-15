using GymManagement.DAL.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagement.DAL.Models
{
    public class GymUser : BaseEntity
    {
        public string  Name { get; set; }
        public string Email { get; set; }

        public string Phone { get; set; }

        public DateOnly DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public Address Address { get; set; }

    }

    [Owned]
    public class Address
    {
        public string BuildingNumber { get; set; } = default!;
        public string Street { get; set; } = default!;
        public string City { get; set; } = default!;


    }
}
