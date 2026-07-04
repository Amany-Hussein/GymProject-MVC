using AutoMapper;
using GymManagement.BLL.Common;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.AnalyticsViewModels;
using GymManagement.DAL.Models;
using GymManagement.DAL.Repositories.Classes;
using GymManagement.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagement.BLL.Services.Classes
{
    public class AnalyticsService : IAnalyticsService
    {

        private readonly IUnitOfWork unitOfWork;

        public AnalyticsService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public async Task<Result<AnalyticsViewModel>> GetDataAsync(CancellationToken ct = default)
        {
            var now = DateTime.Now;

            var AllMembers = await unitOfWork.GetRepository<Member>().CountAsync(ct: ct);

            var ActiveMembers = await unitOfWork.GetRepository<Membership>().CountAsync(X => X.EndDate > now, ct);

            var AllTrainers = await unitOfWork.GetRepository<Trainer>().CountAsync(ct: ct);

            var UpcomingSessions = await unitOfWork.GetRepository<Session>().CountAsync(X => X.StartDate > now, ct);

            var OngoingSessions = await unitOfWork.GetRepository<Session>().CountAsync(X => X.StartDate <= now && X.EndDate >= now);

            var CompletedSessions = await   unitOfWork.GetRepository<Session>().CountAsync(X => X.EndDate < now);

            var mapped = new AnalyticsViewModel()
            {
                ActiveMembers = ActiveMembers,
                TotalMembers = AllMembers,
                TotalTrainers = AllTrainers,
                UpcomingSessions = UpcomingSessions,
                OngoingSessions = OngoingSessions,
                CompletedSessions = CompletedSessions
            };

            return Result<AnalyticsViewModel>.OK(mapped);

        }
    }
}
