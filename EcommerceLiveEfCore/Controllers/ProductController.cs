using Microsoft.AspNetCore.Mvc;
using EcommerceLiveEfCore.Services;
using EcommerceLiveEfCore.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace EcommerceLiveEfCore.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly ProductService _productService;
        private readonly LoggerService _loggerService;

        public ProductController(ProductService productService, LoggerService loggerService)
        {
            this._productService = productService;
            _loggerService = loggerService;
        }

        [Authorize]
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
            return PartialView("_AddForm");
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddProductViewModel addProductViewModel)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    success = false,
                    message = "Error while saving entity to database"
                });
            }

            var result = await _productService.AddProductAsync(addProductViewModel, User);

            if (!result)
            {
                return Json(new
                {
                    success = false,
                    message = "Error while saving entity to database"
                });
            }

            string logmessage = "Entity saved successfully to database";

            _loggerService.LogInformation(logmessage);
            return Json(new
            {
                success = true,
                message = logmessage
            });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _productService.DeleteProductByIdAsync(id);

            if (!result)
            {
                return Json(new
                {
                    success = false,
                    message = "Error while deleting entity"
                });
            }

            string logmessage = "Entity deleted successfully";
            _loggerService.LogInformation(logmessage);

            return Json(new
            {
                success = true,
                message = logmessage
            });
        }

        public async Task<IActionResult> Edit(Guid id)
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

            return PartialView("_EditForm", editProductViewModel);
        }

        [HttpPost("product/edit/save")]
        public async Task<IActionResult> Edit(EditProductViewModel editProductViewModel)
        {
            var result = await _productService.UpdateProductAsync(editProductViewModel);

            if (!result)
            {
                return Json(new
                {
                    success = false,
                    message = "Error while updating entity on database"
                });
            }

            string logmessage = "Entity updated successfully";
            _loggerService.LogInformation(logmessage);

            return Json(new
            {
                success = true,
                message = logmessage
            }); ;
        }
    }
}
