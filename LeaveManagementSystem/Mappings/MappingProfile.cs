using AutoMapper;
using LeaveManagementSystem.DTOs;
using LeaveManagementSystem.Models;

namespace LeaveManagementSystem.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<LeaveRequest, LeaveRequestDto>()
                .ForMember(dest => dest.LeaveType, opt => opt.MapFrom(src => src.LeaveType.ToString()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<CreateLeaveRequestDto, LeaveRequest>()
                .ForMember(dest => dest.LeaveType, opt => opt.MapFrom(src => Enum.Parse<LeaveType>(src.LeaveType)))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Status.Pending))
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

            CreateMap<UpdateLeaveRequestDto, LeaveRequest>()
                .ForMember(dest => dest.LeaveType, opt => opt.MapFrom(src => Enum.Parse<LeaveType>(src.LeaveType)))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Enum.Parse<Status>(src.Status)));
        }
    }
}