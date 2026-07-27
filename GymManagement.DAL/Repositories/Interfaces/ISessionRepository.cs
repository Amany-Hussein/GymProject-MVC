using GymManagement.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace GymManagement.DAL.Repositories.Interfaces
{
    public interface ISessionRepository : IGenericRepository<Session>
    {
        //Get All , Get by id , add , update , delete => From IGenericRepository


        Task<IEnumerable<Session>> GetSessionWithTrainerAndCategory(CancellationToken ct = default);

        Task<int> CountOfBookedSlotsAsync(int sessionId , CancellationToken ct = default);

        Task<Session> GetSessionByIdWithTrainerAndCategory(int sessionId , CancellationToken ct = default);

        public Task<IEnumerable<Session>> GetSessionsWithTrainerAndCategory(Expression<Func<Session, bool>>? expression = null, CancellationToken ct = default);
        
    }
}
