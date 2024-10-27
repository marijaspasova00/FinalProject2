using AutoMapper;
using FinalProjectAmPlansForLoans.Domain.Enums;
using FinalProjectAmPlansForLoans.Domain.Models;
using FinalProjectAmPlansForLoans.ViewModels;
using System;

namespace FinalProjectAmPlansForLoans.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductViewModel>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<ProductViewModel, Product>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Enum.Parse<ProductStatus>(src.Status)));

            CreateMap<LoanInput, LoanInputViewModel>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.ProductName))
                .ForMember(dest => dest.PaymentFrequency, opt => opt.MapFrom(src => src.PaymentFrequency.ToString()));

            CreateMap<LoanInputViewModel, LoanInput>()
                .ForMember(dest => dest.PaymentFrequency, opt => opt.MapFrom(src => Enum.Parse<PaymentFrequency>(src.PaymentFrequency)));


            CreateMap<AmPlan, AmPlanViewModel>()
                .ForMember(dest => dest.PaymentFrequency, opt => opt.MapFrom(src => src.PaymentFrequency.ToString()));

            CreateMap<AmPlanViewModel, AmPlan>()
                .ForMember(dest => dest.PaymentFrequency, opt => opt.MapFrom(src => Enum.Parse<PaymentFrequency>(src.PaymentFrequency)));
        }
    }
}
