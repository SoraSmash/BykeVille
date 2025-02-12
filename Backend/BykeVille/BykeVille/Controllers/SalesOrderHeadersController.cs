using BykeVille.BLogic;
using BykeVille.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BykeVille.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SalesOrderHeadersController : ControllerBase
    {
        private readonly DbBikeVilleOldContext _context;

        public SalesOrderHeadersController(DbBikeVilleOldContext context)
        {
            _context = context;
        }

        // GET: Ottiene un SalesOrderHeader in base all'Id
        [HttpGet("{id}")]
        public async Task<ActionResult<SalesOrderHeader>> GetSalesOrderHeader(int id)
        {
            try
            {
                var salesOrderHeader = await _context.SalesOrderHeaders
                    .Include(s => s.SalesOrderDetails)
                    .Where(s => s.SalesOrderId == id)
                    .FirstOrDefaultAsync();

                if (salesOrderHeader == null)
                {
                    return NotFound();
                }

                return salesOrderHeader;
            }
            catch (Exception ex)
            {
                LogManager.SaveLogBackend(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: Ottiene il SalesOrderHeader attivo per un specifico cliente
        [Authorize]
        [HttpGet("ByCustomerID/{customerId}")]
        public async Task<ActionResult<IEnumerable<SalesOrderHeader>>> GetSalesOrderHeaderByCustomerId(int customerId)
        {
            try
            {
                var salesOrderHeaders = await _context.SalesOrderHeaders
                    .Where(soh => soh.CustomerId == customerId && soh.OnlineOrderFlag == true)
                    .Include(soh => soh.SalesOrderDetails)
                    .ThenInclude(sod => sod.Product)
                    .ToListAsync();

                if (salesOrderHeaders == null || salesOrderHeaders.Count == 0)
                {
                    return null;
                }

                return Ok(salesOrderHeaders);
            }
            catch (Exception ex)
            {
                LogManager.SaveLogBackend(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: Ottiene un elenco di SalesOrderHeader già effettuati in base all'Id del cliente
        [Authorize]
        [HttpGet("AllByCustomerID/{customerId}")]
        public async Task<ActionResult<IEnumerable<SalesOrderHeader>>> GetSalesOrderHeadersByCustomerId(int customerId)
        {
            try
            {
                var salesOrderHeaders = await _context.SalesOrderHeaders
                    .Where(soh => soh.CustomerId == customerId && soh.OnlineOrderFlag == false)
                    .Include(soh => soh.SalesOrderDetails)
                    .ThenInclude(sod => sod.Product)
                    .ToListAsync();

                if (salesOrderHeaders == null || salesOrderHeaders.Count == 0)
                {
                    return null;
                }

                return Ok(salesOrderHeaders);
            }
            catch (Exception ex)
            {
                LogManager.SaveLogBackend(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        // POST: Aggiunge un nuovo SalesOrderHeader
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<SalesOrderHeader>> PostSalesOrderHeader(SalesOrderHeader salesOrderHeader)
        {
            Random random = new Random();

            try
            {
                if (salesOrderHeader.SalesOrderId != 0)
                {
                    salesOrderHeader.SalesOrderId = 0;
                }

                foreach (var s in salesOrderHeader.SalesOrderDetails)
                {
                    if (s.SalesOrderDetailId != 0)
                        s.SalesOrderDetailId = 0;

                    s.Rowguid = Guid.NewGuid();
                    _context.Entry(s).State = EntityState.Added;
                }

                salesOrderHeader.Rowguid = Guid.NewGuid();
                salesOrderHeader.OnlineOrderFlag = true;
                salesOrderHeader.Status = 0;
                salesOrderHeader.PurchaseOrderNumber = "PO" + random.NextInt64(100000000, 99999999999).ToString();
                salesOrderHeader.AccountNumber = "10-4020-00" + random.NextInt64(1000, 9999).ToString();

                _context.SalesOrderHeaders.Add(salesOrderHeader);

                await _context.SaveChangesAsync();

                return CreatedAtAction("GetSalesOrderHeader", new { id = salesOrderHeader.SalesOrderId }, salesOrderHeader);
            }
            catch (Exception ex)
            {
                LogManager.SaveLogBackend(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // POST: Aggiunge un nuovo SalesOrderDetail
        [Authorize]
        [HttpPost("SalesOrderDetail")]
        public async Task<IActionResult> PostSalesOrderDetail(SalesOrderDetail salesOrderDetail)
        {
            try
            {
                // Crea un nuovo SalesOrderDetail, escludendo SalesOrderDetailID
                var newDetail = new SalesOrderDetail
                {
                    SalesOrderId = salesOrderDetail.SalesOrderId, // Imposta l'ID dell'ordine a cui appartiene
                    ProductId = salesOrderDetail.ProductId, // Imposta l'ID del prodotto
                    OrderQty = salesOrderDetail.OrderQty, // Imposta la quantità
                    UnitPrice = salesOrderDetail.UnitPrice, // Imposta il prezzo unitario
                    UnitPriceDiscount = salesOrderDetail.UnitPriceDiscount, // Imposta lo sconto
                    LineTotal = salesOrderDetail.LineTotal, // Imposta il totale della linea
                    Rowguid = Guid.NewGuid(), // Imposta il GUID della riga
                    ModifiedDate = DateTime.UtcNow // Imposta la data di modifica (o altro valore)
                };

                // Aggiungi il nuovo dettaglio all'elenco
                _context.SalesOrderDetails.Add(newDetail);

                try
                {
                    // Salva le modifiche nel database
                    await _context.SaveChangesAsync();

                    var salesOrderHeader = await _context.SalesOrderHeaders
                    .Where(s => s.SalesOrderId == newDetail.SalesOrderId)
                    .FirstOrDefaultAsync();

                    salesOrderHeader.TaxAmt = Math.Round(salesOrderHeader.SubTotal * 0.22m, 2);
                    salesOrderHeader.Freight = Math.Round(salesOrderHeader.SubTotal * 0.05m, 2);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    return StatusCode(500, "Errore durante il salvataggio.");
                }

                // Restituisci il risultato, indicando la risorsa creata
                return Ok();
            }
            catch (Exception ex)
            {
                LogManager.SaveLogBackend(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // DELETE: Elimina un SalesOrderDetail in base all'Id
        [Authorize]
        [HttpDelete("SalesOrderDetail")]
        public async Task<IActionResult> DeleteSalesOrderDetail(int id)
        {
            try
            {
                var salesOrderDetail = await _context.SalesOrderDetails
                    .Include(sod => sod.SalesOrder)
                    .ThenInclude(soh => soh.SalesOrderDetails)
                    .Where(sod => sod.SalesOrderDetailId == id).FirstAsync();

                Console.WriteLine(salesOrderDetail.SalesOrder.SalesOrderId);

                if (salesOrderDetail == null)
                {
                    return NotFound();
                }

                if (salesOrderDetail.SalesOrder.SalesOrderDetails.Count > 1)
                {
                    _context.SalesOrderDetails.Remove(salesOrderDetail);
                    var salesOrderId = salesOrderDetail.SalesOrderId;
                    await _context.SaveChangesAsync();

                    var salesOrderHeader = await _context.SalesOrderHeaders
                        .Include(s => s.SalesOrderDetails)
                        .Where(s => s.SalesOrderId == salesOrderId)
                        .FirstOrDefaultAsync();

                    var subTotal = salesOrderHeader.SalesOrderDetails.Sum(s => s.LineTotal);

                    salesOrderHeader.TaxAmt = Math.Round(subTotal * 0.22m, 2);
                    salesOrderHeader.Freight = Math.Round(subTotal * 0.05m, 2);

                    await _context.SaveChangesAsync();
                }
                else
                {
                    _context.SalesOrderHeaders.Remove(salesOrderDetail.SalesOrder);
                    await _context.SaveChangesAsync();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                LogManager.SaveLogBackend(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // PUT: Modifica un SalesOrderDetail in base all'Id
        [Authorize]
        [HttpPut("SalesOrderDetail/{salesOrderDetailId}")]
        public async Task<IActionResult> PutSalesOrderDetail(int salesOrderDetailId, [FromBody] SalesOrderDetail salesOrderDetail)
        {
            try
            {
                if (salesOrderDetailId != salesOrderDetail.SalesOrderDetailId)
                {
                    return BadRequest("The SalesOrderDetailId in the URL does not match the ID in the request body.");
                }

                var existingDetail = await _context.SalesOrderDetails
                    .FirstOrDefaultAsync(s => s.SalesOrderDetailId == salesOrderDetailId);

                if (existingDetail == null)
                {
                    return NotFound($"SalesOrderDetail with SalesOrderDetailId {salesOrderDetailId} not found.");
                }

                existingDetail.OrderQty = salesOrderDetail.OrderQty;
                existingDetail.ProductId = salesOrderDetail.ProductId;
                existingDetail.UnitPrice = salesOrderDetail.UnitPrice;
                existingDetail.UnitPriceDiscount = salesOrderDetail.UnitPriceDiscount;
                existingDetail.LineTotal = salesOrderDetail.LineTotal;
                existingDetail.Rowguid = salesOrderDetail.Rowguid;
                existingDetail.ModifiedDate = salesOrderDetail.ModifiedDate;
            
                await _context.SaveChangesAsync();

                var salesOrderHeader = await _context.SalesOrderHeaders
                        .Include(s => s.SalesOrderDetails)
                        .Where(s => s.SalesOrderId == salesOrderDetail.SalesOrderId)
                        .FirstOrDefaultAsync();

                var subTotal = salesOrderHeader.SalesOrderDetails.Sum(s => s.LineTotal);

                salesOrderHeader.TaxAmt = Math.Round(subTotal * 0.22m, 2);
                salesOrderHeader.Freight = Math.Round(subTotal * 0.05m, 2);

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                LogManager.SaveLogBackend(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // PUT: Modifica un SalesOrderHeader in base all'Id
        [Authorize]
        [HttpPut("{salesOrderHeaderId}")]
        public async Task<IActionResult> PutSalesOrderHeader(int salesOrderHeaderId, [FromBody] SalesOrderHeader salesOrderHeader)
        {
            try
            {
                if (salesOrderHeaderId != salesOrderHeader.SalesOrderId)
                {
                    return BadRequest("The SalesOrderHeaderId in the URL does not match the ID in the request body.");
                }

                var existingHeader = await _context.SalesOrderHeaders
                    .FirstOrDefaultAsync(s => s.SalesOrderId == salesOrderHeaderId);

                if (existingHeader == null)
                {
                    return NotFound($"SalesOrderHeader with SalesOrderId {salesOrderHeaderId} not found.");
                }

                existingHeader.RevisionNumber = salesOrderHeader.RevisionNumber;
                existingHeader.OrderDate = salesOrderHeader.OrderDate;
                existingHeader.DueDate = salesOrderHeader.DueDate;
                existingHeader.ShipDate = salesOrderHeader.ShipDate;
                existingHeader.Status = salesOrderHeader.Status;
                existingHeader.OnlineOrderFlag = salesOrderHeader.OnlineOrderFlag;
                existingHeader.SalesOrderNumber = salesOrderHeader.SalesOrderNumber;
                existingHeader.PurchaseOrderNumber = salesOrderHeader.PurchaseOrderNumber;
                existingHeader.AccountNumber = salesOrderHeader.AccountNumber;
                existingHeader.CustomerId = salesOrderHeader.CustomerId;
                existingHeader.ShipToAddressId = salesOrderHeader.ShipToAddressId;
                existingHeader.BillToAddressId = salesOrderHeader.BillToAddressId;
                existingHeader.ShipMethod = salesOrderHeader.ShipMethod;
                existingHeader.CreditCardApprovalCode = salesOrderHeader.CreditCardApprovalCode;
                existingHeader.SubTotal = salesOrderHeader.SubTotal;
                existingHeader.TaxAmt = salesOrderHeader.TaxAmt;
                existingHeader.Freight = salesOrderHeader.Freight;
                existingHeader.TotalDue = salesOrderHeader.TotalDue;
                existingHeader.Comment = salesOrderHeader.Comment;
                existingHeader.Rowguid = salesOrderHeader.Rowguid;
                existingHeader.ModifiedDate = salesOrderHeader.ModifiedDate;

            
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                LogManager.SaveLogBackend(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
