using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ErrorHandlerMiddlewareMock
{
    public partial class CustomExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public CustomExceptionMiddleware(RequestDelegate next) 
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpcontext)
        {
            try
            {
                await _next(httpcontext);
            }
            catch (Exception ex)
            {
                await HandlerCustomExceptionAsync(httpcontext, ex);
            }
        }

        private Task HandlerCustomExceptionAsync(HttpContext context, Exception exception)
        {
            // set the type of content response body 
            context.Response.ContentType = "application/json";

            // check type errors to set a Body Header Status Code
            if (exception.Message.Contains("Forbiden"))
            {
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            }
            else if (exception.Message.Contains("Not Found"))
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            // fill out the Body Response properties according to scope
            return context.Response.WriteAsync(new ExceptionDetail
                    {
                        status = "error",
                        error = exception.Message
                    }.ToString());
        }
    }
}
