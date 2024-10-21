
using CacheAsideRedis.Extensions;
using Microsoft.Extensions.Caching.Distributed;

namespace CacheAsideRedis.Services
{
    public class ProductService : IProductService
    {
        private readonly IDistributedCache _cache;
        private const string cacheKey = "products";
        public ProductService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<List<Products>> GetProductsFromRedis()
        {
            var cacheOptions = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(5))
                .SetSlidingExpiration(TimeSpan.FromSeconds(5));

            var products = _cache.GetOrSetAsync(
                cacheKey,
                async () =>
                {
                    return await GetValuesFromDbAsync();
                },
                cacheOptions)!;
            return await products;
        }

        private async Task<List<Products>> GetValuesFromDbAsync()
        {
            List<Products> products = new()
            {
                new Products { Id = 1, CreateDate = DateTime.Now, Category = "Category 1", Description = "Produto 1" },
                new Products { Id = 2, CreateDate = DateTime.Now, Category = "Category 2", Description = "Produto 2" },
                new Products { Id = 3, CreateDate = DateTime.Now, Category = "Category 3", Description = "Produto 3" }
            };

            Task<List<Products>> productTask = Task<List<Products>>.Factory.StartNew(() =>
            {
                Thread.Sleep(1000);
                return products;
            });

            return await productTask;
        }
    }
}
