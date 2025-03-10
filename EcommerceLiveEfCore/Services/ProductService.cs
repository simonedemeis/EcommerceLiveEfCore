using EcommerceLiveEfCore.Data;
using EcommerceLiveEfCore.ViewModels;
using EcommerceLiveEfCore.Models;
using Microsoft.EntityFrameworkCore;

namespace EcommerceLiveEfCore.Services
{
    public class ProductService
    {
        private readonly EcommerceLiveEfCoreDbContext _context;

        public ProductService(EcommerceLiveEfCoreDbContext context)
        {
            //Iniettiamo il dbcontext all'interno del servizio
            _context = context;
        }

        public async Task<ProductsListViewModel> GetAllProductsAsync()
        {
            try
            {
                var productsList = new ProductsListViewModel();

                //tramite ToListAsync trasformo l'outp della select che efcore effettua in una lista
                productsList.Products = await _context.Products.ToListAsync();

                return productsList;
            }
            catch
            {
                return new ProductsListViewModel() { Products = new List<Product>()};
            }
        }
    }
}
