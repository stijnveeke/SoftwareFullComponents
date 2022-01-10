
using System.Collections.Generic;
using System.Threading.Tasks;
using DataModels;
using ProductComponent.DTO;

namespace ProductComponent.Data {

    public interface IProductRepository
    {
        public Task<IEnumerable<Product>> GetProducts();
        public Task<Product> GetProductById(int id);
        public Task<Product> GetProductBySlug(string productSlug);
        public Task<Product> CreateProduct(Product product);
        public Task<Product> EditProduct(Product product);
        public Task DeleteProduct(int id);
        public Task DeleteProduct(string productSlug);
        public bool ProductExists(int id);
        public bool ProductExists(string productSlug);
    }
}