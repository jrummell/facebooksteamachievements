using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using Autofac;
using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SteamAchievements.Data;
using SteamAchievements.Services;
using SteamAchievements.Services.Models;
using SteamAchievements.Web.Spa.Settings;
using JsonWebKey = Microsoft.IdentityModel.Tokens.JsonWebKey;

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
                                                    {Title = "Liftoff API", Version = "v" + assemblyName.Version});

                                       // Set the comments path for the Swagger JSON and UI.
                                       var xmlFile = $"{assemblyName.Name}.xml";
                                       var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                                       c.IncludeXmlComments(xmlPath);

                                       //Note: The warning messages for these obsolete methods are a lie - it won't work without them
                                       c.DescribeAllEnumsAsStrings();
                                       c.DescribeStringEnumsInCamelCase();
                                   });

            var mapper = new ModelMapCreator();
            mapper.CreateMappings();

            services.AddLocalization(options => options.ResourcesPath = "Resources");

            ConfigureOauth2(services);
        }

        private void ConfigureOauth2(IServiceCollection services)
        {
            // OAuth 2
            var apiSettingsSection = Configuration.GetSection(nameof(ApiSettings));
            var apiSettings = new ApiSettings();
            apiSettingsSection.Bind(apiSettings);

            SecureString password = new NetworkCredential("", apiSettings.CertificatePassword).SecurePassword;
            var certificate = new X509Certificate2(File.ReadAllBytes(apiSettings.CertificatePath), password);

            services.AddIdentityServer()
                    .AddInMemoryIdentityResources(new[] {new IdentityResources.OpenId()})
                     // http://docs.identityserver.io/en/latest/topics/add_apis.html
                    .AddInMemoryApiResources(new[] {new ApiResource(IdentityServerConstants.LocalApi.ScopeName)})
                    .AddInMemoryClients(new[]
                                        {
                                            new Client
                                            {
                                                ClientId = apiSettings.ClientId,
                                                AllowedGrantTypes = GrantTypes.ClientCredentials,
                                                ClientSecrets = new List<Secret>
                                                                {
                                                                    new Secret(apiSettings.ClientSecret
                                                                                          .Sha256())
                                                                },
                                                AllowedScopes = {IdentityServerConstants.LocalApi.ScopeName}
                                            }
                                        })
                     // http://docs.identityserver.io/en/latest/topics/startup.html#refstartupkeymaterial
                    //.AddValidationKey(new SecurityKeyInfo
                    //                  {
                    //                      Key =
                    //                          new JsonWebKey(JsonConvert
                    //                                            .SerializeObject(new
                    //                                                             {
                    //                                                                 key
                    //                                                                     = Guid
                    //                                                                        .NewGuid()
                    //                                                             })),
                    //                      SigningAlgorithm = "RS256"
                    //                  });
            .AddSigningCredential(new SigningCredentials(new X509SecurityKey(certificate), "RS256"));

            services.AddLocalApiAuthentication();
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

            app.UseStaticFiles();
            app.UseDefaultFiles();

            app.UseHttpsRedirection();

            app.UseRouting();
            
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseIdentityServer();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1"); });

            app.UseEndpoints(config => { config.MapControllers(); });
        }
    }
}
