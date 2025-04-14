using Airbnb.Application.DTOs.Property;
using Airbnb.Application.DTOs.PropertyImage;
using Airbnb.Application.DTOs.PropertyCategory;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using AutoMapper;
using Airbnb.DATA.models;
using Airbnb.Application.DTOs.Review;
using Airbnb.Application.DTOs.Booking;

namespace Airbnb.Application.Map
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Property, PropertyDTO>().ReverseMap();
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
