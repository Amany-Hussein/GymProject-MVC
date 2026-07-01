using GymManagement.BLL.ViewModels.MemberViewModels;
using GymManagement.BLL.ViewModels.PlanViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagement.BLL.Services.Interfaces
{
    public interface IPlanService
    {
        // Get All
        Task<IEnumerable<PlanViewModel>> GetAllAsync(CancellationToken ct = default);

        //Get Specefic Plan By Id 
        Task<PlanViewModel?> GetPlanDetailsByIdAsync( int id , CancellationToken ct = default);

        Task<UpdatePlanViewModel> GetPlanToUpdateAsync(int PlanId, CancellationToken ct = default);


        // Update Plan By Id
        Task<bool> UpdatePlanAsync(int id, UpdatePlanViewModel Model, CancellationToken ct = default);



    }
}   
