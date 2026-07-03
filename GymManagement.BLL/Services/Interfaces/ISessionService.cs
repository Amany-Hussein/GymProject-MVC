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

        // Get Session Details
        Task<Result<SessionViewModel>> GetSessionByIdAsync(int sessionId, CancellationToken ct = default);

        // Get Session to Update
        Task<Result<UpdateSessionViewModel>> GetSessionToUpdateAsync(int sessionId, CancellationToken ct = default);

        //Update Session
        Task<Result> UpdateSessionAsync(int sessionId, UpdateSessionViewModel model ,  CancellationToken ct = default);
    }
}

