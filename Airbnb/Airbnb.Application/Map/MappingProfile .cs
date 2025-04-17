using Airbnb.Application.DTOs.Booking;
using Airbnb.Application.DTOs.Property;
using Airbnb.Application.DTOs.PropertyCategory;
using Airbnb.Application.DTOs.PropertyImage;
using Airbnb.Application.DTOs.Review;
using Airbnb.DATA.models;
using AutoMapper;

namespace Airbnb.Application.Map
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Property, PropertyDTO>().ForMember(dest => dest.MaxGuest, opt => opt.MapFrom(src => src.Maxgeusts)).
                ReverseMap();
            CreateMap<PropertyImageDto, PropertyImage>().ReverseMap();
            CreateMap<CreatePropertyDTO, Property>();
            CreateMap<UpdatePropertyImageDTO, Property>();
            CreateMap<Booking, BookingResponseDTO>()
            .ForMember(dest => dest.PropertyTitle,
                       opt => opt.MapFrom(src => src.Property.Title))
            .ForMember(dest => dest.PropertyImage,
                       opt => opt.MapFrom(src => src.Property.PropertyImages.FirstOrDefault().ImageUrl));

            CreateMap<PropertyImage, PropertyImageDTO>().ReverseMap();
            CreateMap<CreatePropertyImageDTO, PropertyImage>();
            CreateMap<UpdatePropertyImageDTO, PropertyImage>();

            CreateMap<PropertyCategory, PropertyCategoryDTO>().ReverseMap();
            CreateMap<CreatePropertyCategoryDTO, PropertyCategory>();
            CreateMap<UpdatePropertyCategoryDTO, PropertyCategory>();

            CreateMap<Review, ReviewDTO>().ReverseMap();
            CreateMap<CreateReviewDTO, Review>();
            CreateMap<UpdateReviewDTO, Review>();

            CreateMap<Booking, BookingDTO>().ReverseMap();
            CreateMap<CreateBookingDTO, Booking>();
            CreateMap<UpdateBookingDTO, Booking>();
            CreateMap<Availability, AvailabilityDto>();

            CreateMap<Review, ReviewDTO>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src =>
                src.User != null ? $"{src.User.FirstName} {src.User.LastName}" : "Anonymous"))
            .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Rating ?? 0))
            .ForMember(dest => dest.Comment, opt => opt.MapFrom(src => src.Comments ?? string.Empty));

        }
    }

}
