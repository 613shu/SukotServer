using AutoMapper;
using SukotSystemCore.DTOs.CustomerDTOs.Request;
using SukotSystemCore.DTOs.CustomerDTOs.Response;
using SukotSystemCore.DTOs.AdminDTOs;
using SukotSystemCore.DTOs.AdminDTOs.Request;
using SukotSystemCore.DTOs.AdminDTOs.Response;
using SukotSystemCore.DTOs.RabbiDTOs;
using SukotSystemCore.DTOs.RabbiDTOs.Request;
using SukotSystemCore.DTOs.RabbiDTOs.Response;
using SukotSystemCore.DTOs.Common;
using SukotSystemCore.Models;
using SukotSystemCore.DTOs.SecretaryDTOs;

namespace SukotSystemCore.DTOs
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Customer
            CreateMap<CustomerRegisterDTO, Customer>();
            CreateMap<Customer, CustomerResponseDTO>();
            CreateMap<Customer, CustomerAdminDTO>();
            CreateMap<Customer, CustomerRabbiDTO>().ReverseMap();

            // Rabbi
            CreateMap<RabbiRegisterDTO, Rabbi>();
            // CoveredCityIds (List<int>) has no matching Rabbi property by name/type, so
            // AutoMapper leaves CoveredCities untouched here - RabbiRepository.UpdateRabbi
            // reconciles that many-to-many set explicitly from the DTO's id list instead.
            CreateMap<RabbiUpdateDTO, Rabbi>();
            CreateMap<Rabbi, RabbiResponseDTO>()
                .ForMember(dest => dest.HomeCityName, opt => opt.MapFrom(src => src.HomeCity.Name));
            CreateMap<Rabbi, RabbiAdminDTO>()
                .ForMember(dest => dest.HomeCityName, opt => opt.MapFrom(src => src.HomeCity.Name))
                .ForMember(dest => dest.Orders, opt => opt.MapFrom(src => src.HandledOrders));

            // Admin
            CreateMap<Admin, AdminResponseDTO>();
            CreateMap<AdminUpdateDTO, Admin>();

            // City
            CreateMap<City, CityDTO>();
            CreateMap<CityCreateDTO, City>();

            // Order - Customer's own view
            CreateMap<OrderCreateDTO, Order>();
            CreateMap<Order, CustomerDTOs.Response.OrderResponseDTO>()
                .ForMember(dest => dest.CityName, opt => opt.MapFrom(src => src.City.Name))
                .ForMember(dest => dest.PerformedRabbiName, opt => opt.MapFrom(
                    src => src.PerformedRabbi == null ? null : src.PerformedRabbi.FirstName + " " + src.PerformedRabbi.LastName));

            // Order - Rabbi's view
            CreateMap<Order, OrderRabbiDTO>()
                .ForMember(dest => dest.CityName, opt => opt.MapFrom(src => src.City.Name))
                .ForMember(dest => dest.RequestedCustomer, opt => opt.MapFrom(src => src.RequestedCustomer));

            // Order - Admin's view
            CreateMap<Order, OrderAdminDTO>()
                .ForMember(dest => dest.CityName, opt => opt.MapFrom(src => src.City.Name))
                .ForMember(dest => dest.RequestedCustomer, opt => opt.MapFrom(src => src.RequestedCustomer))
                .ForMember(dest => dest.PerformedRabbiName, opt => opt.MapFrom(
                    src => src.PerformedRabbi == null ? null : src.PerformedRabbi.FirstName + " " + src.PerformedRabbi.LastName));


            CreateMap<SecretaryRequstDTO,Secretary>();
            CreateMap<Secretary, SecretaryResponseDTO>();

        }
    }
}
