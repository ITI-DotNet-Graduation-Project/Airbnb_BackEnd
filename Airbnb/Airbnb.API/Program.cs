using Airbnb.Application;
using Airbnb.Application.Map;
using Airbnb.Application.Services.Abstract;
using Airbnb.Application.Services.Implementation;
using Airbnb.Application.Settings;
using Airbnb.DATA.models;
using Airbnb.DATA.models.Identity;
using Airbnb.Infrastructure.Abstract;
using Airbnb.Infrastructure.Context;
using Airbnb.Infrastructure.Repos;
using Airbnb.Infrastructure.Repos.Abstract;
using Airbnb.Infrastructure.Repos.Implementation;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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

            // Swagger configuration
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Add database context
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DB"));
            });

            // Hangfire
            builder.Services.AddHangfire(x =>
                x.UseSqlServerStorage(builder.Configuration.GetConnectionString("DB")));
            builder.Services.AddHangfireServer();

            // Identity
            builder.Services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            // JWT Settings
            builder.Services.Configure<JwtSetting>(builder.Configuration.GetSection(nameof(JwtSetting)));
            builder.Services.AddOptions<JwtSetting>().BindConfiguration(nameof(JwtSetting)).ValidateDataAnnotations();
            var settings = builder.Configuration.GetSection(nameof(JwtSetting)).Get<JwtSetting>();

            // Authentication with JWT
            builder.Services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                o.SaveToken = true;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key)),
                    ValidateIssuer = true,
                    ValidIssuer = settings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = settings.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    NameClaimType = "sub",
                    RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
                };
            });

            builder.Services.AddAuthorization();

            // Configure Identity Options
            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 8;
                options.SignIn.RequireConfirmedEmail = true;
                options.User.RequireUniqueEmail = true;
            });

            // Email settings
            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection(nameof(EmailSettings)));
            builder.Services.AddHttpContextAccessor();

            // Rate Limiting
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
                });
            });

            // Dependency Injection for Services and Repositories
            builder.Services.AddScoped(typeof(IGenericRepo<>), typeof(GenericRepo<>));
            builder.Services.AddScoped<IPropertyService, PropertyService>();
            builder.Services.AddScoped<IPropertyRepo, PropertyRepo>();
            builder.Services.AddScoped<IPropertyImageRepo, PropertyImageRepo>();
            builder.Services.AddScoped<IPropertyImageService, PropertyImageService>();
            builder.Services.AddScoped<IPropertyCategoryService, PropertyCategoryService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IReviewService, ReviewService>();
            builder.Services.AddScoped<IBookingService, BookingService>();

            // Payment-specific services and repositories
            builder.Services.AddScoped<PaymentRepo>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();

            // AutoMapper registration
            builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

            // CORS policy
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseStaticFiles();
            app.UseHttpsRedirection();

            // Seed roles and admin user
            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                await DefaultRoles.SeedRolesAsync(roleManager);
                await DefaultUser.SeedAdminUserAsync(userManager);
            }

            app.UseCors("AllowAll");

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
