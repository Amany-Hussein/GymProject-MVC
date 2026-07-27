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
    public class MembershipRepository : GenericRepository<Membership> , IMemberShipRepository
    {
        private readonly GymDbContext context;

        public MembershipRepository(GymDbContext dbcontext) : base(dbcontext)
        {
            context = dbcontext;
        }

        public async Task<IEnumerable<Membership>> GetMembershipsWithPlanAndMember(Expression<Func<Membership, bool>>? filter = null, CancellationToken ct = default)
        {
            IQueryable<Membership> query = context.Memberships.AsNoTracking().Include(X => X.Plan).Include(X => X.Member);
            if (filter is not null)
            {
                query = query.Where(filter);
            }

            return await query.ToListAsync(ct);
        }
    }
}
