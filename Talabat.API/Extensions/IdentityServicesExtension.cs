using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using System.Runtime.CompilerServices;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Services.Contract;
using Talabat.Repository.Identity;
using Microsoft.IdentityModel.Tokens;
using Talabat.Services;
using System.Text;

namespace Talabat.API.Extensions
{
    public static class IdentityServicesExtension
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services , IConfiguration configuration)
        {
            #region Allow DI For Identity Services
            services.AddScoped(typeof(IAuthService), typeof(AuthService));
            services.AddIdentity<AppUser, IdentityRole>(Options =>
            {
                // Configuration --> ex Password
            }).AddEntityFrameworkStores<AppIdentityDbContext>();
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateAudience = true,
                    ValidAudience = configuration["JWT:ValidAudience"],
                    ValidateIssuer = true,
                    ValidIssuer = configuration["JWT:ValidIssuer"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:SecretKey"])),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromDays(double.Parse(configuration["JWT:DurationInDays"]))
                };
            }).AddJwtBearer("Bearer02", options =>
            {

            }).AddCookie("xxx", options =>
            {

            });
            #endregion
            return services;
        }
    }
}

