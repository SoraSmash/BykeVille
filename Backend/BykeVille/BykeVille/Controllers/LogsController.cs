using BykeVille.BLogic;
using BykeVille.NewModels;
using Microsoft.AspNetCore.Mvc;

namespace BykeVille.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class LogsController : ControllerBase
    {

        public LogsController()
        { }

        // GET: Ritorna i log del backend per una data specifica
        [HttpGet("Backend")]
        public List<LogBackend> GetLogsBackend(DateOnly date)
        {
            try
            {
                return LogManager.LoadLogBackend(date);
            }
            catch (Exception ex)
            {
                LogManager.SaveLogBackend(ex);
                return null;
            }
        }

        // GET: Ritorna i log del frontend per una data specifica
        [HttpGet("Frontend")]
        public List<LogFrontend> GetLogsFrontend(DateOnly date)
        {
            try
            {
                return LogManager.LoadLogFrontend(date);
            }
            catch (Exception ex)
            {
                LogManager.SaveLogBackend(ex);
                return null;
            }
        }

        // POST: Salva i dettagli dell'eccezione nel frontend nel database
        [HttpPost]
        public void PostLogFrontend(string exception)
        {
            try
            {
                LogManager.SaveLogFrontend(exception);
            }
            catch (Exception ex)
            {
                LogManager.SaveLogBackend(ex);
            }
        }

    }
}
