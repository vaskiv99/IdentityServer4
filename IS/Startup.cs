using System.Collections.Generic;
using Common;
using IdentityServer4;
using IdentityServer4.Services;
using IS.Data;
using IS.Models;
using IS.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using Swashbuckle.AspNetCore.Swagger;

namespace IS
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            LogManager.LoadConfiguration(ConfigLogger.ConnectionString);
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Identity Server 4" });

                var security = new Dictionary<string, IEnumerable<string>>
                {
                    {"Bearer", new string[] { }},
                };

                c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });
                c.AddSecurityRequirement(security);
            });
            services.AddCors(c => c.AddPolicy("Policy", builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            }));

            services.AddDbContext<ApplicationDbContext>(options =>
               options.UseSqlServer(Configuration.GetConnectionString("TestConnection")));

            services.AddTransient<UserService>();

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddInMemoryPersistedGrants()
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryApiResources(Config.GetApiResources())
                .AddInMemoryClients(Config.GetClients())
                .AddAspNetIdentity<ApplicationUser>();

            services.AddTransient<IProfileService, IdentityClaimsProfileService>();

            services.Configure<IdentityOptions>(config =>
            {
                config.Password.RequireDigit = true;
                config.Password.RequiredLength = 8;
                config.Password.RequireUppercase = false;
                config.Password.RequireNonAlphanumeric = false;
            });

            services.AddMvc();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
                {
                    options.Authority = "https://localhost:44304/";
                    options.Audience = "Api";
                    options.RequireHttpsMetadata = true;
                })
                .AddFacebook("MyFirstApp", options =>
                    {
                        options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                        options.AppId = "241602073332504";
                        options.AppSecret = "4aa87acdef15f0439e6ec9145e6cc9f3";
                        options.SaveTokens = true;
                    });
            services.AddSingleton<ILoggerManager, LoggerManager>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env,ILoggerFactory logger)
        {
            logger.AddConsole();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors("Policy");
            app.UseCors(c =>
            {
                c.AllowAnyHeader();
                c.AllowAnyMethod();
                c.AllowAnyOrigin();
            });
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
            app.UseIdentityServer();
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "areas",
                    template: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                );
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
