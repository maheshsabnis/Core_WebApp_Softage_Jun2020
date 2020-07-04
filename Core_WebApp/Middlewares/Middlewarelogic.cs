using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core_WebApp.Middlewares
{
    public class ErrorInformation
    {
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }
    /// <summary>
    /// The class containing Middleware logic
    /// This must be constructor injected with RequestDelegate
    /// This class will have an asyn method of name InvokeAsync()
    /// with input parameter as HttpContext. This method will contains
    /// the logic
    /// </summary>
    public class ExceptionMiddlewareLogic
    {
        private readonly RequestDelegate request;
        public ExceptionMiddlewareLogic(RequestDelegate request)
        {
            this.request = request;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                // if not exception occures then proceed to next middleware in pipeline
                await request(httpContext);
            }
            catch (Exception ex)
            {
                // handle exception and generate response
                await HandleErrorAsync(httpContext, ex);
            }
        }

        private async Task HandleErrorAsync(HttpContext ctx, Exception ex)
        {
            // set the response code
            ctx.Response.StatusCode = 500; // internal server error
            // read the exception message
            string message = ex.Message;
            // define the error object
            var errorObject = new ErrorInformation()
            {
                 ErrorCode = ctx.Response.StatusCode,
                 ErrorMessage = message
            };

            // serialize this object in JSON format
            string ResponseJSONMessage = System.Text.Json.JsonSerializer.Serialize(errorObject);
            // Write the Http Response
            await ctx.Response.WriteAsync(ResponseJSONMessage);
        }
    }

    /// <summary>
    /// Class containing Extension method to register the Custom Middleware
    /// In Http Request Pipeline
    /// </summary>
    public static class CustomMiddlewareRegistration
    {
        public static void CustomExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddlewareLogic>();
        }
    }
}
