using AutoMapper;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.SessionViewModels;
using GymManagement.DAL.Models;
using GymManagement.DAL.Models.Enums;
using GymManagement.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace GymManagement.BLL.Services.Classes
{
    public class SessionService : ISessionService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public SessionService(IUnitOfWork unitOfWork , IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<bool> CreateSessionAsync(CreateSessionViewModel model, CancellationToken ct = default)
        {
            if (model.EndDate <= model.StartDate)
                return false;

            if (model.StartDate <= DateTime.Now)
                return false;

            if (model.Capacity < 1 || model.Capacity > 25)
                return false;


            // Get Trainer
            var trainer = await unitOfWork.GetRepository<Trainer>().GetByIdAsync(model.TrainerId);
            if(trainer is null)
                return false;

            //Get Category
            var Category = await unitOfWork.GetRepository<Category>().GetByIdAsync(model.CategoryId);
            if (Category is null)
                return false;

            //Trainer specialty must match Category
            var IsValid = Enum.TryParse<Specialty>(Category.CategoryName, true ,out var CategorySpecialty);

            if (!IsValid || trainer.Specialty != CategorySpecialty)
                return false;
            
            var session = mapper.Map<Session>(model);

            unitOfWork.GetRepository<Session>().AddAsync(session);

            var result = await unitOfWork.SaveChangesAsync();

            return result > 0;
        }
        

        public async Task<IEnumerable<SessionViewModel>> GetAllSessionsAsync(CancellationToken ct = default)
        {
            //Session.Category.Trainer
            var Sessions = await unitOfWork.SessionRepository.GetSessionWithTrainerAndCategory(ct);

            if (Sessions == null || !Sessions.Any())
                return null;


            var MappedSession = Sessions.Select(s => new SessionViewModel()
            {
                Id = s.Id,
                Capacity = s.Capacity,
                CategoryName = s.Category.CategoryName,
                TrainerName = s.Trainer.Name,
                Description = s.Description,
                StartDate = s.StartDate,
                EndDate = s.EndDate,
            });

            foreach (var Session in MappedSession)
            {
                Session.AvailableSlots = Session.Capacity - await unitOfWork.SessionRepository.CountOfBookedSlotsAsync(sessionId: Session.Id , ct);
            }

            return MappedSession;
        }

        public async Task<IEnumerable<CategorySelectViewModel>> GetCategoryrForDropDown(CancellationToken ct = default)
        {
            var result = await unitOfWork.GetRepository<Category>().GetAllAsync(ct: ct);
            return mapper.Map<IEnumerable<CategorySelectViewModel>>(result);
        }


        public async Task<IEnumerable<TrainerSelectViewModel>> GetTrainerForDropDown(CancellationToken ct = default)
        {
            var result = await unitOfWork.GetRepository<Trainer>().GetAllAsync(ct: ct);
            return mapper.Map<IEnumerable<TrainerSelectViewModel>>(result);
        }
    }
    
}
