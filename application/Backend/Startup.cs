using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Prometheus;

namespace Backend
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo App API", Version = "v1" });
            });
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAnyOrigin", builder => builder.AllowAnyOrigin());
            });
            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new CorsAuthorizationFilterFactory("AllowAnyOrigin"));
            });
            services.AddHealthChecks();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMetricServer();
            app.UseHttpMetrics();

            // we need a custom exception handler to record 500 status code due to unhandled exceptions. Without this 
            // middleware, asp.net will handle the exception -> 500 conversion somewhere even higher in the stack, before we
            // have a chance to inject our middleware for Prometheus
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "text/html";

                    var exceptionHandlerPathFeature =
                        context.Features.Get<IExceptionHandlerPathFeature>();

                    await context.Response.WriteAsync("<html lang=\"en\"><body>\r\n");
                    await context.Response.WriteAsync($"Unhandled exception was thrown: <br>{exceptionHandlerPathFeature?.Error}<br>\r\n");
                    await context.Response.WriteAsync("</body></html>\r\n");
                    await context.Response.WriteAsync(new string(' ', 512)); // IE padding
                });
            });

            if (!env.IsDevelopment())
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseMvc();

            app.UseHttpsRedirection();
            app.UseCors("AllowAnyOrigin");

            // In real world scenarios ready and live should not be the same
            app.UseHealthChecks("/health/ready");
            app.UseHealthChecks("/health/live");
        }
    }
}
