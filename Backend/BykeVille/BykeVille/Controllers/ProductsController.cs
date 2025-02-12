using BykeVille.BLogic;
using BykeVille.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BykeVille.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly DbBikeVilleOldContext _context;

        public ProductsController(DbBikeVilleOldContext context)
        {
            _context = context;
        }

        // GET: Ottiene tutti i prodotti
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            try
            {
                return await _context.Products.ToListAsync();
            }
            catch (Exception ex)
            {
                LogManager.SaveLogBackend(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: Ritorna i 10 prodotti che sono stati aggiunti più di recente
        [HttpGet("NewArrival")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsNewArrival()
        {
            IEnumerable<Product> products;
            try
            {
                products = await _context.Products
                    .OrderByDescending(p => p.SellStartDate)
                    .Take(100)
                    .ToListAsync();

                products = products.DistinctBy(p => p.ProductModelId).Take(10);
            }
            catch (Exception ex)
            {
                LogManager.SaveLogBackend(ex);
                return BadRequest(ex.Message);
            }

            return Ok(products);
        }

        // GET: Ritorna i 10 prodotti più acquistati
        [HttpGet("MostPurchased")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsMostPurchased()
        {
            try
            {
                var mostPurchasedProducts = await _context.SalesOrderDetails
                    .GroupBy(sod => sod.ProductId)
                    .Select(group => new
                    {
                        Product = group.FirstOrDefault().Product,
                        TotalQuantitySold = group.Sum(sod => sod.OrderQty)
                    })
                    .OrderByDescending(p => p.TotalQuantitySold)
                    .Take(100)
                    .ToListAsync();
                IEnumerable<Product> products = mostPurchasedProducts.Select(x => x.Product);

                products = products.DistinctBy(p => p.ProductModelId).Take(10);
                return Ok(products);
            }
            catch (Exception ex)
            {
                LogManager.SaveLogBackend(ex);
                return BadRequest(ex.Message);
            }
        }

        // GET: Ritorna le categorie e le sottocategorie per la navbar
        [HttpGet("Categories")]
        public async Task<ActionResult<IEnumerable<ProductCategory>>> GetProductsCategories()
        {
            try
            {
                var parentCategoriesWithChildren = await _context.ProductCategories
                    .Where(c => c.ParentProductCategoryId == null)
                    .Include(parent => parent.InverseParentProductCategory)
                    .ToListAsync();

                return Ok(parentCategoriesWithChildren);
            }
            catch (Exception ex)
            {
                LogManager.SaveLogBackend(ex);
                return BadRequest(ex.Message);
            }
        }

        // GET: Ritorna tutti i prodotti con lo stesso ProductModelId
        [HttpGet("Models")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductModels(int productModelId)
        {
            try
            {
                var productModels = await _context.Products
                                 .Where(p => p.ProductModelId == productModelId)
                                 .ToListAsync();

                return Ok(productModels);
            }
            catch (Exception ex)
            {
                LogManager.SaveLogBackend(ex);
                return BadRequest(ex.Message);
            }
        }

        // GET: Ritorna i prodotti in base al nome
        [HttpGet("ByName")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsByName(string name)
        {
            try
            {
                IEnumerable<Product> products = await _context.Products
                            .Where(p => p.Name.Contains(name))
                            .ToListAsync();

                products = products.DistinctBy(p => p.ProductModelId);

                return Ok(products);
            }
            catch (Exception ex)
            {
                LogManager.SaveLogBackend(ex);
                return BadRequest(ex.Message);
            }
        }

        // GET: Ritorna i prodotti per una specifica categoria
        [HttpGet("ByCategories")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsByCategories(string categoryName)
        {
            try
            {
                var categoryId = await _context.ProductCategories
                                .Where(c => c.Name.Equals(categoryName))
                                .Select(c => c.ProductCategoryId)
                                .FirstOrDefaultAsync();

                if (categoryId == 0)
                {
                    return NotFound("No category found with the provided name.");
                }

                IEnumerable<Product> products = await _context.Products
                    .Where(p => p.ProductCategoryId == categoryId)
                    .ToListAsync();

                if (products == null || !products.Any())
                {
                    return NotFound("No products found for the given category.");
                }

                products = products.DistinctBy(p => p.ProductModelId);

                return Ok(products);
            }
            catch (Exception ex)
            {
                LogManager.SaveLogBackend(ex);
                return BadRequest(ex.Message);

            }
        }

        // GET: Ritorna la descrizione di un prodotto in base al ProductModelId
        [HttpGet("DescriptionByProductModelID")]
        public async Task<ActionResult<String>> GetProductDescription(int productModelId)
        {
            try
            {
                var description = await _context.ProductModelProductDescriptions
                    .Where(p => p.ProductModelId == productModelId && p.Culture == "en")
                    .Include(p => p.ProductDescription)
                    .Select(p => p.ProductDescription.Description)
                    .FirstOrDefaultAsync();

                if (description == null)
                {
                    return NotFound("No description found for the given productModelId");
                }

                return Ok(description);
            }
            catch (Exception ex)
            {
                LogManager.SaveLogBackend(ex);
                return BadRequest(ex.Message);
            }
        }

        // GET: Ritorna un prodotto in base all'id
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);

                if (product == null)
                {
                    return NotFound();
                }

                return product;
            }
            catch (Exception ex)
            {
                LogManager.SaveLogBackend(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // PUT: Modifica un prodotto in base all'id
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            if (id != product.ProductId)
            {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    LogManager.SaveLogBackend(ex);
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                }
            }

            return NoContent();
        }

        // POST: Aggiunge un nuovo prodotto
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            try
            {
                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetProduct", new { id = product.ProductId }, product);
            }
            catch (Exception ex)
            {
                LogManager.SaveLogBackend(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // DELETE: Elimina un prodotto in base all'id
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null)
                {
                    return NotFound();
                }

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                LogManager.SaveLogBackend(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // Verifica se un prodotto esiste
        private bool ProductExists(int id)
        {
            try
            {
                return _context.Products.Any(e => e.ProductId == id);
            }
            catch (Exception ex)
            {
                LogManager.SaveLogBackend(ex);
                return false;
            }
        }
    }
}
