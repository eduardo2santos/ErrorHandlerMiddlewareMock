using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;

namespace ErrorHandlerMiddlewareMock
{
    public static class ConfigureExceptionCustomMiddleware
    {
        public static void ConfigureExceptionCustomHandler(this IApplicationBuilder app) {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                   // set the type of content response body 
                   context.Response.ContentType = "application/json";

                    // catch the exception features
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    // check it's null to throw the response body according to scope
                    if (contextFeature != null)
                    {
                        // check type errors to set a Body Header Status Code
                        if (contextFeature.Error.Message.Contains("Forbiden")) 
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        } 
                        else if (contextFeature.Error.Message.Contains("Not Found"))
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                        } 
                        else 
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        }

                        // fill out the Body Response properties according to scope
                        await context.Response.WriteAsync(new ExceptionDetail
                        {
                            status = "error",
                            error = contextFeature.Error.Message
                        }.ToString());
                    }
                });
            });
        }
        public static void ConfigureExceptionCustomHandler2(this IApplicationBuilder app)
        {
            app.UseMiddleware<CustomExceptionMiddleware>();
        }
    }
}
