using System;
using System.Net;
using System.Net.Http.Json;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

using Serilog;
using CoffeeMachine.Web.Exceptions.CustomExceptions;

namespace CoffeeMachine.Web.Exceptions
{
    /// <summary>
    /// custom middleware for exceptions that occur during application execution
    /// </summary>
    public class GlobalExceptionHandler : IMiddleware
    {
        private readonly IWebHostEnvironment _env;

        public GlobalExceptionHandler(IWebHostEnvironment env)
        {
            _env = env;
        }

        /// <summary>
        /// call next middleware or handle exception
        /// </summary>
        /// <param name="context">request of client</param>
        /// <param name="next">ref of next middleware</param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next.Invoke(context);
            }
            catch (NullCashboxException nullCashboxEx)
            {
                if (_env.IsDevelopment())
                {
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    Log.Error("In cashbox of coffee machine not enough money");
                    Log.Error(nullCashboxEx.ToString());
                    await context.Response.WriteAsJsonAsync($"{nullCashboxEx}");
                }
                else
                {
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    var result = "In cashbox of coffee machine not enough money!";
                    Log.Error(nullCashboxEx.Message);
                    await context.Response.WriteAsJsonAsync(result);
                }
            }
            catch (NullReferenceException nullEx)
            {
                if (_env.IsDevelopment())
                {
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    Log.Error("Nof found object");
                    Log.Error(nullEx.ToString());
                    await context.Response.WriteAsJsonAsync(nullEx.ToString());
                }
                else
                {
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    var result = "Not found! 404";
                    Log.Error(nullEx.Message);
                    await context.Response.WriteAsJsonAsync(result);
                }
            }
            catch (Exception ex)
            {
                if (_env.IsDevelopment())
                {
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    Log.Error(ex.ToString());
                    await context.Response.WriteAsJsonAsync(ex.ToString());
                }
                else
                {
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    var result = "Unknown error";
                    Log.Error(ex.Message);
                    await context.Response.WriteAsJsonAsync(result);
                }
            }
        }
    }
}