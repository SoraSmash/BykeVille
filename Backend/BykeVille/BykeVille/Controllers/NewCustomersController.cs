using BykeVille.BLogic;
using BykeVille.Models;
using BykeVille.NewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;

namespace BykeVille.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class NewCustomersController : ControllerBase
    {
        private readonly DbBikeVilleNewContext _contextNew;
        private readonly DbBikeVilleOldContext _contextOld;

        public NewCustomersController(DbBikeVilleNewContext contextNew, DbBikeVilleOldContext contextOld)
        {
            _contextNew = contextNew;
            _contextOld = contextOld;
        }


        // GET: Ottiene tutti i NewCustomer
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NewCustomer>>> GetNewCustomers()
        {
            try
            {
                return await _contextNew.NewCustomers.ToListAsync();
            }
            catch (Exception ex)
            {
                LogManager.SaveLogBackend(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: Ottiene un NewCustomer in base all'email
        [HttpGet("UserByEmail")]
        public async Task<ActionResult<IEnumerable<NewCustomer>>> GetNewCustomers(string emailAddress)
        {
            try
            {
                var customer = await _contextNew.NewCustomers
                    .Include(c => c.NewCustomerAddresses)
                    .FirstOrDefaultAsync(c => c.EmailAddress == emailAddress);

                return Ok(customer);
            }
            catch (Exception ex)
            {
                LogManager.SaveLogBackend(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: Ottiene tutti gli indirizzi di un NewCustomer in base all'email
        [Authorize]
        [HttpGet("AddressesByEmail")]
        public async Task<ActionResult<IEnumerable<NewAddress>>> GetNewAddresses(string emailAddress)
        {
            try
            {
                var customer = await _contextNew.NewCustomers
                    .Include(c => c.NewCustomerAddresses)
                    .FirstOrDefaultAsync(c => c.EmailAddress == emailAddress);

                if(customer == null)
                {
                    return NotFound();
                }

                var customerWithAddresses = _contextNew.NewCustomerAddresses
                    .Include(ca => ca.Customer)
                    .Include(ca => ca.Address)
                    .Where(ca => ca.Customer.EmailAddress == emailAddress)
                    .ToList();

                var addresses = customerWithAddresses
                    .Select(ca => ca.Address)
                    .ToList();

                return Ok(addresses);
            }
            catch (Exception ex)
            {
                LogManager.SaveLogBackend(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        //Genera una stringa casuale di lunghezza specificata e caratteri specificati
        [NonAction]
        public string GenerateRandomString(int length, string characters)
        {
            Random random = new Random();
            string result = "";

            try
            {
                for (int i = 0; i < length; i++)
                {
                    result = result + characters[random.Next(characters.Length)];
                }

                return result;
            }
            catch (Exception ex)
            {
                LogManager.SaveLogBackend(ex);
                return null;
            }
        }

        // POST: Aggiunge un nuovo indirizzo per un cliente specifico
        [Authorize]
        [HttpPost("AddAddress/{customerId}")]
        public async Task<ActionResult<NewAddress>> PostNewAddress(int customerId, NewAddress newAddress)
        {
            string characters = "ABCDEF0123456789";

            try
            {
                Address oldAddress = new Address(
                    0,
                    Faker.Address.StreetAddress(),
                    "",
                    Faker.Address.City(),
                    Faker.Address.Country(),
                    "United States",
                    Faker.Address.ZipCode(),
                    Guid.Parse($"{GenerateRandomString(8, characters)}-{GenerateRandomString(4, characters)}-{GenerateRandomString(4, characters)}-{GenerateRandomString(4, characters)}-{GenerateRandomString(12, characters)}"),
                    DateTime.Now.Date
                    );

                _contextOld.Add(oldAddress);

                await _contextOld.SaveChangesAsync();

                CustomerAddress oldCustomerAddress = new CustomerAddress(
                    customerId,
                    oldAddress.AddressId,
                    "Main Office",
                    Guid.Parse($"{GenerateRandomString(8, characters)}-{GenerateRandomString(4, characters)}-{GenerateRandomString(4, characters)}-{GenerateRandomString(4, characters)}-{GenerateRandomString(12, characters)}"),
                    DateTime.Now.Date
                    );

                _contextOld.Add(oldCustomerAddress);

                await _contextOld.SaveChangesAsync();

                newAddress.AddressId = 0;
                newAddress.Rowguid = Guid.Parse($"{GenerateRandomString(8, characters)}-{GenerateRandomString(4, characters)}-{GenerateRandomString(4, characters)}-{GenerateRandomString(4, characters)}-{GenerateRandomString(12, characters)}");
                newAddress.ModifiedDate = DateTime.Now.Date;
                newAddress.AddressOldId = oldAddress.AddressId;

                _contextNew.Add(newAddress);

                await _contextNew.SaveChangesAsync();

                NewCustomerAddress newCustomerAddress = new NewCustomerAddress(
                    customerId,
                    newAddress.AddressOldId,
                    "Main Office",
                    Guid.Parse($"{GenerateRandomString(8, characters)}-{GenerateRandomString(4, characters)}-{GenerateRandomString(4, characters)}-{GenerateRandomString(4, characters)}-{GenerateRandomString(12, characters)}"),
                    DateTime.Now.Date,
                    customerId,
                    newAddress.AddressOldId
                    );

                _contextNew.Add(newCustomerAddress);

                await _contextNew.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                LogManager.SaveLogBackend(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // DELETE: Elimina un indirizzo in base all'addressOldId
        [Authorize]
        [HttpDelete("DeleteAddress/{addressOldId}")]
        public async Task<ActionResult<NewAddress>> DeleteNewAddress(int addressOldId)
        {
            try
            {
                var newAddress = await _contextNew.NewAddresses.FindAsync(addressOldId);
                if (newAddress == null)
                {
                    return NotFound();
                }
                var newCustomerAddress = await _contextNew.NewCustomerAddresses
                    .FirstOrDefaultAsync(ca => ca.AddressOldId == addressOldId);
                if (newCustomerAddress == null)
                {
                    return NotFound();
                }
                _contextNew.NewCustomerAddresses.Remove(newCustomerAddress);
                await _contextNew.SaveChangesAsync();
                _contextNew.NewAddresses.Remove(newAddress);
                await _contextNew.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                LogManager.SaveLogBackend(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: Ottiene un NewCustomer in base all'ID
        [HttpGet("{id}")]
        public async Task<ActionResult<NewCustomer>> GetNewCustomer(int id)
        {
            try
            {
                var newCustomer = await _contextNew.NewCustomers.FindAsync(id);

                if (newCustomer == null)
                {
                    return NotFound();
                }

                return newCustomer;
            }
            catch (Exception ex)
            {
                LogManager.SaveLogBackend(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // PUT: Modifica un NewCustomer in base all'ID
        [HttpPut("{id}")]
        public async Task<IActionResult> PutNewCustomer(int id, NewCustomer newCustomer)
        {
            try
            {
                if (id != newCustomer.CustomerOldId)

                {
                    Console.WriteLine($"ID nella rotta: {id}, CustomerOldId nel corpo: {newCustomer.CustomerOldId}");

                    return BadRequest();
                }

                var existingCustomer = await _contextNew.NewCustomers
                    .FirstOrDefaultAsync(c => c.CustomerOldId == id);

                if (existingCustomer == null)
                {
                    return NotFound();
                }

                existingCustomer.NameStyle = newCustomer.NameStyle;
                existingCustomer.Title = newCustomer.Title;
                existingCustomer.FirstName = newCustomer.FirstName;
                existingCustomer.MiddleName = newCustomer.MiddleName;
                existingCustomer.LastName = newCustomer.LastName;
                existingCustomer.Suffix = newCustomer.Suffix;
                existingCustomer.CompanyName = newCustomer.CompanyName;
                existingCustomer.SalesPerson = newCustomer.SalesPerson;
                existingCustomer.EmailAddress = newCustomer.EmailAddress;
                existingCustomer.Phone = newCustomer.Phone;
                if (!newCustomer.PasswordHash.IsNullOrEmpty())
                {
                    var saltedPassword = Encoding.UTF8.GetBytes(existingCustomer.PasswordSalt + newCustomer.PasswordHash);
                    using (var sha256 = SHA256.Create())
                    {
                        var hashBytes = sha256.ComputeHash(saltedPassword);
                        existingCustomer.PasswordHash = Convert.ToBase64String(hashBytes);
                    }
                }
                existingCustomer.ModifiedDate = newCustomer.ModifiedDate;
                existingCustomer.Role = newCustomer.Role;
                existingCustomer.CustomerOldId = newCustomer.CustomerOldId;
            }
            catch (Exception ex)
            {
                LogManager.SaveLogBackend(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

            try
            {
                await _contextNew.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NewCustomerExists(id))
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


        // POST: Aggiunge un nuovo NewCustomer
        [HttpPost]
        public async Task<ActionResult<NewCustomer>> PostNewCustomer(NewCustomer newCustomer)
        {
            try
            {
                _contextNew.NewCustomers.Add(newCustomer);
                await _contextNew.SaveChangesAsync();

                return CreatedAtAction("GetNewCustomer", new { id = newCustomer.CustomerId }, newCustomer);
            }
            catch (Exception ex)
            {
                LogManager.SaveLogBackend(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // DELETE: Elimina un NewCustomer in base all'ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNewCustomer(int id)
        {
            try
            {
                var newCustomer = await _contextNew.NewCustomers.FindAsync(id);
                if (newCustomer == null)
                {
                    return NotFound();
                }

                _contextNew.NewCustomers.Remove(newCustomer);
                await _contextNew.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                LogManager.SaveLogBackend(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // Verifica se un NewCustomer esiste in base all'ID
        private bool NewCustomerExists(int id)
        {
            try
            {
                return _contextNew.NewCustomers.Any(e => e.CustomerId == id);
            }
            catch (Exception ex)
            {
                LogManager.SaveLogBackend(ex);
                return false;
            }
        }
    }
}
