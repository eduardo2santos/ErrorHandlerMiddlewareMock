using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ErrorHandlerMiddlewareMock;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ErrorHandlerMiddlewareMock
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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            /*=======================================*/
            // Option 1: call the UseExceptionHandler with custom return
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    context.Response.ContentType = "application/json";
                    
                    // Use exceptionHandlerPathFeature to get the type of error into the exception
                    var exceptionHandlerPathFeature =
                        context.Features.Get<IExceptionHandlerPathFeature>();
                    
                    // return to client according to scope
                    await context.Response.WriteAsync($"status:\"error\",error:\"{exceptionHandlerPathFeature?.Error.Message}\"");
                });
            });

            //// Option 2: call the custom exception handler method
            app.ConfigureExceptionCustomHandler();
            //// Option 3: call the custom exception handler method injected
            app.ConfigureExceptionCustomHandler2();
            /*=======================================*/
            
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
