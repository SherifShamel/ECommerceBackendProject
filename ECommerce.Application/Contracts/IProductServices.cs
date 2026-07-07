using ECommerce.Application.Common;
using ECommerce.Application.DTO_s.Products;
using ECommerce.Application.Params;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.Contracts
{
    public interface IProductServices
    {
        Task<Result<PaginatedResult<ProductDto>>> GetAllProductsAsync(ProductQueryParams queryParams, CancellationToken ct = default);
        Task<Result<IReadOnlyList<BrandDto>>> GetAllBrandsAsync(CancellationToken ct = default);
        Task<Result<IReadOnlyList<TypeDto>>> GetAllTypesAsync(CancellationToken ct = default);

        Task<Result<ProductDto>> GetByIdAsync(int id, CancellationToken ct);
    }
}
