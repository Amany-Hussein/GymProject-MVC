using GymManagement.BLL.Common;
using GymManagement.BLL.ViewModels.TrainerViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagement.BLL.Services.Interfaces
{
    public interface ITrainerService
    {
        // Get All
        Task<Result<IEnumerable<TrainerViewModel>>> GetAllTrainersAsync(CancellationToken ct = default);

        // Create new trainer
        Task<Result> CreateTrainerAsync(CreateTrainerViewModel model, CancellationToken ct = default);

        // Get Trainer details
        Task<Result<TrainerViewModel>> GetTrainerDetailsByIdAsync(int tranerId, CancellationToken ct = default);

        // Get trainer data to update
        Task<Result<UpdateTrainerViewModel>> GetTrainerToUpdate(int trainerid, CancellationToken ct = default);

        // Update trainer
        Task<Result> UpdateTrainerAsync(int trainerid, UpdateTrainerViewModel model, CancellationToken ct = default);

        // Delete Trainer
        Task<Result> DeleteTrainer(int trainerid);
    }
}
