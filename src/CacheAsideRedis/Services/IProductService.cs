namespace CacheAsideRedis.Services
{
    public interface IProductService
    {
        public Task<List<Products>> GetProductsFromRedis();
    }
}
