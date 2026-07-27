using AutoMapper;
using GymManagement.BLL.Common;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.BookingViewModels;
using GymManagement.BLL.ViewModels.MembershipViewModels;
using GymManagement.BLL.ViewModels.SessionViewModels;
using GymManagement.DAL.Models;
using GymManagement.DAL.Repositories.Classes;
using GymManagement.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagement.BLL.Services.Classes
{
    public class BookingService : IBookingService
    {

        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public BookingService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        // Done
        public async Task<Result> CancelBooking(int memberid, int sessionid, CancellationToken ct = default)
        {
            var booking = await unitOfWork.BookingRepository.FirstOrDefaultAsync(X => X.MemberId == memberid && X.SessionId == sessionid);
            if (booking is null) return Result.NotFound("Booking Not Found");
            unitOfWork.BookingRepository.DeleteAsync(booking);
            var result = await unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.OK() : Result.Fail("Failed To Delete Booking");
        }

        // Done
        public async Task<Result> CreateBooking(CreateBookingViewModel model, CancellationToken ct = default)
        {
            var sessionexist = await unitOfWork.SessionRepository.GetByIdAsync(model.SessionId, ct);

            if (sessionexist is null) 
                return Result.Fail("Session Not Exist");

            if (sessionexist.StartDate <= DateTime.Now) 
                return Result.Fail("Can't Book Session Already Started");

            var memberexist = await unitOfWork.GetRepository<Member>().GetByIdAsync(model.MemberId);

            if (memberexist is null) 
                return Result.Fail("Member Not Exist");

            var membership = await unitOfWork.MembershipRepository.AnyAsync(X => X.MemberId == model.MemberId && X.EndDate > DateTime.Now, ct);

            if (!membership) 
                return Result.Fail("Member Must Have Active MemberShip First");

            var memberwithsamesessionberbefore = await unitOfWork.BookingRepository.AnyAsync(X => X.MemberId == model.MemberId && X.SessionId == model.SessionId);

            if (memberwithsamesessionberbefore)
            {
                return Result.Fail("Member Already Booked This Session Before");
            }

            var bookedslots = await unitOfWork.SessionRepository.CountOfBookedSlotsAsync(model.SessionId);

            if (bookedslots >= sessionexist.Capacity) 
                return Result.Fail("No Available Slots , Session Full Capacity");

            var mapped = mapper.Map<Booking>(model);

            unitOfWork.BookingRepository.AddAsync(mapped);

            var result = await unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.OK() : Result.Fail("Failed to Create Booking");

        }

        // Done
        public async Task<Result<IEnumerable<SessionViewModel>>> GetAllSessionsAsync(CancellationToken ct = default)
        {
            var sessions = await unitOfWork.SessionRepository.GetSessionsWithTrainerAndCategory(X => X.EndDate >= DateTime.UtcNow, ct); 

            if (!sessions.Any()) 
                return Result<IEnumerable<SessionViewModel>>.NotFound("No Sessions Available");

            var mapped = mapper.Map<IEnumerable<SessionViewModel>>(sessions);

            foreach (var session in mapped)
            {
                session.AvailableSlots = session.Capacity - await unitOfWork.SessionRepository.CountOfBookedSlotsAsync(session.Id, ct);
            }

            return Result<IEnumerable<SessionViewModel>>.OK(mapped);
        }

        // Done
        public async Task<Result<IEnumerable<MemberForSessionViewModel>>> GetMemberForOngoingSession(int sessionid, CancellationToken ct = default)
        {
            var bookings = await unitOfWork.BookingRepository.GetBySessionId(sessionid, ct);
            var mapped = mapper.Map<IEnumerable<MemberForSessionViewModel>>(bookings);
            return Result<IEnumerable<MemberForSessionViewModel>>.OK(mapped);
        }

        // Done
        public async Task<Result<IEnumerable<MemberForSessionViewModel>>> GetMemberForUpComingSession(int sessionid, CancellationToken ct = default)
        {
            var bookings = await unitOfWork.BookingRepository.GetBySessionId(sessionid, ct);

            var mapped = mapper.Map<IEnumerable<MemberForSessionViewModel>>(bookings);
            return Result<IEnumerable<MemberForSessionViewModel>>.OK(mapped);
        }

        // Done
        public async Task<Result<IEnumerable<MemberSelectListViewModel>>> GetMembersForDropDownList(CancellationToken ct = default)
        {
            var members = await unitOfWork.GetRepository<Member>().GetAllAsync();

            var mapped = mapper.Map<IEnumerable<MemberSelectListViewModel>>(members);
            return Result<IEnumerable<MemberSelectListViewModel>>.OK(mapped);
        }

        // Done
        public async Task<Result> MarkAttened(int memberid, int sessionid, CancellationToken ct = default)
        {
            var booking = await unitOfWork.BookingRepository.FirstOrDefaultAsync(X => X.MemberId == memberid && X.SessionId == sessionid);

            if (booking is null)
                return Result.NotFound("Booking Not Found");

            booking.IsAttented = true;
            booking.UpdatedAt = DateTime.Now;

            unitOfWork.BookingRepository.UpdateAsync(booking);

            var result = await unitOfWork.SaveChangesAsync(ct);

            return result > 0 ? Result.OK() : Result.Fail("Failed To Mark As Attend");
        }
    }
}
