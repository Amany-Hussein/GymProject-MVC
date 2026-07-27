using GymManagement.DAL.Models;
using GymManagement.DAL.Repositories.Interfaces;
using GymProject.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagement.DAL.Repositories.Classes
{
    public class BookingRepository : GenericRepository<Booking> , IBookingRepository
    {
        private readonly GymDbContext dbContext;

        public BookingRepository(GymDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

        public Task<List<Booking>> GetBySessionId(int sessionid, CancellationToken ct = default)
        {
            return dbContext.Bookings.AsNoTracking().Include(X => X.Member)
                                                     .Where(X => X.SessionId == sessionid)
                                                     .ToListAsync(ct);
        }
    }
}
