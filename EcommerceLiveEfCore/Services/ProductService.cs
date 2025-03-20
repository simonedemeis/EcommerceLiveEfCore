using EcommerceLiveEfCore.Data;
using EcommerceLiveEfCore.Models;
using EcommerceLiveEfCore.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace EcommerceLiveEfCore.Services
{
    public class ProductService
    {

        private readonly ApplicationDbContext _context;
        private readonly LoggerService _loggerService;
        private readonly UserManager<ApplicationUser> _userManager;
        public ProductService(ApplicationDbContext context, LoggerService loggerService, UserManager<ApplicationUser> userManager)
        {
            this._context = context;
            this._loggerService = loggerService;
            this._userManager = userManager;
        }

        private async Task<bool> SaveAsync()
        {
            try
            {
                var rowsAffected = await _context.SaveChangesAsync();

                if(rowsAffected > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch(Exception ex)
            {
                _loggerService.LogError(ex.Message);
                return false;
            }
        }

        public async Task<ProductsListViewModel> GetAllProductsAsync()
        {
            var productsList = new ProductsListViewModel();

            try
            {
                productsList.Products = await _context.Products.Include(p => p.User).ToListAsync();
                _loggerService.LogInformation("Products list requested by admin");
            }
            catch(Exception ex)
            {
                productsList.Products = null;
                _loggerService.LogError(ex.Message);
            }
            
            return productsList;
        }

        public async Task<bool> AddProductAsync(AddProductViewModel addProductViewModel, ClaimsPrincipal userPrincipal)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(userPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value);
                
                var product = new Product()
                {
                    Id = Guid.NewGuid(),
                    Name = addProductViewModel.Name,
                    Description = addProductViewModel.Description,
                    Price = addProductViewModel.Price,
                    Category = addProductViewModel.Category,
                    UserId = user.Id
                };

                _context.Products.Add(product);

                return await SaveAsync();
            }
            catch(Exception ex)
            { 
                _loggerService.LogError(ex.Message);
                return false; 
            }
        }

        public async Task<Product?> GetProductByIdAsync(Guid id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);

                if (product == null)
                {
                    return null;
                }

                return product;
            }
            catch(Exception ex)
            {
                _loggerService.LogError(ex.Message);
                return null;
            }
        }

        public async Task<bool> DeleteProductByIdAsync(Guid id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);

                if (product == null)
                {
                    _loggerService.LogWarning($"Product not found");
                    return false;
                }
                
                _context.Products.Remove(product);

                return await SaveAsync();
            }
            catch(Exception ex)
            {
                _loggerService.LogError(ex.Message);
                return false;
            }
        }

        public async Task<bool> UpdateProductAsync(EditProductViewModel editProductViewModel)
        {
            try
            {
                var product = await _context.Products.FindAsync(editProductViewModel.Id);

                if (product == null)
                {
                    return false;
                }

                product.Name = editProductViewModel.Name;
                product.Description = editProductViewModel.Description;
                product.Price = editProductViewModel.Price;
                product.Category = editProductViewModel.Category;

                return await SaveAsync();
            }
            catch(Exception ex)
            {
                _loggerService.LogError(ex.Message);
                return false;
            }
        }
    }
}
