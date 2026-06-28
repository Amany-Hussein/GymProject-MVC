using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.MemberViewModels;
using GymManagement.DAL.Models;
using GymManagement.DAL.Repositories.Interfaces;
using GymProject.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagement.BLL.Services.Classes
{
    public class MemberService : IMemberService
    {
        //DataBase Connection 
        private readonly IGenericRepository<Member> MemberRepo;
        private readonly IGenericRepository<Membership> MembershipRepo;
        private readonly IGenericRepository<Plan> PlanRepo;
        private readonly IGenericRepository<HealthRecord> HealthRecordRepo;
        private readonly IGenericRepository<Booking> BookingRepo;



        public MemberService(IGenericRepository<Member> MemberRepo
            , IGenericRepository<Membership> membershipRepo 
            , IGenericRepository<Plan> planRepo ,
            IGenericRepository<HealthRecord> healthRecordRepo ,
            IGenericRepository<Booking> BookingRepo)
        {
            this.MemberRepo = MemberRepo;
            MembershipRepo = membershipRepo;
            PlanRepo = planRepo;
            HealthRecordRepo = healthRecordRepo;
            this.BookingRepo = BookingRepo;
        }



        public async Task<bool> CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct = default)
        {

            // Email exist or not
            var EmailExist = await MemberRepo.AnyAsync(x => x.Email == model.Email);

            // Phone exist or not
            var PhoneExist = await MemberRepo.AnyAsync(x => x.Phone == model.Phone);

            if (EmailExist || PhoneExist)
                return false;

            var member = new Member()
            {
                Name = model.Name,
                Phone = model.Phone,
                Email = model.Email,
                Gender = model.Gender,
                DateOfBirth = model.DateOfBirth,
                Address = new Address()
                {
                    City = model.City,
                    BuildingNumber = model.BuildingNumber,
                    Street = model.Street,
                },
                HealthRecord = new HealthRecord()
                {
                    Height = model.HealthRecordViewModel.Height,
                    Weight = model.HealthRecordViewModel.Weight,
                    BloodType = model.HealthRecordViewModel.BloodType,
                    Note = model.HealthRecordViewModel.Note,
                },

            };
            var result = await MemberRepo.AddAsync(member);

            return result > 0;
        }

        public async Task<bool> DeleteMemberAsync(int id, CancellationToken ct = default)
        {
            var member = await MemberRepo.GetByIdAsync(id , ct);

            if(member == null) return false;

            // Check if member has active Booking or not
            var HasActiveBooking = await BookingRepo.AnyAsync(x => x.MemberId == member.Id && x.Session.StartDate > DateTime.Now);

            if(HasActiveBooking) return false;

            var result = await MemberRepo.DeleteAsync(member);

            return result > 0;
        }
        

            
        

        public async Task<IEnumerable<MemberViewModel>> GetAllAsync(CancellationToken ct = default)
        {
            //members come from database
            var Members = await MemberRepo.GetAllAsync(ct: ct);

            if (!Members.Any())
                return [];

            List<MemberViewModel> result = new List<MemberViewModel>();

            foreach (var Member in Members)
            {
                // data comes from database I need to send it to the view
                //Manual Mapping
                var memberViewModel = new MemberViewModel()
                {
                    Name = Member.Name,
                    Phone = Member.Phone,
                    Photo = Member.Photo,
                    Email = Member.Email,
                    Id = Member.Id,
                    Gender = Member.Gender.ToString(),
                };
                result.Add(memberViewModel);
            }
            return result;
        }

        public async Task<MemberViewModel?> GetMemberDetailsByIdAsync(int MemberId, CancellationToken ct = default)
        {
            //Get Member By Id
            var member = await MemberRepo.GetByIdAsync(MemberId , ct);

            if(member == null) return null;

            var model = new MemberViewModel()
            {
                Name = member.Name,
                Email = member.Email,
                Phone = member.Phone,
                Gender = member.Gender.ToString(),
                Photo = member.Photo,
                DateOfBirth = member.DateOfBirth.ToString(),
                Address = $"{member.Address.BuildingNumber} - {member.Address.Street} - {member.Address.City}",

            };

            // Check If Member has Acive Membership or not
            var ActiveMembership = await MembershipRepo.FirstOrDefaultAsync(x => x.MemberId == MemberId && x.EndDate > DateTime.Now);

            if(ActiveMembership is not null)
            {
                var ActivePlan = await PlanRepo.GetByIdAsync(ActiveMembership.PlanId, ct);

                model.PlanName = ActivePlan.Name;
                model.MembershipStartDate = ActiveMembership.CreatedAt.ToString();
                model.MembershipEndDate = ActiveMembership.EndDate.ToString();
            }

            return model;
        }

        public async Task<HealthRecordViewModel?> GetMemberHealthRecord(int MemberId, CancellationToken ct = default)
        {
            var record = await HealthRecordRepo.FirstOrDefaultAsync(x => x.MemberId == MemberId, ct: ct);

            if(record is null) return null;

            else
            {
                return new HealthRecordViewModel()
                {
                    Height = record.Height,
                    Weight = record.Weight,
                    BloodType = record.BloodType,
                    Note = record.Note,
                };
            }
        }

        public async Task<MemberToUpdateViewModel> GetMemberToUpdateAsync(int MemberId, CancellationToken ct = default)
        {
            var Member = await MemberRepo.GetByIdAsync(MemberId, ct);

            if (Member == null)
                return null;
            else
            {
                return new MemberToUpdateViewModel()
                {
                    Name = Member.Name,
                    Phone = Member.Phone,
                    Photo = Member.Photo,
                    Email = Member.Email,
                    BuildingNumber = Member.Address.BuildingNumber,
                    City = Member.Address.City,
                    Street = Member.Address.Street,
                };
            }
        }

        public async Task<bool> UpdateMemberAsync(int id, MemberToUpdateViewModel model, CancellationToken ct = default)
        {
            //Get Member
            var member = await MemberRepo.GetByIdAsync(id, ct);

            //Check if any other has the same Email or Phone
            var EmailExists = await MemberRepo.AnyAsync(x => x.Email == model.Email && x.Id != id);
            var PhoneExists = await MemberRepo.AnyAsync(x => x.Phone == model.Email && x.Id != id);

            if(EmailExists || PhoneExists)
            {
                return false;
            }

            member.Phone = model.Phone;
            member.Email = model.Email;
            member.Address.City = model.City;
            member.Address.Street = model.Street;
            member.Address.BuildingNumber = model.BuildingNumber;
            member.UpdatedAt = DateTime.Now;

            var result = await MemberRepo.UpdateAsync(member);

            return result > 0;
        }
    }

}
