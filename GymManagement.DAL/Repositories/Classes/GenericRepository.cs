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
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity, new()
    {

        //DataBase Connection 
        private readonly GymDbContext dbContext;
        private readonly DbSet<TEntity> set;

        public GenericRepository(GymDbContext dbContext)
        {
            this.dbContext = dbContext;
            set = dbContext.Set<TEntity>();
        }
        public async Task<int> AddAsync(TEntity entity)
        {
            set.Add(entity);
            return await dbContext.SaveChangesAsync();
        }

        public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default)
        {
            return set.AsNoTracking().AnyAsync(predicate, ct);
        }

        public async Task<int> DeleteAsync(TEntity entity)
        {
            set.Remove(entity);
            return await dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(bool tracking = false, CancellationToken ct = default)
        {
            IQueryable<TEntity> query = tracking ? set : set.AsNoTracking();
            return await query.ToListAsync();
        }

        public async Task<TEntity> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await set.FindAsync(id , ct);
        }

        public async Task<int> UpdateAsync(TEntity entity) 
        {
            set.Update(entity);
            return await dbContext.SaveChangesAsync();
        }
    }
}
