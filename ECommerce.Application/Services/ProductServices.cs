using AutoMapper;
using ECommerce.Application.Common;
using ECommerce.Application.Contracts;
using ECommerce.Application.DTO_s.Products;
using ECommerce.Application.Params;
using ECommerce.Application.Specifications;
using ECommerce.Domain.Contracts;
using ECommerce.Domain.Entities.Products;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace ECommerce.Application.Services
{
    public class ProductServices : IProductServices
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public ProductServices(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }
        public async Task<Result<IReadOnlyList<BrandDto>>> GetAllBrandsAsync(CancellationToken ct = default)
        {
            var brands = await unitOfWork.GetRepository<ProductsBrand, int>().GetAllAsync(ct);

            var mappedBrands = mapper.Map<IReadOnlyList<ProductsBrand>, IReadOnlyList<BrandDto>>(brands);

            return Result<IReadOnlyList<BrandDto>>.Ok(mappedBrands);
        }

        public async Task<Result<PaginatedResult<ProductDto>>> GetAllProductsAsync(ProductQueryParams queryParams, CancellationToken ct = default)
        {
            var spec = new ProductSpecifications(queryParams);
            var products = await unitOfWork.GetRepository<Product, int>().GetAllWithSpecificationsAsync(spec, ct);

            var mappedProducts = mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductDto>>(products);

            var countSpec = new ProductCountSpecifications(queryParams);
            var totalCount = await unitOfWork.GetRepository<Product, int>().GetProductCountWithSpecificationsAsync(countSpec, ct);
            return Result<PaginatedResult<ProductDto>>.Ok(new PaginatedResult<ProductDto>(mappedProducts, queryParams.PageIndex, products.Count, totalCount));
        }

        public async Task<Result<IReadOnlyList<TypeDto>>> GetAllTypesAsync(CancellationToken ct = default)
        {
            var types = await unitOfWork.GetRepository<ProductsType, int>().GetAllAsync(ct);

            var mappedTypes = mapper.Map<IReadOnlyList<ProductsType>, IReadOnlyList<TypeDto>>(types);

            return Result<IReadOnlyList<TypeDto>>.Ok(mappedTypes);
        }

        public async Task<Result<ProductDto>> GetByIdAsync(int id, CancellationToken ct)
        {
            var spec = new ProductSpecifications(id);
            var Product = await unitOfWork.GetRepository<Product, int>().GetByIdWithSpecificationAsync(spec, ct);

            if (Product is null)
                return Result<ProductDto>.Fail(Error.NotFound("Product.NotFound", $"Product with id {id} not found"));


            var mappedProduct = mapper.Map<Product, ProductDto>(Product);

            return mappedProduct;
        }
    }
}
