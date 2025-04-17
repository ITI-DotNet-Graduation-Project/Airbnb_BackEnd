
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
using Airbnb.Infrastructure.UnitOfWorks;
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



            builder.Services.AddControllers();

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
                o.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        if (!string.IsNullOrEmpty(context.Token))
                        {

                            var cleanToken = new string(context.Token
                                .Where(c => !char.IsWhiteSpace(c) && !char.IsControl(c))
                                .ToArray());

                            Console.WriteLine($"Original token: '{context.Token}'");
                            Console.WriteLine($"Cleaned token: '{cleanToken}'");
                            Console.WriteLine($"Original length: {context.Token.Length}");
                            Console.WriteLine($"Cleaned length: {cleanToken.Length}");

                            context.Token = cleanToken;
                        }
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine($"Auth failed: {context.Exception}");
                        return Task.CompletedTask;
                    }
                };
            });
            builder.Services.AddAuthorization();

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
            //builder.Services.AddControllers()
            //    .AddJsonOptions(options =>
            //    {
            //        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
            //    });

            /////
            builder.Services.AddScoped<IPropertyService, PropertyService>();
            builder.Services.AddScoped(typeof(IGenericRepo<>), typeof(GenericRepo<>)); // This fixes the error
            builder.Services.AddScoped<IPropertyService, PropertyService>();
            builder.Services.AddScoped<IPropertyRepo, PropertyRepo>();
            builder.Services.AddScoped<IPropertyImageRepo, PropertyImageRepo>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IReviewRepo, ReviewRepo>();
            builder.Services.AddScoped<IUserRepo, UserRepo>();
            builder.Services.AddScoped<IBookingRepo, BookingRepo>();
            builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

            builder.Services.AddScoped<IPropertyImageService, PropertyImageService>();
            builder.Services.AddScoped<IPropertyCategoryService, PropertyCategoryService>();
            builder.Services.AddScoped<IUserService, UserService>();

            builder.Services.AddScoped<IReviewService, ReviewService>();

            builder.Services.AddScoped<IBookingService, BookingService>();

            // if using AutoMapper
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
            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                await DefaultRoles.SeedRolesAsync(roleManager);
                await DefaultUser.SeedAdminUserAsync(userManager);
            }
            app.UseCors("AllowAll");

            app.Use(async (context, next) =>
            {
                if (context.Request.Headers.TryGetValue("Authorization", out var authHeader))
                {
                    var rawHeader = authHeader.ToString();
                    Console.WriteLine($"Raw Authorization Header: '{rawHeader}'");
                    Console.WriteLine($"Header Length: {rawHeader.Length}");


                    var headerBytes = Encoding.UTF8.GetBytes(rawHeader);
                    Console.WriteLine($"Hex: {BitConverter.ToString(headerBytes)}");


                    var spaceCount = rawHeader.Count(c => c == ' ');
                    Console.WriteLine($"Space characters in header: {spaceCount}");
                }
                await next();
            });
            app.UseRouting();
            app.Use(async (context, next) =>
            {
                Console.WriteLine($"Request path: {context.Request.Path}");
                Console.WriteLine($"Auth header exists: {context.Request.Headers.ContainsKey("Authorization")}");
                await next();
            });
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
