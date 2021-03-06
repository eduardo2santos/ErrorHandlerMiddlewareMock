using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

            /*=================================================================================*/
            // Option 0: treat any error occurring outside the app, for ex. calling not exists page
            app.Use(async (context, next) =>
            {
                await next();

                // check the response Statuscode is different from 2xx nor 3xx range 
                if ((!context.Response.StatusCode.ToString().StartsWith("2") && !context.Response.StatusCode.ToString().StartsWith("3")) && !context.Response.HasStarted)
                {
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync("{" + $"status:\"{context.Response.StatusCode.ToString()}\",error:\"{Enum.GetName(typeof(HttpStatusCode), context.Response.StatusCode)}\"" +"}");
                    await next();
                }
            });

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
                    await context.Response.WriteAsync("{" + $"status:\"{context.Response.StatusCode.ToString()}\",error:\"{exceptionHandlerPathFeature?.Error.Message}\"" + "}");
                });
            });

            //// Option 2: call the custom exception handler method
            //app.ConfigureExceptionCustomHandler();
            //// Option 3: call the custom exception handler method injected
            //app.ConfigureExceptionCustomHandler2();
            /*========================================================================================*/
            
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
