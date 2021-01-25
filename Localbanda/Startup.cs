using Localbanda.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;

namespace Localbanda
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
            services.AddControllers();
            services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v2.0.2",
                    Title = "LocalBanda API",
                    Description = "ASP.NET Core Web API",
                    TermsOfService = new Uri("https://nowgray.com/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "Nowgray.com",
                        Email = "askdev@nowgray.com",
                        Url = new Uri("https://nowgray.com"),
                    }

                });
            });
            var ConnectionString = GlobalConfig.ConnectionString;//Configuration.GetConnectionString("MyConnectionString"); 
            services.AddDistributedMemoryCache();
            //services.AddDistributedSqlServerCache(options =>
            //{
            //    options.ConnectionString =
            //        _config["DistCache_ConnectionString"];
            //    options.SchemaName = "dbo";
            //    options.TableName = "TestCache";
            //});


            services.AddApiVersioning(
                options =>
                {
                    options.ReportApiVersions = true;
                    options.DefaultApiVersion = new ApiVersion(0, 0);
                    options.AssumeDefaultVersionWhenUnspecified = true;
                    options.UseApiBehavior = false;
                });
            services.AddDbContext<Models.LocalbandaDbContext>
              (options => options.UseSqlServer(ConnectionString));
            services.Configure<ApiBehaviorOptions>(options =>
            {

                options.SuppressModelStateInvalidFilter = true;
            });
            services.AddRouting(r => r.SuppressCheckForUnhandledSecurityMetadata = true);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors("MyPolicy");
            app.Use((context, next) =>
            {
                context.Items["__CorsMiddlewareInvoked"] = true;
                return next();
            });
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();
            app.UseSwaggerUI(x =>
            {
                x.SwaggerEndpoint("/swagger/v1/swagger.json", "LocalBanda v2.0.2");
                x.RoutePrefix = string.Empty;
            });
            //app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseHttpsRedirection();

        }

    }
}
