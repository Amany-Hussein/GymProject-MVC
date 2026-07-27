using AutoMapper;
using GymManagement.BLL.Common;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.MembershipViewModels;
using GymManagement.DAL.Models;
using GymManagement.DAL.Repositories.Classes;
using GymManagement.DAL.Repositories.Interfaces;
using GymProject.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagement.BLL.Services.Classes
{
    public class MembershipService : IMembershipService
    {

        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public MembershipService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        // Done
        public async Task<Result> CreateMemberShipAsync(CreateMembershipViewModel model, CancellationToken ct)
        {
            var memberexist = await unitOfWork.GetRepository<Member>().AnyAsync(X => X.Id == model.MemberId, ct);

            if (!memberexist) 
                return Result.NotFound("Member Must Be Exist");

            var planexist = await unitOfWork.GetRepository<Plan>().AnyAsync(X => X.Id == model.PlanId);

            if (!planexist) 
                return Result.NotFound("Plan Must Be Exist");

            var hasactivemembership = await unitOfWork.MembershipRepository.AnyAsync(X => X.MemberId == model.MemberId && X.EndDate > DateTime.Now, ct); // X.isactive?

            if (hasactivemembership) 
                return Result.Fail("Member Already Have One Active Membership");

            var plan = await unitOfWork.GetRepository<Plan>().GetByIdAsync(model.PlanId, ct);

            if (!plan.IsActive) 
                return Result.Fail("Plan Is Not Active Right Now");
            var membership = mapper.Map<Membership>(model);

            membership.EndDate = (model.StartDate ?? DateTime.Now).AddDays(plan.DurationDays);

            unitOfWork.MembershipRepository.AddAsync(membership);

            var result = await unitOfWork.SaveChangesAsync(ct);

            return result > 0 ? Result.OK() : Result.Fail("Failed To Create MemberShip");


        }

        // Done
        public async Task<Result> DeleteActiveMemberShipp(int memberid, CancellationToken ct = default)
        {
            var activemembership = await unitOfWork.MembershipRepository.FirstOrDefaultAsync(X => X.MemberId == memberid && X.EndDate > DateTime.Now, true);

            if (activemembership == null) 
                return Result.NotFound("No Active Membership Found For This Member");

            unitOfWork.MembershipRepository.DeleteAsync(activemembership);

            var result = await unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.OK() : Result.Fail("Failed To Delete MemberShip");

        }

        // Done
        public async Task<Result<IEnumerable<MembershipViewModel>>> GetAllMemberShipsAsync(CancellationToken ct = default)
        {
            var memberships = await unitOfWork.MembershipRepository.GetMembershipsWithPlanAndMember(x => x.EndDate > DateTime.Now, ct); 

            var mapped = mapper.Map<IEnumerable<MembershipViewModel>>(memberships);
            return Result<IEnumerable<MembershipViewModel>>.OK(mapped);

        }

        // Done
        public async Task<Result<IEnumerable<MemberSelectListViewModel>>> GetMembersForDropDownList(CancellationToken ct = default)
        {
            var members = await unitOfWork.GetRepository<Member>().GetAllAsync(ct: ct);
            var mapped = mapper.Map<IEnumerable<MemberSelectListViewModel>>(members);
            return Result<IEnumerable<MemberSelectListViewModel>>.OK(mapped);
        }

        // Done
        public async Task<Result<IEnumerable<PlanSelectListViewModel>>> GetPlansForDropDownList(CancellationToken ct = default)
        {
            var plans = await unitOfWork.GetRepository<Plan>().GetAllAsync(ct: ct);
            var mapped = mapper.Map<IEnumerable<PlanSelectListViewModel>>(plans);
            return Result<IEnumerable<PlanSelectListViewModel>>.OK(mapped);
        }
    }
}
