using CacheAsideRedis.Services;
using Microsoft.AspNetCore.Mvc;

namespace CacheAsideRedis.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ILogger<ProductsController> _logger;
        private readonly IProductService _productService;
        public ProductsController(
            ILogger<ProductsController> logger, 
            IProductService productService)
        {
            _logger = logger;
            _productService = productService;
        }

        [HttpGet(Name = "DistributedCache")]
        public async Task<IEnumerable<Products>> Get()
        {            
            return await _productService.GetProductsFromRedis();
        }
    }
}
