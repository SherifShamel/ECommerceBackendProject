using ECommerce.Domain.Contracts;
using ECommerce.Domain.Entities.Baskets;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace ECommerce.Infrastructure.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IDatabase _database;
        public BasketRepository(IConnectionMultiplexer connection)
        {
            _database = connection.GetDatabase();
        }
        public async Task<CustomerBasket?> CreateOrUpdateBasketAsync(CustomerBasket basket, TimeSpan? timeToLive = null, CancellationToken ct = default)
        {
            var Json = JsonSerializer.Serialize(basket);
            var Success = await _database.StringSetAsync(basket.Id, Json, timeToLive ?? TimeSpan.FromDays(30));
            return Success ? basket : null;
        }

        public async Task<bool> DeleteBasketAsync(string basketId, CancellationToken ct = default)
        {
            return await _database.KeyDeleteAsync(basketId);
        }

        public async Task<CustomerBasket?> GetBasketAsync(string basketId, CancellationToken ct = default)
        {
            var Basket = await _database.StringGetAsync(basketId);
            if (Basket.IsNullOrEmpty)
                return null;

            return JsonSerializer.Deserialize<CustomerBasket>(Basket.ToString());
        }
    }
}
