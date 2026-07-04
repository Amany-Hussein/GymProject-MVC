using GymManagement.BLL.Common;
using GymManagement.BLL.ViewModels.MemberViewModels;
using GymManagement.BLL.ViewModels.PlanViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagement.BLL.Services.Interfaces
{
    public interface IPlanService
    {
        Task<IEnumerable<PlanViewModel>> GetAllPlansAsync(CancellationToken ct = default);
        Task<Result<PlanViewModel>> GetPlanDetailsByIdAsync(int planid, CancellationToken ct = default);
        Task<Result> ActivateButtom(int planid, CancellationToken ct = default);
        Task<Result<UpdatePlanViewModel>> GetPlanToUpdate(int planid, CancellationToken ct = default);
        Task<Result> UpdatePlanAsync(int planid, UpdatePlanViewModel model, CancellationToken ct = default);

    }
}   
