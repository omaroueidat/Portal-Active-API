using API.Services;
using Domain;
using Persistence;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Infrastructure.Secuirity;
using Microsoft.AspNetCore.Authorization;

namespace API.Extensions
{
    public static class IdentityServiceExtension
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddIdentityCore<AppUser>(opt =>
            {
                opt.Password.RequireNonAlphanumeric = true;
                opt.User.RequireUniqueEmail = true;     // Enable Unique Email
                

            })
            .AddEntityFrameworkStores<DataContext>();

            // Key of the Jwt Issuer
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"]));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opt =>
                {
                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = key,
                        ValidateIssuer = false,
                        ValidateAudience = false,
                    };
                });

            // Add Authorization to the API
            services.AddAuthorization(opt =>
            {
                // Options of the Authorization where we will add the handler

                // Add a policy to the Authorization
                opt.AddPolicy("IsActivityHost", policy =>
                {
                    // Link this policy to our class
                    policy.Requirements.Add(new IsHostRequirement());
                });
            });

            // Adding the Authorization Handler class as a service that will last 
            // as lifetime of the method
            services.AddTransient<IAuthorizationHandler, IsHostRequirementHandler>();

            // Add the service for the token
            services.AddScoped<TokenService>();

            return services;
        }
    }
}
