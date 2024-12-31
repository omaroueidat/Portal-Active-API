using Application.Activities;
using Application.Core;
using Application.Interfaces;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastructure.Email;
using Infrastructure.Photos;
using Infrastructure.Secuirity;
using MailerSendNetCore.Common.Extensions;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, 
            IConfiguration config)
        {

            // Add services to the container
            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen();

            services.AddDbContext<DataContext>(opt =>
            {
                opt.UseSqlServer(config.GetConnectionString("DefaultConnection"));
            });

            services.AddCors(opt =>
            {
                opt.AddPolicy("CorsPolicy", policy =>
                {
                    policy
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials()
                        .WithExposedHeaders("WWW-Authenticate", "Pagination")
                        .WithOrigins("http://localhost:3000", "https://localhost:3000");
                });
            });

            // Register the MediatR thsat takes the assembly that contains the Handler and the query that we defined
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(List.Handler).Assembly));

            services.AddAutoMapper(typeof(MappingProfiles).Assembly);

            // Adding the validation services to the DI container
            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<CreateActivity>();

            // Adding the service for the HttpContext Accessor
            services.AddHttpContextAccessor();

            // Add the services for the UserAccessor
            services.AddScoped<IUserAccessor, UserAccessor>();

            // Add the configiration to the class from the config file
            services.Configure<CloudinarySettings>(config.GetSection("Cloudinary"));

            // Add the services of the Photo Upload
            services.AddScoped<IPhotoAccessor, PhotoAccessor>();

            // Add the service for SignalR
            services.AddSignalR();

            // Add the services for Mailer Send
            services.AddMailerSendEmailClient(config.GetSection("MailerSend"));

            // Add the EmailSender Service
            services.AddScoped<EmailSender>();

            return services;
        }
    }
}
