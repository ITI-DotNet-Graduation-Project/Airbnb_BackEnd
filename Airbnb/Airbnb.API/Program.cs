
using Airbnb.Application;
using Airbnb.Application.Settings;
using Airbnb.DATA.models;
using Airbnb.DATA.models.Identity;
using Airbnb.Infrastructure.Context;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using School.Application.SeedRoles;
using System.Text;
using System.Threading.RateLimiting;

namespace Airbnb.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DB"));
            });
            builder.Services.addAppModule();

            builder.Services.AddHangfire(x =>
            x.UseSqlServerStorage(builder.Configuration.GetConnectionString("DB")));

            builder.Services.AddHangfireServer();

            builder.Services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.Configure<JwtSetting>(builder.Configuration.GetSection(nameof(JwtSetting)));
            builder.Services.AddOptions<JwtSetting>().BindConfiguration(nameof(JwtSetting)).ValidateDataAnnotations();
            var settings = builder.Configuration.GetSection(nameof(JwtSetting)).Get<JwtSetting>();


            builder.Services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }
            )
            .AddJwtBearer(o =>
            {
                o.SaveToken = true;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings!.Key)),
                    ValidIssuer = settings.Issuer,
                    ValidAudience = settings.Audience,
                };
            });

            builder.Services.Configure<IdentityOptions>(options =>
            {

                options.Password.RequiredLength = 8;
                options.SignIn.RequireConfirmedEmail = true;
                options.User.RequireUniqueEmail = true;

            });


            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection(nameof(EmailSettings)));
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddRateLimiter(RateLimiterOption =>
            {
                RateLimiterOption.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                RateLimiterOption.AddTokenBucketLimiter("Token", option =>
                {
                    option.TokenLimit = 2;
                    option.QueueLimit = 1;
                    option.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    option.TokensPerPeriod = 2;
                    option.ReplenishmentPeriod = TimeSpan.FromSeconds(30);
                    option.AutoReplenishment = true;
                }
                );
            });


            builder.Services.AddCors();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                await DefaultRoles.SeedRolesAsync(roleManager);
                await DefaultUser.SeedAdminUserAsync(userManager);
            }
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
