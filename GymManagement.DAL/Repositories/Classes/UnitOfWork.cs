using GymManagement.DAL.Models;
using GymManagement.DAL.Repositories.Interfaces;
using GymProject.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagement.DAL.Repositories.Classes
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly GymDbContext dbContext;
        private readonly  Dictionary<string , object> repositories = [];


        //DataBase Connection

        public UnitOfWork(GymDbContext dbContext, ISessionRepository sessionRepository)
        {
            this.dbContext = dbContext;
            SessionRepository = sessionRepository;
        }

        public ISessionRepository SessionRepository { get; }

        public IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity, new()
        {
            // Check if Repo Exists or not

            var typeName = typeof(TEntity).Name;

            // If exists in Dictionary => use it
            if (repositories.TryGetValue(typeName, out object? value))
                return (IGenericRepository<TEntity>)value;

            // If not => create repo then add it to the Dictionary then use it
            else
            {
                var repo = new GenericRepository<TEntity>(dbContext);
                repositories[typeName] = repo;
                return repo;
            }

        }

        public Task<int> SaveChangesAsync(CancellationToken ct)
        {
            return dbContext.SaveChangesAsync(ct);
        }
    }
}
