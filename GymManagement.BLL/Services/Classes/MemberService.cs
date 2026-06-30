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
        private readonly IUnitOfWork unitOfWork;

        // object of UintOfWork
        public MemberService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<bool> CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct = default)
        {

            // Email exist or not
            var EmailExist = await unitOfWork.GetRepository<Member>().AnyAsync(x => x.Email == model.Email);

            // Phone exist or not
            var PhoneExist = await unitOfWork.GetRepository<Member>().AnyAsync(x => x.Phone == model.Phone);

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
            unitOfWork.GetRepository<Member>().AddAsync(member);

            var result = await unitOfWork.SaveChangesAsync(ct);

            return result > 0;
        }

        public async Task<bool> DeleteMemberAsync(int id, CancellationToken ct = default)
        {
            var member = await unitOfWork.GetRepository<Member>().GetByIdAsync(id , ct);

            if(member is  null) return false;

            // Check if member has active Booking or not
            var HasActiveBooking = await unitOfWork.GetRepository<Booking>().AnyAsync(x => x.MemberId == member.Id && x.Session.StartDate > DateTime.Now);

            if(HasActiveBooking) return false;

            unitOfWork.GetRepository<Member>().DeleteAsync(member);

            var result = await  unitOfWork.SaveChangesAsync(ct);

            return result > 0;
        }

        public async Task<IEnumerable<MemberViewModel>> GetAllAsync(CancellationToken ct = default)
        {
            //members come from database
            var Members = await unitOfWork.GetRepository<Member>().GetAllAsync(ct: ct);

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
            var member = await unitOfWork.GetRepository<Member>().GetByIdAsync(MemberId , ct);

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
            var ActiveMembership = await unitOfWork.GetRepository<Membership>().FirstOrDefaultAsync(x => x.MemberId == MemberId && x.EndDate > DateTime.Now);

            if(ActiveMembership is not null)
            {
                var ActivePlan = await unitOfWork.GetRepository<Plan>().GetByIdAsync(ActiveMembership.PlanId, ct);

                model.PlanName = ActivePlan.Name;
                model.MembershipStartDate = ActiveMembership.CreatedAt.ToString();
                model.MembershipEndDate = ActiveMembership.EndDate.ToString();
            }

            return model;
        }

        public async Task<HealthRecordViewModel?> GetMemberHealthRecord(int MemberId, CancellationToken ct = default)
        {
            var record = await unitOfWork.GetRepository<HealthRecord>().FirstOrDefaultAsync(x => x.MemberId == MemberId, ct: ct);

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
            var Member = await unitOfWork.GetRepository<Member>().GetByIdAsync(MemberId, ct);

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
            var member = await unitOfWork.GetRepository<Member>().GetByIdAsync(id, ct);

            //Check if any other has the same Email or Phone
            var EmailExists = await unitOfWork.GetRepository<Member>().AnyAsync(x => x.Email == model.Email && x.Id != id);
            var PhoneExists = await unitOfWork.GetRepository<Member>().AnyAsync(x => x.Phone == model.Email && x.Id != id);

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

            unitOfWork.GetRepository<Member>().UpdateAsync(member);

            var result = await unitOfWork.SaveChangesAsync(ct);

            return result > 0;
        }
    }

}
