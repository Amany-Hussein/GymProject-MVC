using GymManagement.DAL.Models;
using GymProject.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace GymManagement.DAL.Repositories.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : BaseEntity , new()
    {
        //Get All 
        Task<IEnumerable<TEntity>> GetAllAsync(bool tracking = false, CancellationToken ct = default);

        //Get By Id
        Task<TEntity> GetByIdAsync(int id, CancellationToken ct = default);

        //Add
        void AddAsync(TEntity entity);

        //Update 
        void UpdateAsync(TEntity entity);

        //Delete
        void DeleteAsync(TEntity entity);

        Task<bool> AnyAsync(Expression<Func<TEntity , bool>> predicate , CancellationToken ct = default);

        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, bool tracking = false , CancellationToken ct = default);
    }
}
