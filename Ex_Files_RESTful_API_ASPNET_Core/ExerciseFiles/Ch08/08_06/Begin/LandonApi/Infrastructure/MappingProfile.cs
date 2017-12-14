using AutoMapper;
using LandonApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LandonApi.Infrastructure
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RoomEntity, Room>()
                .ForMember(dest => dest.Rate, opt => opt.MapFrom(src => src.Rate / 100.0m))
                .ForMember(dest => dest.Self, opt => opt.MapFrom(src =>
                    Link.To(nameof(Controllers.RoomsController.GetRoomByIdAsync), new { roomId = src.Id })));

            CreateMap<OpeningEntity, Opening>()
                .ForMember(dest => dest.Rate, opt => opt.MapFrom(src => src.Rate / 100m))
                .ForMember(dest => dest.StartAt, opt => opt.MapFrom(src => src.StartAt.ToUniversalTime()))
                .ForMember(dest => dest.EndAt, opt => opt.MapFrom(src => src.EndAt.ToUniversalTime()))
                .ForMember(dest => dest.Room, opt => opt.MapFrom(src =>
                    Link.To(nameof(Controllers.RoomsController.GetRoomByIdAsync), new { roomId = src.RoomId })));

            CreateMap<BookingEntity, Booking>()
                .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.Total / 100m))
                .ForMember(dest => dest.Self, opt => opt.MapFrom(src =>
                    Link.To(
                        nameof(Controllers.BookingsController.GetBookingByIdAsync),
                        new { bookingId = src.Id })))
                .ForMember(dest => dest.Room, opt => opt.MapFrom(src =>
                    Link.To(
                        nameof(Controllers.RoomsController.GetRoomByIdAsync),
                        new { roomId = src.Id })));
        }
    }
}
