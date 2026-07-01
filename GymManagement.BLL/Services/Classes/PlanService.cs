using AutoMapper;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.MemberViewModels;
using GymManagement.BLL.ViewModels.PlanViewModels;
using GymManagement.DAL.Models;
using GymManagement.DAL.Repositories.Interfaces;
using GymProject.Models;
using System;
using System.Collections.Generic;
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
        public async Task<IEnumerable<PlanViewModel>> GetAllAsync(CancellationToken ct = default)
        {
            var Plans = await unitOfWork.GetRepository<Plan>().GetAllAsync(ct : ct);

            if (!Plans.Any())
                return [];

            List<PlanViewModel> result = new List<PlanViewModel>();

            foreach (var plan in Plans)
            {
                //Manual Mapping
                var planViewModel = mapper.Map<PlanViewModel>(plan);


                //  new PlanViewModel()
                //{
                //    Id = plan.Id,
                //    Name = plan.Name,
                //    Description = plan.Description,
                //    DurationDays = plan.DurationDays,
                //    Price = plan.Price,
                //    IsActive = plan.IsActive,
                //};
                result.Add(planViewModel);
            }
            return result;
        }

        public async Task<PlanViewModel?> GetPlanDetailsByIdAsync(int id, CancellationToken ct = default)
        {
            var plan = await unitOfWork.GetRepository<Plan>().GetByIdAsync(id, ct);

            if (plan == null) return null;

            var planViewModel = mapper.Map<PlanViewModel>(plan);


            //new PlanViewModel()
            //{
            //    Id = plan.Id,
            //    Name = plan.Name,
            //    Description = plan.Description,
            //    DurationDays = plan.DurationDays,
            //    Price = plan.Price,
            //    IsActive = plan.IsActive,
            //};
            return planViewModel;
        }

        public async Task<MemberToUpdateViewModel> GetMemberToUpdateAsync(int MemberId, CancellationToken ct = default)
        {
            var Member = await unitOfWork.GetRepository<Member>().GetByIdAsync(MemberId, ct);

            if (Member == null)
                return null;
            else
            {
                return new MemberToUpdateViewModel()
                {
                    Name = Member.Name,
                    Phone = Member.Phone,
                    Photo = Member.Photo,
                    Email = Member.Email,
                    BuildingNumber = Member.Address.BuildingNumber,
                    City = Member.Address.City,
                    Street = Member.Address.Street,
                };
            }
        }
        

        public async Task<UpdatePlanViewModel> GetPlanToUpdateAsync(int PlanId, CancellationToken ct = default)
        {
            var plan = await unitOfWork.GetRepository<Plan>().GetByIdAsync(PlanId, ct);

            if (plan == null)
                return null;
            else
            {
                return new UpdatePlanViewModel()
                {
                    PlanName = plan.Name,
                    Description = plan.Description,
                    DurationDays = plan.DurationDays,
                    Price = plan.Price,
                };
            }
        }

        public Task<bool> UpdatePlanAsync(int id, UpdatePlanViewModel Model, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}
