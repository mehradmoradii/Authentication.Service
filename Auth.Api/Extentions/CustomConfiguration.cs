using Auth.Api.Extentions.MinimalApi;
using Auth.Api.Extentions.Services.EmailSenderService;
using Auth.Api.Middlewares;
using Auth.Api.MinimalApis.AppGroups;
using Auth.Command.Commands.AppGroups;
using Auth.Command.Handler;
using Auth.Domain.AppRoles.Aggregate;
using Auth.Domain.AppUsers.Aggregate;
using Auth.Infrastracture.Repository;
using Auth.Infrastracture.TokenServices;
using Auth.Infrastracture.TokenServices.AccessTokenServices;
using Auth.Infrastracture.TokenServices.RefreshTokenServices;
using Auth.Infrastracture.Validation;
using Auth.Query.Handler;
using Auth.Query.Handler.MapperProfile.AppGroups;
using Auth.Query.Queries.AppGroups;
using Auth.Repository.Base;
using Auth.Repository.Context;
using Auth.Repository.Mapping.AppGroups;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using StackExchange.Redis;
using System.Text;

namespace Auth.Api.Extentions
{
    public static class CustomConfiguration
    {
        public static void AddCustomConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddLogging();
            services.AddCors(opt =>
             opt.AddPolicy("AllowAngular", policy =>
             {
                 policy.WithOrigins("http://localhost:4200")
                       .AllowAnyHeader()
                       .AllowAnyMethod()
                       .AllowCredentials();

                 opt.AddPolicy(name:
                                     "CorsPolicy", b =>
                                    b.AllowAnyHeader()
                                    .AllowAnyOrigin()
                                    .AllowAnyMethod());
             }));
    
            services.AddDbContext<ApplicationDbcontext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                
                b => b.MigrationsAssembly("Auth.Repository")));

            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.TokenValidationParameters = new()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["AuthSetting:validIssuer"],
                        ValidAudience = configuration["AuthSetting:validAudience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(configuration["AuthSetting:secretKey"])
                        )
                    };
                });
            //services.AddAuthorization();
            services.AddAuthorization(options =>
            {
                options.AddPolicy("TriggerAuth", policy =>
                {
                    policy.RequireAssertion(_ => true); // Force running auth handler
                });
            });
            services.AddIdentity<AppUser, AppRole>(options =>
            {
                // Password Setting
                options.Password.RequiredLength = 6;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;

                // User Setting
                options.User.RequireUniqueEmail = true;

                // Lockout setting
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
                options.Lockout.MaxFailedAccessAttempts = 5;
            })
            .AddEntityFrameworkStores<ApplicationDbcontext>()
            .AddDefaultTokenProviders();




            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(2);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

           

            services.AddAutoMapper(cfg => { }, typeof(AppGroupMapperProfile).Assembly);

           

            services.InstallCommandsAndCommandValidators();
            services.InstallCommandHandlers();
            services.InstallQueryHandlers();



            //LIFETTIME
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSingleton<IConnectionMultiplexer>(sp =>
                        ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis")));
            
            services.AddScoped<AuthorizationMiddleware>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAccessTokenService, AccessTokenService>();
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            services.AddScoped<IRefreshTokenStore, RedisRefreshTokenStore>();



            services.AddTransient(typeof(IPipeLineBehavior<,>), typeof(CommandValidatorBehavior<,>));
            services.AddTransient(typeof(IPipeLineBehavior<,>), typeof(QueryValidatorBehavior<,>));
            services.AddTransient<IEmailSenderService, EmailSender>();
  

           
            services.AddScoped<DbContext,ApplicationDbcontext>();
            services.AddScoped(typeof(AppGroupMinimalApi));
            services.AddScoped(typeof(IRepositoryBase<,>), typeof(RepositoryBase<,>));
            //services.AddScoped<ExceptionHandlerMiddleware>();

            services.AddSwaggerGen();
        }
    }
}
