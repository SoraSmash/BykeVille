using BykeVille.BLogic;
using BykeVille.Models;
using BykeVille.NewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BykeVille.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SignInController : ControllerBase
    {

        private readonly DbBikeVilleOldContext _contextDbBikeVilleOld;
        private readonly DbBikeVilleNewContext _contextDbBikeVilleNew;
        public CredentialsDbManager credentialsDbManager;

        public SignInController(DbBikeVilleOldContext contextDbBikeVilleOld, DbBikeVilleNewContext contextDbBikeVilleNew)
        {
            _contextDbBikeVilleOld = contextDbBikeVilleOld;
            _contextDbBikeVilleNew = contextDbBikeVilleNew;

            var connectionStringDbOld = _contextDbBikeVilleOld.Database.GetDbConnection().ConnectionString;
            var connectionStringDbNew = _contextDbBikeVilleNew.Database.GetDbConnection().ConnectionString;
            credentialsDbManager = new CredentialsDbManager(connectionStringDbOld, connectionStringDbNew);
        }

        // POST: Aggiunge un nuovo cliente nel database
        [HttpPost]
        public System.Boolean Post([FromBody] Customer customer)
        {
            try
            {
                return credentialsDbManager.AddCustomer(customer);
            }
            catch (Exception ex)
            {
                LogManager.SaveLogBackend(ex);
                return false;
            }
        }
    }
}
