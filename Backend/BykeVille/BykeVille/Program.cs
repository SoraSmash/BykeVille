
using BykeVille.AuthModels;
using BykeVille.BLogic;
using BykeVille.Models;
using BykeVille.NewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace BykeVille
{

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //CORS Policy
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowLocalhost", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            //Opzione per Swagger per semplificare l'inserimento di una data di tipo DateOnly
            builder.Services.AddSwaggerGen(options => {
                options.MapType<DateOnly>(() => new OpenApiSchema
                {
                    Type = "string",
                    Format = "date"
                });
            });

            //Aggiunta dei DbContext
            builder.Services.AddDbContext<DbBikeVilleOldContext>(
                options => options.UseSqlServer(builder.Configuration.GetConnectionString("OldSqlConnection")));

            builder.Services.AddDbContext<DbBikeVilleNewContext>(
                options => options.UseSqlServer(builder.Configuration.GetConnectionString("NewSqlConnection")));

            //Aggiunta di NewtonSoftJson per evitare i loop infiniti
            builder.Services.AddControllersWithViews().AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            //Aggiunta delle impostazioni JWT
            JwtSettings jwtSettings = new();
            builder.Configuration.GetSection("JwtSettings").Bind(jwtSettings);
            _ = builder.Services.AddSingleton(jwtSettings);

            //Aggiunta dell'autenticazione JWT
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
              .AddJwtBearer(opts =>
              {
                  opts.TokenValidationParameters = new TokenValidationParameters
                  {
                      ValidateIssuer = true,
                      ValidateAudience = true,
                      ValidateLifetime = true,
                      ValidateIssuerSigningKey = true,
                      ValidIssuer = jwtSettings.Issuer,
                      ValidAudience = jwtSettings.Audience,
                      RequireExpirationTime = true,
                      IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                      ClockSkew = TimeSpan.Zero
                  };
              });

            //Inizializzazione della connessione al database per il LogManager
            LogManager.InitConnection(builder.Configuration["NewConnectionStrings:NewSqlConnection"]);

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors("AllowLocalhost");

            app.MapControllers();

            app.Run();
        }
    }
}
