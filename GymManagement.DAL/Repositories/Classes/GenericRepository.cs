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
        public async void AddAsync(TEntity entity)
        {
            set.Add(entity);
        }

        public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default)
        {
            return set.AsNoTracking().AnyAsync(predicate, ct);
        }

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>>? expression = null, CancellationToken ct = default)
        {
            if (expression is null)
            {
                return await dbContext.Set<TEntity>().AsNoTracking().CountAsync(ct);
            }
            return await dbContext.Set<TEntity>().AsNoTracking().CountAsync(expression, ct);
        }


        public async void DeleteAsync(TEntity entity)
        {
            set.Remove(entity);
        }

        public async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, bool tracking = false, CancellationToken ct = default)
        {
            IQueryable<TEntity> query = tracking ? set : set.AsNoTracking();
            return await query.FirstOrDefaultAsync(predicate);
        
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

        public async void UpdateAsync(TEntity entity) 
        {
            set.Update(entity);
        }
    }
}
