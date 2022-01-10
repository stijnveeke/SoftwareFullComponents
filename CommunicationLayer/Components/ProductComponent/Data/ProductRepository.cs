using DataModels;
using Microsoft.EntityFrameworkCore;
using ProductComponent.Data;
using ProductComponent.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductComponent.Data
{
    public class ProductRepository : IProductRepository
    {
        private readonly ProductComponentContext _context;
        public ProductRepository(ProductComponentContext context)
        {
            _context = context;
        }
        
        public async Task<Product> CreateProduct(Product product)
        {
            _context.Product.Add(product);
            await _context.SaveChangesAsync();

            return product;
        }

        public async Task DeleteProduct(int id)
        {
            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                throw new DbUpdateConcurrencyException("Not found!");
            }

            _context.Product.Remove(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProduct(string productSlug)
        {
            var product = await _context.Product.FirstOrDefaultAsync(f => f.ProductSlug == productSlug);
            if (product == null)
            {
                throw new DbUpdateConcurrencyException("Not found!");
            }

            _context.Product.Remove(product);
            await _context.SaveChangesAsync();
        }

        public async Task<Product> EditProduct(Product product)
        {
            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return product;
        }

        public async Task<Product> GetProductById(int id)
        {
            return await _context.Product.FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<Product> GetProductBySlug(string productSlug)
        {
            return await _context.Product.FirstOrDefaultAsync(f => f.ProductSlug == productSlug);
        }

        public async Task<IEnumerable<Product>> GetProducts()
        {
            return await _context.Product.ToListAsync();
        }

        public bool ProductExists(string productSlug)
        {
            return _context.Product.Any(e => e.ProductSlug == productSlug);
        }

        public bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.Id == id);
        }
    }
}
