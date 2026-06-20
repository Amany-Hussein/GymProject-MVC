using GymManagement.BLL.ViewModels.MemberViewModels;
using GymManagement.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagement.BLL.Services.Interfaces
{
    public interface IMemberService
    {
        // Get All

        Task<IEnumerable<MemberViewModel>> GetAllAsync(CancellationToken ct = default);

        // Create Member
        Task<bool> CreateMemberAsync(CreateMemberViewModel member , CancellationToken ct = default);
    }
}
