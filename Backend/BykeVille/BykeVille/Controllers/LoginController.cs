using BykeVille.AuthModels;
using BykeVille.BLogic;
using BykeVille.Models;
using BykeVille.NewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BykeVille.Controllers
{

    [Route("[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {

        private JwtSettings jwtSettings;
        private readonly DbBikeVilleOldContext _contextDbBikeVilleOld;
        private readonly DbBikeVilleNewContext _contextDbBikeVilleNew;
        public CredentialsDbManager credentialsDbManager;

        public LoginController(JwtSettings jwtSettings, DbBikeVilleOldContext contextDbBikeVilleOld, DbBikeVilleNewContext contextDbBikeVilleNew)
        {
            _contextDbBikeVilleOld = contextDbBikeVilleOld;
            _contextDbBikeVilleNew = contextDbBikeVilleNew;
            this.jwtSettings = jwtSettings;

            var connectionStringDbOld = _contextDbBikeVilleOld.Database.GetDbConnection().ConnectionString;
            var connectionStringDbNew = _contextDbBikeVilleNew.Database.GetDbConnection().ConnectionString;
            credentialsDbManager = new CredentialsDbManager(connectionStringDbOld, connectionStringDbNew);
        }

        // POST: Controlla le credenziali per effettuare il login e restituisce il token in caso di successo
        [HttpPost("CheckCredentials")]
        public IActionResult Post([FromBody] Credentials credentials)
        {
            try
            {
                if (credentialsDbManager.CheckCredentials(new Credentials(credentials.Email, credentials.Password)))
                {
                    var token = GenerateJwtToken(credentials.Email);
                    return Ok(new { token });
                }
                else
                    return Unauthorized();
            }
            catch (Exception ex)
            {
                LogManager.SaveLogBackend(ex);
                return StatusCode(500, ex.Message);
            }
        }

        //Controlla se la password inserita dall'utente è corretta
        [HttpPost("CheckPassword")]
        public Boolean CheckPassword([FromBody] Credentials credentials)
        {
            try
            {
                return credentialsDbManager.CheckPassword(new Credentials(credentials.Email, credentials.Password));
            }
            catch (Exception ex)
            {
                LogManager.SaveLogBackend(ex);
                return false;
            }
        }

        //Genera il token JWT
        private string GenerateJwtToken(string username)
        {
            try
            {
                var secretKey = jwtSettings.SecretKey;
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(secretKey);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                  new Claim (ClaimTypes.Name, username)
                }),
                    Expires = DateTime.UtcNow.AddMinutes(jwtSettings.TokenExpirationMinutes),
                    Issuer = jwtSettings.Issuer,
                    Audience = jwtSettings.Audience,
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                string tokenString = tokenHandler.WriteToken(token);
                return tokenString;
            }
            catch (Exception ex)
            {
                LogManager.SaveLogBackend(ex);
                return null;
            }
        }
    }
}
