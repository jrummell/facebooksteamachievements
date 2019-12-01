using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using Autofac;
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

namespace SteamAchievements.Web.Spa
{
    public class Startup
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

            app.UseAuthorization();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1"); });

            app.UseRouting();

            app.UseEndpoints(config => { config.MapControllers(); });
        }
    }
}
