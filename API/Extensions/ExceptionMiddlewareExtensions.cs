using API.Models;
using Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace API.Extensions
{
    public static class ExceptionMiddlewareExtensions
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app, ILogger logger)
        {
            app.UseExceptionHandler(
                appError =>
                {
                    appError.Run(async context =>
                    {
                        var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                        context.Response.StatusCode = (int)GetErrorCode(contextFeature.Error);
                        context.Response.ContentType = "application/json";
                        if (contextFeature != null)
                        {
                            logger.LogError($"Something went wrong: {contextFeature.Error}");
                            await context.Response.WriteAsync(new ErrorDetails
                            {
                                StatusCode = context.Response.StatusCode,
                                ExceptionMessage = GetErrorMessage(contextFeature.Error, context.Response.StatusCode)
                            }.ToString());
                        }
                    });
                });
        }
        private static HttpStatusCode GetErrorCode(Exception e)
        {
            switch (e)
            {
                case ValidationException _:
                    return HttpStatusCode.BadRequest;
                case AuthenticationException _:
                    return HttpStatusCode.Forbidden;
                case NotImplementedException _:
                    return HttpStatusCode.NotImplemented;
                case KnownException _:
                    return HttpStatusCode.BadRequest;
                default:
                    return HttpStatusCode.InternalServerError;
            }
        }
        private static string GetErrorMessage(Exception e, int statusCode)
        {
            switch (statusCode)
            {
                //case (int)HttpStatusCode.InternalServerError:
                case 0:
                    return "An error occurred, please try again later or contact the administrator.";
                default:
                    return e.Message;
            }
        }
    }
}
