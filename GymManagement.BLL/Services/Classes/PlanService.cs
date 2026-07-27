using AutoMapper;
using GymManagement.BLL.Common;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.MemberViewModels;
using GymManagement.BLL.ViewModels.PlanViewModels;
using GymManagement.DAL.Models;
using GymManagement.DAL.Repositories.Classes;
using GymManagement.DAL.Repositories.Interfaces;
using GymProject.Models;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GymManagement.BLL.Services.Classes
{
    public class PlanService : IPlanService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public PlanService( IUnitOfWork unitOfWork , IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        // Done
        public async Task<Result> ActivateButtom(int planid, CancellationToken ct = default)
        {
            var plan = await unitOfWork.GetRepository<Plan>().GetByIdAsync(planid, ct);
            plan.IsActive = !plan.IsActive;
            unitOfWork.GetRepository<Plan>().UpdateAsync(plan);
            var result = await unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.OK() : Result.Fail("Can NO Deal WIth Plan Now");
        }

        // Done
        public async Task<IEnumerable<PlanViewModel>> GetAllPlansAsync(CancellationToken ct = default)
        {
            var plans = await unitOfWork.GetRepository<Plan>().GetAllAsync(ct: ct);
            if (!plans.Any()) return [];
            var result = mapper.Map<IEnumerable<PlanViewModel>>(plans);
            return result;
        }

        // Done
        public async Task<Result<PlanViewModel>> GetPlanDetailsByIdAsync(int planid, CancellationToken ct = default)
        {
            var plan = await unitOfWork.GetRepository<Plan>().GetByIdAsync(planid, ct);
            if (plan is null) return Result<PlanViewModel>.NotFound("Not Found Plan");
            var result = mapper.Map<PlanViewModel>(plan);
            return Result<PlanViewModel>.OK(result);
        }

        // Done
        public async Task<Result<UpdatePlanViewModel>> GetPlanToUpdate(int planid, CancellationToken ct = default)
        {
            var plan = await unitOfWork.GetRepository<Plan>().GetByIdAsync(planid, ct);

            if (plan is null) return Result<UpdatePlanViewModel>.NotFound("Plan Not Found!");

            var activememberships = await unitOfWork.GetRepository<Membership>().AnyAsync(X => X.PlanId == planid);

            if (activememberships) return Result<UpdatePlanViewModel>.Fail("Can Not Update Plan With Active Memberships ");

            var result = mapper.Map<UpdatePlanViewModel>(plan);
            return Result<UpdatePlanViewModel>.OK(result);
        }


        // Done
        public async Task<Result> UpdatePlanAsync(int planid, UpdatePlanViewModel model, CancellationToken ct = default)
        {
            var plan = await unitOfWork.GetRepository<Plan>().GetByIdAsync(planid, ct);
            if (plan is null) return Result.NotFound("Plan Not Found");
            if (model.Name != plan.Name) return Result.Fail("Not Allowed To Change Plan Name");



            mapper.Map<UpdatePlanViewModel, Plan>(model, plan);
            plan.UpdatedAt = DateTime.Now;
            unitOfWork.GetRepository<Plan>().UpdateAsync(plan);
            var result = await unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.OK() : Result.Fail("Can Not Update Plan");
        }
    }
}
