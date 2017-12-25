using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LandonApi.Filters;
using LandonApi.Infrastructure;
using LandonApi.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;

namespace LandonApi
{
    public class Startup
    {
        private readonly int? _httpsPort;
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;

            //var builder = new ConfigurationBuilder()
            //    .SetBasePath(env.ContentRootPath)
            //    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            //    .AddJsonFile($"appsettings.{ env.EnvironmentName}.json", optional: true)
            //    .AddEnvironmentVariables();
            //Configuration = builder.Build();

            //Get the HTTPS port (only in development)
            if (env.IsDevelopment())
            {
                var launchJsonConfig = new ConfigurationBuilder()
                    .SetBasePath(env.ContentRootPath)
                    .AddJsonFile("Properties\\launchSettings.json")
                    .Build();
                _httpsPort = launchJsonConfig.GetValue<int>("iisSettings:iisExpress:sslPort");

            }

        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Use an in-memory dataase for quick dev and testing
            //TODO: Swap out a real dataabse in production
            services.AddDbContext<HotelApiContext>(opt=>opt.UseInMemoryDatabase());

            services.AddMvc(opt=>
            {
                opt.Filters.Add(typeof(JsonExceptionFilter));

                //Require HTTPS for all controllers
                opt.SslPort = _httpsPort;

                opt.Filters.Add(typeof(RequireHttpsAttribute));
            });

            services.AddRouting(opt => opt.LowercaseUrls = true);
            services.AddApiVersioning(opt =>
            {
                opt.ApiVersionReader = new MediaTypeApiVersionReader();
                opt.AssumeDefaultVersionWhenUnspecified = true;
                opt.ReportApiVersions = true;
                opt.DefaultApiVersion=new ApiVersion(1, 0);
                opt.ApiVersionSelector = new CurrentImplementationApiVersionSelector(opt);
            });
            services.Configure<HotelInfo>(Configuration.GetSection("Info"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
               // app.UseDeveloperExceptionPage();
                var context = app.ApplicationServices.GetRequiredService<HotelApiContext>();
                AddTestData(context);

            }

            app.UseHsts(opt =>
            {
                opt.MaxAge(days: 180);
                opt.IncludeSubdomains();
                opt.Preload();
            });
        


            app.UseMvc();
        }
        private static void AddTestData(HotelApiContext context)
        {
            context.Rooms.Add(new RoomEntity
            {
                Id = Guid.Parse("301df04d-8679-4b1b-1b92-0a586ae53d08"),
                Name = "Oxford Suite",
                Rate = 10119,
            });
            //context.Rooms.Add(new RoomEntity
            //{
            //    Id = Guid.Parse("301df04d-8679-4b1b-1b92-0a586ae53d10"),
            //    Name = "Picadilly Suite",
            //    Rate = 10119,
            //});

            context.SaveChanges();
        }
    }
}
