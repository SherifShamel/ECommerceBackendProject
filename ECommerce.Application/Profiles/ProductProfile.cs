using AutoMapper;
using ECommerce.Application.DTO_s.Products;
using ECommerce.Domain.Entities.Products;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.Profiles
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<ProductsBrand, BrandDto>();
            CreateMap<ProductsType, TypeDto>();

            CreateMap<Product, ProductDto>().ForMember(dist => dist.BrandName, opt => opt.MapFrom(src => src.Brand.Name))
                                            .ForMember(dist => dist.TypeName, opt => opt.MapFrom(src => src.Type.Name))
                                            .ForMember(dist => dist.PictureUrl, opt => opt.MapFrom<PictureUrlResolver>());
        }
    }
}
