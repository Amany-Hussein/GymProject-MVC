using GymManagement.BLL.Common;
using GymManagement.BLL.ViewModels.MembershipViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagement.BLL.Services.Interfaces
{
    public interface IMembershipService
    {
        Task<Result<IEnumerable<MembershipViewModel>>> GetAllMemberShipsAsync(CancellationToken ct = default);
        Task<Result> CreateMemberShipAsync(CreateMembershipViewModel model, CancellationToken ct);
        Task<Result<IEnumerable<MemberSelectListViewModel>>> GetMembersForDropDownList(CancellationToken ct = default);
        Task<Result<IEnumerable<PlanSelectListViewModel>>> GetPlansForDropDownList(CancellationToken ct = default);
        Task<Result> DeleteActiveMemberShipp(int memberid, CancellationToken ct = default); // for member who want cancel  membership  

    }
}
