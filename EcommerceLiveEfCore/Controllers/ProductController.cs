using EcommerceLiveEfCore.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EcommerceLiveEfCore.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductService _productService;

        public ProductController(ProductService productService)
        {
            //inietto il servizio all'interno del controller
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            var productsList = await _productService.GetAllProductsAsync();

            return View(productsList);
        }
    }
}
