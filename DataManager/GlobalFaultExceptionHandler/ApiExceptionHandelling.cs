using Helpers;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Filters;

namespace DataManager.GlobalFaultExceptionHandler
{
    public class ApiExceptionHandelling : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            if (context.Exception is KnownException)
            {
                throw getError((context.Exception as KnownException).Message,
                             "Known Exception",
                             HttpStatusCode.BadRequest);
            }

            throw getError("An error occurred, please try again later or contact the administrator.",
                            "Critical Exception",
                            HttpStatusCode.InternalServerError);
        }
        public static HttpResponseException getError(string message, string phrase, HttpStatusCode code)
        {
            HttpResponseException ex = new HttpResponseException(new HttpResponseMessage(code)
            {
                Content = new StringContent(getErrorJSON(message)),
                //Content = new StringContent(message),
                ReasonPhrase = phrase
            });
            ex.Response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return ex;
        }
        public static string getErrorJSON(string msg)
        {
            return "{ \"ExceptionMessage\":\"" + msg + "\" }";
        }

    }
}