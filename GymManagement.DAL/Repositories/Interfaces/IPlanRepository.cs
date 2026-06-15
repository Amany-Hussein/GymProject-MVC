using GymProject.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagement.DAL.Repositories.Interfaces
{
    public interface IPlanRepository
    {
        //Get All Plans 
        Task<IEnumerable<Plan>> GetAllAsync(bool tracking = false , CancellationToken ct = default);

        //Get Plan By Id
        Task<Plan> GetByIdAsync(int id , CancellationToken ct = default);

        //Add
        Task<int> AddAsync(Plan plan , CancellationToken ct = default);

        //Update 
        Task<int> UpdateAsync(Plan plan, CancellationToken ct = default);

        //Delete
        Task<int>DeleteAsync(Plan plan, CancellationToken ct = default);


    }
}
