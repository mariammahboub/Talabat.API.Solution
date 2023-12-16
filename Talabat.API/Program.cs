using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Talabat.API.Errors;
using Talabat.API.Extensions;
using Talabat.API.Helpers;
using Talabat.API.Middlewares;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Services.Contract;
using Talabat.Repository;
using Talabat.Repository.Data;
using Talabat.Repository.Identity;
using Talabat.Services;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Talabat.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var WebApplicationBuilder = WebApplication.CreateBuilder(args);


            #region Configure Services

            // Add services to the container.
            WebApplicationBuilder.Services.AddControllers();

            WebApplicationBuilder.Services.AddSwaggerServices(); // My Own Extension Method To Allow DI for Swagger Services => To Make Much Readable

            #region Allow DI For DbContext
            WebApplicationBuilder.Services.AddDbContext<StoreDbContext>(Options =>
               {
                   Options.UseSqlServer(WebApplicationBuilder.Configuration.GetConnectionString("DefaultConnection"));
               });

            WebApplicationBuilder.Services.AddDbContext<AppIdentityDbContext>(Options =>
            {
                Options.UseSqlServer(WebApplicationBuilder.Configuration.GetConnectionString("IdentityConnection"));
            });
            #endregion

            #region Allow DI For redis
            WebApplicationBuilder.Services.AddSingleton<IConnectionMultiplexer>(S =>
            {
                var connection = WebApplicationBuilder.Configuration.GetConnectionString("Redis");

                var configurationOptions = ConfigurationOptions.Parse(connection);
                configurationOptions.AbortOnConnectFail = false; // Set the necessary configuration option

                return ConnectionMultiplexer.Connect(configurationOptions);
            });

            #endregion

            WebApplicationBuilder.Services.AddApplicationServices(); // My Own Extension Method To Allow DI for Application Services => To Make Much Readable
            WebApplicationBuilder.Services.AddIdentityServices(WebApplicationBuilder.Configuration);    // My Own Extension Method To Allow DI for Identity Services => To Make Much Readable
            WebApplicationBuilder.Services.AddCors(options =>
            {
                options.AddPolicy("MyPolicy", options =>
                {
                    options.AllowAnyHeader().AllowAnyMethod().WithOrigins(WebApplicationBuilder.Configuration["FrontBaseUrl"]);
                });
            });
            #endregion

            var app = WebApplicationBuilder.Build();

            #region Update-Databas Automatic
            
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var _dbcontext = services.GetRequiredService<StoreDbContext>(); //ask clr for create obj from dbcontext
            var _IdentityDbContext = services.GetRequiredService<AppIdentityDbContext>(); //ask clr for create obj from _IdentityDbContext
            var _loggerFactory = services.GetRequiredService<ILoggerFactory>();
            try
            {
                await _dbcontext.Database.MigrateAsync();             //Update Database
                await StoreContextSeed.SeedAsync(_dbcontext);         //Data Seed

                await _IdentityDbContext.Database.MigrateAsync();      //Update Database
                var userManager = services.GetRequiredService<UserManager<AppUser>>();
                await AppIdentityDbContextSeed.SeedUserAsync(userManager);         //Data Seed
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger<Program>();
                logger.LogError(ex, "An error Occured During Apply The Migration");
            }

            #endregion

            #region Configure Kestrel Middleware

            #region Exception Error Handler [Server error "back end"]
            app.UseMiddleware<ExceptionMiddleware>(); 
            #endregion

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwaggerMiddleware();
            }
            #region Not found End Point Handler

            app.UseStatusCodePagesWithReExecute("/errors/{0}"); 

            #endregion

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCors("MyPolicy");
            app.MapControllers(); 
            app.UseAuthentication();
            app.UseAuthorization();
            #endregion

            app.Run();
        }
    }
}