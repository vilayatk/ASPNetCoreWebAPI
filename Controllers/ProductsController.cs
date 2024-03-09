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

        /*
            [HttpGet, Route("api/Products/{id}")] // OR  Route("{id}") OR [HttpGet("{id}")]
            The int argument comes[FromRoute] -> WebAPI is clever enough to get from Route itself
            we can even get it[FromBody], Request Body
            [FromQuery] -> Data from URL
        */
        //public ActionResult GetProduct(int id) // id is passed from {id} parameter from the Route
        //{
        //    var product = _context.Products.Find(id);
        //    return product is null ? NotFound("Product with id: " + id + " does not exist") : Ok(product);
        //    // Not found : 404 Proper error handling
        //    // Ok : Return product with Product
        //}

        [HttpGet, Route("api/Products/{id}")]
        public async Task<ActionResult> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            return product is null ? NotFound("Product with id: " + id + " does not exist") : Ok(product);
            // Not found : 404 Proper error handling
            // Ok : Return product with Product
        }

        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product) // Interface to return Custom Action Results
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            // Status 201 Created! // We call the GetProduct Method to return the product by id
            return CreatedAtAction("GetProduct",
                new { id = product.Id }, product);
        }
    }
}
