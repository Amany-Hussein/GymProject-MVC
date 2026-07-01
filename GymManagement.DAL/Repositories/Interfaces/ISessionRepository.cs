using GymManagement.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagement.DAL.Repositories.Interfaces
{
    public interface ISessionRepository : IGenericRepository<Session>
    {
        //Get All , Get by id , add , update , delete => From IGenericRepository


        Task<IEnumerable<Session>> GetSessionWithTrainerAndCategory(CancellationToken ct = default);

        Task<int> CountOfBookedSlotsAsync(int sessionId , CancellationToken ct = default);
    }
}
