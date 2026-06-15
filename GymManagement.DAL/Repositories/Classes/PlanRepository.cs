using GymManagement.DAL.Repositories.Interfaces;
using GymProject.Context;
using GymProject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagement.DAL.Repositories.Classes
{
    public class PlanRepository : IPlanRepository
    {

        //database connection
        private readonly GymDbContext dbContext;


        public PlanRepository(GymDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<int> AddAsync(Plan plan, CancellationToken ct = default)
        {
            dbContext.Plans.Add(plan);
            return await dbContext.SaveChangesAsync(ct);
        }

        public async Task<int> DeleteAsync(Plan plan, CancellationToken ct = default)
        {
            dbContext.Plans.Remove(plan);
            return await dbContext.SaveChangesAsync(ct);
        }

        public async Task<IEnumerable<Plan>> GetAllAsync(bool tracking = false, CancellationToken ct = default)
        {
            //if (tracking)
            //    return await dbContext.Plans.ToListAsync(ct);
            //else
            //    return await dbContext.Plans.AsNoTracking().ToListAsync(ct);

            IQueryable<Plan> query = tracking ? dbContext.Plans : dbContext.Plans.AsNoTracking();
            return await query.ToListAsync(ct);

        }

        public async Task<Plan> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await dbContext.Plans.FindAsync(id, ct);
        }

        public async Task<int> UpdateAsync(Plan plan, CancellationToken ct = default)
        {
            dbContext.Plans.Update(plan);
            return await dbContext.SaveChangesAsync(ct);
        }
    }
}
