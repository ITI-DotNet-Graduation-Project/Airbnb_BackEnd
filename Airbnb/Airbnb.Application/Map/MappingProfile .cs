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
            CreateMap<Property, PropertyDTO>().ReverseMap();
            CreateMap<PropertyImageDto, PropertyImage>().ReverseMap();
            CreateMap<CreatePropertyDTO, Property>();
            CreateMap<UpdatePropertyImageDTO, Property>();

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



        }
    }

}
