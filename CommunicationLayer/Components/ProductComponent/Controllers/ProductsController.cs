using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataModels;
using ProductComponent.Data;
using Microsoft.AspNetCore.Authorization;
using ProductComponent.DTO;
using AutoMapper;

namespace ProductComponent.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public ProductsController(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        // GET: api/Products
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ProductRead>>> GetProducts()
        {
            Console.WriteLine("--> Getting products");
            var products = await _productRepository.GetProducts();

            return Ok(_mapper.Map<IEnumerable<ProductRead>>(products));
        }

        // GET: api/Products/5
        [HttpGet("{productSlug}")]
        [AllowAnonymous]
        public async Task<ActionResult<ProductRead>> GetProduct(string productSlug)
        {
            var product = await _productRepository.GetProductBySlug(productSlug);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<ProductRead>(product));
        }

        // PUT: api/Products/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize("edit:product")]
        public async Task<IActionResult> PutProduct(int id, ProductEdit productEdit)
        {
            var product = _mapper.Map<Product>(productEdit);

            if (id != product.Id)
            {
                return BadRequest();
            }

            try
            {
                await _productRepository.EditProduct(product);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_productRepository.ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize("create:product")]
        public async Task<ActionResult<Product>> PostProduct(ProductCreate productCreate)
        {
            var product = await _productRepository.CreateProduct(_mapper.Map<Product>(productCreate));
            return CreatedAtAction("GetProduct", new { id = product.Id }, product);
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        [Authorize("delete:product")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                await _productRepository.DeleteProduct(id);
            }
            catch(DbUpdateConcurrencyException e)
            {
                if (e.Message == "Not found!")
                {
                    return NotFound();
                }
                
            }

            return NoContent();
        }
    }
}
