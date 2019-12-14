using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using Autofac;
using AutoMapper;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using SteamAchievements.Data;
using SteamAchievements.Services;
using SteamAchievements.Services.Models;
using SteamAchievements.Web.Spa.Settings;

namespace SteamAchievements.Web.Spa
{
    internal class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(typeof(SteamAchievementsProfile).Assembly);

            services.AddDbContext<SteamContext>(options =>
                                                {
                                                    options.UseLazyLoadingProxies()
                                                           .ConfigureWarnings(w => w.Ignore(new EventId(30000)))
                                                           .EnableSensitiveDataLogging()
                                                           .UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                                                                         o =>
                                                                             o.MigrationsAssembly(typeof(
                                                                                                      SteamContext
                                                                                                  )
                                                                                                 .Assembly
                                                                                                 .FullName));
                                                });

            services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddEntityFrameworkStores<SteamContext>();

            services.AddControllers()
                     // use Newtonsoft Json instead of the new .NET Json serializer
                    .AddNewtonsoftJson(options =>
                                           options.SerializerSettings.ContractResolver =
                                               new CamelCasePropertyNamesContractResolver());

            services.AddScoped<DbContext, SteamContext>();

            services.AddOptions();

            // Register the Swagger generator, defining 1 or more Swagger documents
            // https://docs.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-2.1
            services.AddSwaggerGen(c =>
                                   {
                                       var assemblyName = Assembly.GetExecutingAssembly().GetName();
                                       c.SwaggerDoc("v1",
                                                    new OpenApiInfo
                                                    {Title = "Steam Achievements Web API", Version = "v" + assemblyName.Version});

                                       // Set the comments path for the Swagger JSON and UI.
                                       var xmlFile = $"{assemblyName.Name}.xml";
                                       var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                                       c.IncludeXmlComments(xmlPath);

                                       //Note: The warning messages for these obsolete methods are a lie - it won't work without them
                                       c.DescribeAllEnumsAsStrings();
                                       c.DescribeStringEnumsInCamelCase();
                                   });

            services.AddLocalization(options => options.ResourcesPath = "Resources");

            ConfigureOauth2(services);

            services.AddSpaStaticFiles(config => { config.RootPath = "app/build"; });
        }

        private void ConfigureOauth2(IServiceCollection services)
        {
            // see https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity-api-authorization?view=aspnetcore-3.1

            var apiSettingsSection = Configuration.GetSection(nameof(ApiSettings));
            var apiSettings = new ApiSettings();
            apiSettingsSection.Bind(apiSettings);

            services.AddIdentityServer()
                    .AddApiAuthorization<User, SteamContext>(options =>
                                                             {
                                                                 options.Clients.Add(new Client
                                                                                     {
                                                                                         ClientId = apiSettings.ClientId,
                                                                                         ClientSecrets =
                                                                                             new List<Secret>
                                                                                             {
                                                                                                 new
                                                                                                     Secret(apiSettings.ClientSecret
                                                                                                               .Sha256())
                                                                                             },
                                                                                         AllowedGrantTypes = GrantTypes.ClientCredentials,
                                                                                         AllowedScopes = new List<string>
                                                                                                         {
                                                                                                             "openid", "profile"
                                                                                                         },
                                                                                         RequireConsent = false,
                                                                                         RequireClientSecret = true
                                                                                     });
                                                             });

            services.AddAuthentication()
                    .AddIdentityServerJwt();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            // Register your own things directly with Autofac, like:
            builder.RegisterAssemblyTypes(GetType().Assembly, typeof(SteamContext).Assembly,
                                          typeof(IAchievementService).Assembly).AsImplementedInterfaces();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var supportedCultures = new[]
                                    {
                                        new CultureInfo("en-US"),
                                        new CultureInfo("fr"),
                                        new CultureInfo("pl")
                                    };

            app.UseRequestLocalization(new RequestLocalizationOptions
                                       {
                                           DefaultRequestCulture = new RequestCulture("en-US"),
                                           // Formatting numbers, dates, etc.
                                           SupportedCultures = supportedCultures,
                                           // UI strings that we have localized.
                                           SupportedUICultures = supportedCultures
                                       });
            // allow / to route to /wwwroot/index.html
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseIdentityServer();
            app.UseAuthorization();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1"); });

            app.UseEndpoints(config => { config.MapControllers(); });

            app.UseSpa(config => { config.Options.SourcePath = "app"; });
        }
    }
}
