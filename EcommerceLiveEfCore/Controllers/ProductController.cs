using Microsoft.AspNetCore.Mvc;
using EcommerceLiveEfCore.Services;
using EcommerceLiveEfCore.ViewModels;

namespace EcommerceLiveEfCore.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductService _productService;

        public ProductController(ProductService productService)
        {
            this._productService = productService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("product/get-all")]
        public async Task<IActionResult> ListProducts()
        {
            var productsList = await _productService.GetAllProductsAsync();

            return PartialView("_ProductsList", productsList);
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddProductViewModel addProductViewModel)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Error while saving entity to database";
                return RedirectToAction("Index");
            }

            var result = await _productService.AddProductAsync(addProductViewModel);

            if (!result)
            {
                TempData["Error"] = "Error while saving entity to database";
            }

            return RedirectToAction("Index");
        }

        [Route("product/details/{id:guid}")]
        public async Task<IActionResult> Details(Guid id)
        {
            var product = await _productService.GetProductByIdAsync(id);

            if (product == null)
            {
                TempData["Error"] = "Error while finding entity on database";
                return RedirectToAction("Index");
            }

            var productDetailsViewModel = new ProductDetailsViewModel()
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Category = product.Category
            };

            return Json(productDetailsViewModel);
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _productService.DeleteProductByIdAsync(id);

            if (!result)
            {
                TempData["Error"] = "Error while deleting entity from database";
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit([FromQuery]Guid id)
        {
            var product = await _productService.GetProductByIdAsync(id);

            if(product == null)
            {
                return RedirectToAction("Index");
            };

            var editProductViewModel = new EditProductViewModel()
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Category = product.Category
            };

            return View(editProductViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditProductViewModel editProductViewModel)
        {
            var result = await _productService.UpdateProductAsync(editProductViewModel);

            if (!result)
            {
                TempData["Error"] = "Error while updating entity on database";
            }

            return RedirectToAction("Index");
        }
    }
}
