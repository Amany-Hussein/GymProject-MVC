using GymManagement.DAL.Models;
using GymManagement.DAL.Repositories.Interfaces;
using GymProject.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace GymManagement.DAL.Repositories.Classes
{
    public class SessionRepository : GenericRepository<Session>, ISessionRepository
    {
        private readonly GymDbContext dbContext;

        //DataBase Connection
        public SessionRepository(GymDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<int> CountOfBookedSlotsAsync(int sessionId, CancellationToken ct = default)
        {
            return await dbContext.Bookings.AsNoTracking().CountAsync(x => x.SessionId == sessionId);
        }

        public async Task<Session> GetSessionByIdWithTrainerAndCategory(int sessionId, CancellationToken ct = default)
        {
            return await dbContext.Sessions.AsNoTracking().Include(x => x.Trainer).Include(x => x.Category).FirstOrDefaultAsync(s => s.Id == sessionId);
           
        }

        public async Task<IEnumerable<Session>> GetSessionWithTrainerAndCategory(CancellationToken ct = default)
        {
            var query = dbContext.Sessions.AsNoTracking().Include(x => x.Trainer).Include(x => x.Category);
            return await query.ToListAsync();
        }
    }
}
