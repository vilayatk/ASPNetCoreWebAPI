using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyFirstAPI.Models;

namespace MyFirstAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController] // Attribute
    public class ProductsController : ControllerBase
    {
        private readonly ShopContext _context;

        public ProductsController(ShopContext context)
        {
            _context = context;

            _context.Database.EnsureCreated();
        }
        [HttpGet]
        public ActionResult GetAllProducts()
        {
            return Ok(_context.Products.ToArray()); // Status Code 200 OK
        }

        [HttpGet, Route("api/Products/{id}")] // OR  Route("{id}") OR [HttpGet("{id}")]
        public ActionResult GetProduct(int id) // id is passed from {id} parameter from the Route
        {
            var product = _context.Products.Find(id);
            return product is null ? NotFound("Product with id: " + id + " does not exist") : Ok(product);
            // Not found : 204 Proper error handling
            // Ok : Return product with Product
        }
    }
}
