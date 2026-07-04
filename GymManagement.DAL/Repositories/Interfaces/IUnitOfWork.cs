using GymManagement.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagement.DAL.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        // Get Repository
        IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity , new();

        // SaveChanges
        Task<int> SaveChangesAsync(CancellationToken ct = default);

        //Session Repo
        public ISessionRepository SessionRepository { get; }
        public IMemberShipRepository MembershipRepository { get; }


    }
}

