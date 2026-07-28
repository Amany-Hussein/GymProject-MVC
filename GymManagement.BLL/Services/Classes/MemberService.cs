using AutoMapper;
using GymManagement.BLL.Common;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.MemberViewModels;
using GymManagement.DAL.Models;
using GymManagement.DAL.Repositories.Classes;
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
        private readonly IMapper mapper;
        private readonly IAttachmentService attachmentService;

        // object of UintOfWork
        public MemberService(IUnitOfWork unitOfWork , IMapper mapper , IAttachmentService attachmentService)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.attachmentService = attachmentService;
        }

        public async Task<bool> CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct = default)
        {

            // Email exist or not
            var EmailExist = await unitOfWork.GetRepository<Member>().AnyAsync(x => x.Email == model.Email);

            // Phone exist or not
            var PhoneExist = await unitOfWork.GetRepository<Member>().AnyAsync(x => x.Phone == model.Phone);

            if (EmailExist || PhoneExist)
                return false;

            //upload photo
            var StoredPhotoName = await attachmentService.UploadAsync(model.PhotoFile.OpenReadStream(), model.PhotoFile.FileName, "MembersPhoto");
            if (string.IsNullOrWhiteSpace(StoredPhotoName)) return false;


            // Map from CreateMemberViewModel => Member
            var member = mapper.Map<Member>(model);

            // map the photo manually
            member.Photo = StoredPhotoName;


            //new Member()
            //{
            //    Name = model.Name,
            //    Phone = model.Phone,
            //    Email = model.Email,
            //    Gender = model.Gender,
            //    DateOfBirth = model.DateOfBirth,
            //    Address = new Address()
            //    {
            //        City = model.City,
            //        BuildingNumber = model.BuildingNumber,
            //        Street = model.Street,
            //    },
            //    HealthRecord = new HealthRecord()
            //    {
            //        Height = model.HealthRecordViewModel.Height,
            //        Weight = model.HealthRecordViewModel.Weight,
            //        BloodType = model.HealthRecordViewModel.BloodType,
            //        Note = model.HealthRecordViewModel.Note,
            //    },

            //};
            unitOfWork.GetRepository<Member>().AddAsync(member);

            var result = await unitOfWork.SaveChangesAsync(ct);

            if (result > 0)
            {
                return true;
            }// if did not make the else , ohterwise member added or not the photo will be uploaded so delete it
            else
            {
                // delete photo if failed
                attachmentService.Delete(StoredPhotoName, "MembersPhoto");
                return false;
            }
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
                var memberViewModel = mapper.Map<MemberViewModel>(Member);


                //    new MemberViewModel()
                //{
                //    Name = Member.Name,
                //    Phone = Member.Phone,
                //    Photo = Member.Photo,
                //    Email = Member.Email,
                //    Id = Member.Id,
                //    Gender = Member.Gender.ToString(),
                //};
                result.Add(memberViewModel);
            }
            return result;
        }

        public async Task<MemberViewModel?> GetMemberDetailsByIdAsync(int MemberId, CancellationToken ct = default)
        {
            //Get Member By Id
            var member = await unitOfWork.GetRepository<Member>().GetByIdAsync(MemberId , ct);

            if(member == null) return null;


            //var model = new MemberViewModel()
            //{

            //    Name = member.Name,
            //    Phone = member.Phone,
            //    Email = member.Email,
            //    Photo = member.Photo,
            //    Gender = member.Gender.ToString(),
            //    DateOfBirth = member.DateOfBirth.ToShortDateString(),
            //    Address = $"{member.Address.BuildingNumber} _ {member.Address.Street} _ {member.Address.City}",
            //    // plan name ?????????
            //    // membership start & end date  ?????????

            //}; [old]

            //[new]
            //var model = _mapper.Map<Member, MemberViewModel>(member); or
            // Map from Member => MemberViewModel
            var model = mapper.Map<MemberViewModel>(member);


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
                // Map from HealthRecord => HealthRecordViewModel

                return mapper.Map<HealthRecordViewModel>(record);

                //return new HealthRecordViewModel()
                //{
                //    Height = record.Height,
                //    Weight = record.Weight,
                //    BloodType = record.BloodType,
                //    Note = record.Note,
                //};
            }
        }

        public async Task<MemberToUpdateViewModel> GetMemberToUpdateAsync(int MemberId, CancellationToken ct = default)
        {
            var Member = await unitOfWork.GetRepository<Member>().GetByIdAsync(MemberId, ct);

            if (Member == null)
                return null;
            else
            {
                // Map from Member => MemberToUpdateViewModel
                return mapper.Map<MemberToUpdateViewModel>(Member);

                // OLD way
                //return new MemberToUpdateViewModel()
                //{
                //    Name = Member.Name,
                //    Phone = Member.Phone,
                //    Photo = Member.Photo,
                //    Email = Member.Email,
                //    BuildingNumber = Member.Address.BuildingNumber,
                //    City = Member.Address.City,
                //    Street = Member.Address.Street,
                //};
            }
        }

        public async Task<bool> UpdateMemberAsync(int id, MemberToUpdateViewModel model, CancellationToken ct = default)
        {
            //get mmeber
            var member = await unitOfWork.GetRepository<Member>().GetByIdAsync(id, ct);
            //check the if any onther member has phone or email
            var emailexist = await unitOfWork.GetRepository<Member>().AnyAsync(X => X.Email == model.Email && X.Id != id); //not the current member cause may the same one save the same email while editing
            var phoneexist = await unitOfWork.GetRepository<Member>().AnyAsync(X => X.Phone == model.Phone && X.Id != id);
            if (emailexist || phoneexist) return false;
            //membertoupdateviewmodel to member
            //  _mapper.Map<Member>(member); wrong => create new object the address is null here
            mapper.Map<MemberToUpdateViewModel, Member>(model, member);
            member.UpdatedAt = DateTime.Now;
            unitOfWork.GetRepository<Member>().UpdateAsync(member);
            var result = await unitOfWork.SaveChangesAsync(ct);
            return result > 0;
        }
    }

}
