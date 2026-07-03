using ECommerce.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Domain.Contracts
{
    public interface IGenericRepository<TEntity,TKey> where TEntity : BaseEntity<TKey>
    {
        public void Add(TEntity entity);
        public void Update(TEntity entity);
        public void Delete(TEntity entity);


        public Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken ct = default);
        public Task<TEntity?> GetById(TKey id, CancellationToken ct = default);


        public Task<TEntity?> GetByIdWithSpecificationAsync(ISpecifications<TEntity,TKey> specifications, CancellationToken ct = default);
        public Task<IReadOnlyList<TEntity>> GetAllWithSpecificationsAsync(ISpecifications<TEntity,TKey> specifications,CancellationToken ct = default);

    }
}
