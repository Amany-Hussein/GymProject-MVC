using GymManagement.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace GymManagement.DAL.Repositories.Interfaces
{
    public interface IMemberShipRepository : IGenericRepository<Membership>
    {
        //Get All , Get by id , add , update , delete => From IGenericRepository

        Task<IEnumerable<Membership>> GetMembershipsWithPlanAndMember(Expression<Func<Membership, bool>>? filter = null, CancellationToken ct = default); //filteration on memmberships by funcy

    }
}
