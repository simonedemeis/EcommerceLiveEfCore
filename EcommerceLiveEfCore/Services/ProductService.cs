using EcommerceLiveEfCore.Data;
using EcommerceLiveEfCore.Models;
using EcommerceLiveEfCore.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace EcommerceLiveEfCore.Services
{
    public class ProductService
    {

        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            this._context = context;
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
            catch
            {
                return false;
            }
        }

        public async Task<ProductsListViewModel> GetAllProductsAsync()
        {
            var productsList = new ProductsListViewModel();

            productsList.Products = await _context.Products.ToListAsync();

            return productsList;
        }

        public async Task<bool> AddProductAsync(AddProductViewModel addProductViewModel)
        {
            var product = new Product()
            {
                Id = Guid.NewGuid(),
                Name = addProductViewModel.Name,
                Description = addProductViewModel.Description,
                Price = addProductViewModel.Price,
                Category = addProductViewModel.Category
            };

            _context.Products.Add(product);

            return await SaveAsync();
        }

        public async Task<ProductDetailsViewModel> GetProductByIdAsync(Guid id)
        {
            var product = await _context.Products.FindAsync(id);

            if(product == null)
            {
                return new ProductDetailsViewModel();
            }

            var details = new ProductDetailsViewModel()
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Category = product.Category
            };

            return details;
        }

        public async Task<bool> DeleteProductByIdAsync(Guid id)
        {
            var product = await _context.Products.FindAsync(id);

            if(product == null)
            {
                return false;
            }

            _context.Products.Remove(product);

            return await SaveAsync();
        }

        public async Task<bool> UpdateProductAsync(ProductDetailsViewModel productDetailsViewModel)
        {
            var product = await _context.Products.FindAsync(productDetailsViewModel.Id);

            if(product == null)
            {
                return false;
            }

            product.Name = productDetailsViewModel.Name;
            product.Description = productDetailsViewModel.Description;
            product.Price = productDetailsViewModel.Price;
            product.Category = productDetailsViewModel.Category;

            return await SaveAsync();
        }
    }
}
