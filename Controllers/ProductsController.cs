using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyFirstAPI.Models;
using System.Data;

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

        [HttpGet("{id}")]
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

        [HttpPut("{id}")]
        public async Task<ActionResult<Product>> PutProduct(int id, [FromBody] Product product)
        {
            if (id != product.Id)
            {
                return BadRequest("Id does not match product id");
            }
            if (!_context.Products.Any(p => p.Id == id))
            {
                return NotFound("Product does not exist. Update failed!");
            }

            try
            {
                _context.Entry(product).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DBConcurrencyException) // Someone else has modified or deleted the product at the same time
            {
                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Product>> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product is null)
            {
                return NotFound("Product does not exist. Delete failed!");
            }

            try
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
            catch (DBConcurrencyException) // Someone else has modified or deleted the product at the same time
            {
                throw;
            }

            return Ok(product); // Majority of APIs follow that we should return deleted entity
        }

        [HttpDelete("Delete")]
        public async Task<ActionResult<Product>> DeleteMultiple([FromQuery] int[] ids)
        // The name of the parameter is VERY IMPORTANT!!!
        // api/Products/delete?ids=1&ids=3&ids=6 --> works! Length =3
        // api/Products/delete?id=1&id=3&id=6 --> FAILS! Length = 0, because para name is different
        {
            if (ids.Length == 0)
            {
                return BadRequest("Ids missing. Deletion failed!");
            }

            var products = new List<Product>();

            foreach (int id in ids)
            {
                var product = await _context.Products.FindAsync(id);

                if (product is null)
                {
                    return NotFound("Product with id: " +  id + " does not exist. Nothing is deleted");
                }

                products.Add(product);
            }

            try
            {
                _context.Products.RemoveRange(products);
                await _context.SaveChangesAsync();
            }
            catch(DBConcurrencyException)
            {
                throw;
            }
            return Ok("Deletion Successful");
        }
    }
}
