using MemoryCacheApp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace MemoryCacheApp.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IMemoryCache _memoryCache;
        public ProductController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }
        public IActionResult Index()
        {
            #region Option 1
            //Option 1 for check data from Memory
            //if (string.IsNullOrEmpty(_memoryCache.Get<string>("time")))
            //{
            //   _memoryCache.Set<string>("time", DateTime.Now.ToString());
            //}
            #endregion
            #region Option 2
            //Option 2 for check data from Memory
            //if (!_memoryCache.TryGetValue("time", out string timeCache))
            //{
            //    MemoryCacheEntryOptions options = new MemoryCacheEntryOptions();

            //    options.AbsoluteExpiration = DateTime.Now.AddSeconds(5);
            //    _memoryCache.Set<string>("time", DateTime.Now.ToString(), options);
            //}
            #endregion
            #region Absolute Cache Life
            // options.AbsoluteExpiration = DateTime.Now.AddSeconds(5);
            #endregion

            MemoryCacheEntryOptions options = new MemoryCacheEntryOptions();

            options.SlidingExpiration = TimeSpan.FromSeconds(15);
            options.Priority = CacheItemPriority.High;
            options.RegisterPostEvictionCallback((key, value, reason, state) =>
            {
                _memoryCache.Set<string>("callBack", $"{key}->{value}=>reason:{reason}");
            });
            _memoryCache.Set<string>("time", DateTime.Now.ToString(), options);

            List<Product> products = new List<Product>();
            Product product = new Product { Id = 1, Name = "Pen", Price = "25" };
            Product product2 = new Product { Id = 1, Name = "Pen", Price = "25" };
            products.Add(product);
            products.Add(product2);
            var control = _memoryCache.Set<Product>("product1", product);
            var control2 = _memoryCache.Set<List<Product>>("productList", products);

            return View();
        }
        public IActionResult Show()
        {
            #region GetOrCreateCache
            //Get cache or create
            //  _memoryCache.GetOrCreate<string>("time", entry => { return DateTime.Now.ToString(); });
            #endregion
            #region RemoveCache
            // _memoryCache.Remove("time");
            #endregion
            _memoryCache.TryGetValue("time", out string timeCache);
            _memoryCache.TryGetValue("callBack", out string callBack);
            _memoryCache.TryGetValue("product1", out Product product1);
            _memoryCache.TryGetValue("productList", out List<Product> productList);
            ViewBag.time = timeCache;
            ViewBag.callBack = callBack;
            ViewBag.product1 = product1;
            ViewBag.productList = productList;
            return View();
        }
    }
}
