using GymManagement.BLL.Common;
using GymManagement.BLL.ViewModels.MemberViewModels;
using GymManagement.BLL.ViewModels.SessionViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagement.BLL.Services.Interfaces
{
    public interface ISessionService
    {
        // Get All Sessions
        Task<IEnumerable<SessionViewModel>> GetAllSessionsAsync(CancellationToken ct = default);


        // Create Session
        Task<Result> CreateSessionAsync(CreateSessionViewModel model, CancellationToken ct = default);

        // get trainers for viewbag  
        Task<IEnumerable<TrainerSelectViewModel>> GetTrainerForDropDown(CancellationToken ct = default);

        // get Categories for viewbag  
        Task<IEnumerable<CategorySelectViewModel>> GetCategoryrForDropDown(CancellationToken ct = default);
    }
}

