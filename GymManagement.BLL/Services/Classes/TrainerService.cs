using AutoMapper;
using GymManagement.BLL.Common;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.TrainerViewModels;
using GymManagement.DAL.Models;
using GymManagement.DAL.Repositories.Classes;
using GymManagement.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagement.BLL.Services.Classes
{
    public class TrainerService : ITrainerService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public TrainerService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        // Done
        public async Task<Result> CreateTrainerAsync(CreateTrainerViewModel model, CancellationToken ct = default)
        {
            // Email must be unique
            var emailexist = await unitOfWork.GetRepository<Trainer>().AnyAsync(X => X.Email == model.Email);
            if (emailexist) return Result.Validation("Email Already Exist");

            // Phone must be unique
            var phoneexist = await unitOfWork.GetRepository<Trainer>().AnyAsync(X => X.Phone == model.Phone);
            if (phoneexist) return Result.Validation("Phone Number Already Exist");

            var mapped = mapper.Map<Trainer>(model);


            unitOfWork.GetRepository<Trainer>().AddAsync(mapped);

            var result = await unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.OK() : Result.Fail("Failed To Add Trainer");


        }

        // Done
        public async Task<Result> DeleteTrainer(int trainerid)
        {
            //Cannot delete a trainer with scheduled sessions
            var sessioncheck = await unitOfWork.SessionRepository.AnyAsync(s => s.TrainerId == trainerid && s.EndDate > DateTime.Now);
            if (sessioncheck) return Result.Fail("Can't Delete Trainers With Active Session");

            var trainr = await unitOfWork.GetRepository<Trainer>().GetByIdAsync(trainerid);
            if (trainr is null) return Result.NotFound("Trainer Not Found");

            unitOfWork.GetRepository<Trainer>().DeleteAsync(trainr);

            var result = await unitOfWork.SaveChangesAsync();

            return result > 0 ? Result.OK() : Result.Fail("Failed To Delete Trainer");
        }

        // Done
        public async Task<Result<IEnumerable<TrainerViewModel>>> GetAllTrainersAsync(CancellationToken ct = default)
        {
            var Trainers = await unitOfWork.GetRepository<Trainer>().GetAllAsync(ct: ct);
            if (!Trainers.Any()) return Result<IEnumerable<TrainerViewModel>>.NotFound("Trainers Not Found");

            var mapped = mapper.Map<IEnumerable<TrainerViewModel>>(Trainers);

            return Result<IEnumerable<TrainerViewModel>>.OK(mapped);

        }

        // Done
        public async Task<Result<TrainerViewModel>> GetTrainerDetailsByIdAsync(int tranerId, CancellationToken ct = default)
        {
            var trainer = await unitOfWork.GetRepository<Trainer>().GetByIdAsync(tranerId, ct);

            if (trainer is null) return Result<TrainerViewModel>.NotFound("Trainer Not Found");

            var mapped = mapper.Map<TrainerViewModel>(trainer);

            return Result<TrainerViewModel>.OK(mapped);
        }

        // Done
        public async Task<Result<UpdateTrainerViewModel>> GetTrainerToUpdate(int trainerid, CancellationToken ct = default)
        {
            var trainer = await unitOfWork.GetRepository<Trainer>().GetByIdAsync(trainerid);

            if (trainer is null) return Result<UpdateTrainerViewModel>.NotFound("Trainer Not Found");

            var mapped = mapper.Map<UpdateTrainerViewModel>(trainer);

            return Result<UpdateTrainerViewModel>.OK(mapped);
        }

        public async Task<Result> UpdateTrainerAsync(int trainerid, UpdateTrainerViewModel model, CancellationToken ct = default)
        {
            var trainer = await unitOfWork.GetRepository<Trainer>().GetByIdAsync(trainerid);

            if (trainer is null) return Result.NotFound("Trainer Not Found");

            //Email must remain unique 
            var emailexist = await unitOfWork.GetRepository<Trainer>().AnyAsync(X => X.Email == model.Email && X.Id != trainerid);
            if (emailexist) return Result.Validation("Email Already Exist");

            //Phone must remain unique
            var phoneexist = await unitOfWork.GetRepository<Trainer>().AnyAsync(X => X.Phone == model.Phone && X.Id != trainerid);
            if (phoneexist) return Result.Validation("Phone Number Already Exist");
            
            // Mapping
            mapper.Map(model, trainer);

            unitOfWork.GetRepository<Trainer>().UpdateAsync(trainer);

            var result = await unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.OK() : Result.Fail("Failed To Update Trainer");
        }
    }
}
