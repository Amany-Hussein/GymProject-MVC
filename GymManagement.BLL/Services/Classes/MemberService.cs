using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.MemberViewModels;
using GymManagement.DAL.Models;
using GymManagement.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagement.BLL.Services.Classes
{
    public class MemberService : IMemberService
    {
        //DataBase Connection 
        private readonly IGenericRepository<Member> MemberRepo;

        public MemberService(IGenericRepository<Member> MemberRepo)
        {
            this.MemberRepo = MemberRepo;
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
    }
}
