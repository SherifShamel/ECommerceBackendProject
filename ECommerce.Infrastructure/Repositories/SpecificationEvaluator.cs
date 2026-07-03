using ECommerce.Domain.Common;
using ECommerce.Domain.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Infrastructure.Repositories
{
    public static class SpecificationEvaluator
    {
        public static IQueryable<TEntity> CreateQuery<TEntity, TKey>(IQueryable<TEntity> InputQuery, ISpecifications<TEntity,TKey> specifications) where TEntity : BaseEntity<TKey>
        {
            var Query = InputQuery;

            if (specifications.IncludeExpressions.Count > 0)
            {
                Query = specifications.IncludeExpressions.Aggregate(Query, (current, expression) => current.Include(expression));
            }

            if(specifications.Criteria is not null)
            {
                Query = Query.Where(specifications.Criteria);
            }
            if(specifications.OrderBy is not null)
            {
                Query = Query.OrderBy(specifications.OrderBy);
            }
            if(specifications.OrderByDesc is not null)
            {
                Query = Query.OrderByDescending(specifications.OrderByDesc);
            }
            return Query;
        }
    }
}
