using Airbnb.Application.Services.Abstract;
using Airbnb.Application.Services.Implementation;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Airbnb.Application
{
    public static class AppModule
    {

        public static IServiceCollection addAppModule(this IServiceCollection service)
        {
            service.AddScoped<IEmailSender, EmailSender>();
            service.AddScoped<IAuthService, AuthService>();
            service.AddScoped<IJwtProvider, JwtProvider>();

            return service;

        }
    }
}
