using AutoMapper;
using GymManagement.BLL.Common;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.SessionViewModels;
using GymManagement.DAL.Models;
using GymManagement.DAL.Models.Enums;
using GymManagement.DAL.Repositories.Classes;
using GymManagement.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using static System.Collections.Specialized.BitVector32;

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

        public async Task<Result> CreateSessionAsync(CreateSessionViewModel model, CancellationToken ct = default)
        {
            if (model.EndDate <= model.StartDate)
                return Result.Validation("EndDate Must Be Greater Than StartDate");

            if (model.StartDate <= DateTime.Now)
                return Result.Validation("StartDate Must Be In The Future");

            if (model.Capacity < 1 || model.Capacity > 25)
                return Result.Validation("Capacity Must Be Between 1 And 25");


            // Get Trainer
            var trainer = await unitOfWork.GetRepository<Trainer>().GetByIdAsync(model.TrainerId);
            if(trainer is null)
                return Result.NotFound("Trainer Not Found!");

            //Get Category
            var Category = await unitOfWork.GetRepository<Category>().GetByIdAsync(model.CategoryId);
            if (Category is null)
                return Result.NotFound("Category Not Found!");

            //Trainer specialty must match Category
            var IsValid = Enum.TryParse<Specialty>(Category.CategoryName, true ,out var CategorySpecialty);

            if (!IsValid || trainer.Specialty != CategorySpecialty)
                return Result.Validation("Trainer And Category  Must Be The Same speciality");

            var session = mapper.Map<Session>(model);

            unitOfWork.GetRepository<Session>().AddAsync(session);

            var result = await unitOfWork.SaveChangesAsync();

            //return result > 0;
            return result > 0 ? Result.OK() : Result.Fail("Failed To Create Session");

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

        public async Task<Result<SessionViewModel>> GetSessionByIdAsync(int sessionId, CancellationToken ct = default)
        {
            var session = await unitOfWork.SessionRepository.GetSessionByIdWithTrainerAndCategory(sessionId, ct);

            // Check
            if (session is null)
                return Result<SessionViewModel>.NotFound("Session Not Found !");

            else
            {
                var mapped = mapper.Map<SessionViewModel>(session);

                mapped.AvailableSlots = mapped.Capacity - await unitOfWork.SessionRepository.CountOfBookedSlotsAsync(sessionId, ct);

                return Result<SessionViewModel>.OK(mapped);
            }
        }

        

        public async Task<IEnumerable<TrainerSelectViewModel>> GetTrainerForDropDown(CancellationToken ct = default)
        {
            var result = await unitOfWork.GetRepository<Trainer>().GetAllAsync(ct: ct);
            return mapper.Map<IEnumerable<TrainerSelectViewModel>>(result);
        }


        public async Task<Result<UpdateSessionViewModel>> GetSessionToUpdateAsync(int sessionId, CancellationToken ct = default)
        {
            var session = await unitOfWork.SessionRepository.GetByIdAsync(sessionId, ct);

            if(session is null)
                return Result<UpdateSessionViewModel>.NotFound("Session Not Found !");

            if (session.EndDate <= DateTime.Now )
                return Result<UpdateSessionViewModel>.Fail("Cannot Update completed Session");

            if (session.StartDate <= DateTime.Now && session.EndDate > DateTime.Now)
                return Result<UpdateSessionViewModel>.Fail("Cannot Update Ongoing Session");

            // Cannot Update session has already Booking
            var BookingCount = await unitOfWork.SessionRepository.CountOfBookedSlotsAsync(sessionId, ct);
            if (BookingCount > 0)
                return Result<UpdateSessionViewModel>.Fail("Cannot update session has already booking");

            // Map from Session => SessionViewModel
            var mapped = mapper.Map<UpdateSessionViewModel>(session);
            return Result<UpdateSessionViewModel>.OK(mapped);
        }
        public async Task<Result> UpdateSessionAsync(int sessionId, UpdateSessionViewModel model, CancellationToken ct = default)
        {
            var session = await unitOfWork.SessionRepository.GetByIdAsync(sessionId, ct);

            if (session is null) 
                return Result.NotFound("Session Not Found");

            if (session.StartDate <= DateTime.Now) 
                return Result.Validation("Cannot Edit session that already started");

            if(model.StartDate >= model.EndDate) 
                return Result.Validation("EndDate must be after StartDate");


            var BookingCount = await unitOfWork.SessionRepository.CountOfBookedSlotsAsync(sessionId, ct);
            if (BookingCount > 0)
                return Result.Fail("Cannot update session has already booking");

            if (model.StartDate <= DateTime.Now)
                return Result.Validation("Start Date must be in the future");

            var trainer = await unitOfWork.GetRepository<Trainer>().GetByIdAsync(model.TrainerId);//model {changable}
            if (trainer is null) return Result.NotFound("Trainer Not Found");

            var category = await unitOfWork.GetRepository<Category>().GetByIdAsync(session.CategoryId);//session from data base


            var Isvalid = Enum.TryParse<Specialty>(category?.CategoryName, true, out var categotyseciallty);
            if (!Isvalid && trainer.Specialty != categotyseciallty) return Result.Validation("Category And Trainer Must Be The Same Speciallty");

            // Map UpdateSessionViewModel => Session

            mapper.Map(model, session);
            session.UpdatedAt = DateTime.Now;

            unitOfWork.SessionRepository.UpdateAsync(session);
            var result =await unitOfWork.SaveChangesAsync(ct);

            return result > 0 ? Result.OK() : Result.Fail("Failed To Update Session");

        }

        public async Task<Result> DeleteSession(int sessionId, CancellationToken ct = default)
        {
            var session = await unitOfWork.SessionRepository.GetByIdAsync(sessionId);

            if (session is null)
                return Result.NotFound("Session not found.");

            // Rule 1: Cannot delete an ongoing session
            if(session.StartDate <= DateTime.Now && session.EndDate >= DateTime.Now)
                return Result.Validation("Cannot delete an ongoing session.");

            //Delete Session
            unitOfWork.SessionRepository.DeleteAsync(session);

            var result = await unitOfWork.SaveChangesAsync(ct);

            return result > 0 ? Result.OK() : Result.Fail("Failed to delete session.");
        }
    }
    
}
