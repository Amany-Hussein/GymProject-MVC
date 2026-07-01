using AutoMapper;
using GymManagement.BLL.ViewModels.MemberViewModels;
using GymManagement.BLL.ViewModels.PlanViewModels;
using GymManagement.BLL.ViewModels.SessionViewModels;
using GymManagement.DAL.Models;
using GymProject.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagement.BLL.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            SessionProfiels();
            MemberProfiels();
            PlanProfiels();
            TrainerProfiels();

        }

        private void SessionProfiels()
        {

            CreateMap<CreateSessionViewModel, Session>();
            CreateMap<Category, CategorySelectViewModel>();
            CreateMap<Trainer, TrainerSelectViewModel>();
            CreateMap<Session, SessionViewModel>()
                                                .ForMember(dest => dest.TrainerName, opt => opt.MapFrom(src => src.Trainer.Name))
                                                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.CategoryName))
                                                 .ForMember(dest => dest.AvailableSlots, opt => opt.Ignore());//will be calculated after map
            CreateMap<Session, UpdateSessionViewModel>().ReverseMap();



        }

       
        private void MemberProfiels()
        {
            CreateMap<Member, MemberViewModel>().ForMember(dest => dest.Address, opt => opt.MapFrom(src => $"{src.Address.BuildingNumber} _ {src.Address.Street} _ {src.Address.City}"))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth.ToShortDateString()));

            CreateMap<HealthRecord, HealthRecordViewModel>().ReverseMap();  // reverse 

            CreateMap<Member, MemberToUpdateViewModel>()
                                                        .ForMember(dest => dest.BuildingNumber, opt => opt.MapFrom(scr => scr.Address.BuildingNumber))
                                                        .ForMember(dest => dest.City, opt => opt.MapFrom(scr => scr.Address.City))
                                                        .ForMember(dest => dest.Street, opt => opt.MapFrom(scr => scr.Address.Street));

            CreateMap<MemberToUpdateViewModel, Member>()
                                                    .ForPath(dest => dest.Address.BuildingNumber, opt => opt.MapFrom(src => src.BuildingNumber))
                                                    .ForPath(dest => dest.Address.Street, opt => opt.MapFrom(src => src.Street))
                                                    .ForPath(dest => dest.Address.City, opt => opt.MapFrom(src => src.City))
                                                    .ForMember(des => des.Name, opt => opt.Ignore())
                                                    .ForMember(des => des.Photo, opt => opt.Ignore());


            CreateMap<CreateMemberViewModel, Member>()
                                                    .ForMember(dest => dest.Address, opt => opt.MapFrom(src => new Address
                                                    {
                                                        BuildingNumber = src.BuildingNumber,
                                                        Street = src.Street,
                                                        City = src.City,
                                                    }))
                                                    .ForMember(dest => dest.HealthRecord, opt => opt.MapFrom(src => src.HealthRecordViewModel)); 

        }
        

        private void PlanProfiels()
        {
            CreateMap<Plan, PlanViewModel>();
            CreateMap<Plan, UpdatePlanViewModel>().ReverseMap();

        }

        private void TrainerProfiels()
        {

        }
    }
}
