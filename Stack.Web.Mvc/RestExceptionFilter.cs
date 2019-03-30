using System;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Stack.Data;
using Stack.Logging;

namespace Stack.Web.Mvc
{
    public class RestExceptionFilter : IExceptionFilter
    {
        public RestExceptionFilter(IHostingEnvironment env, ILogger logger)
        {
            this.env = env;
            this.logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            logger.Log(context.Exception);

            if (env.IsDevelopment())
            {
                context.Result = new ObjectResult(new DeveloperExceptionResult(
                                        context.Exception.Message,
                                        context.Exception.StackTrace));
            }

            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
            if (context.Exception is ArgumentException || context.Exception is BusinessLogicException)
            {
                statusCode = HttpStatusCode.BadRequest;
            }
            else if (context.Exception is EntityNotFoundException)
            {
                statusCode = HttpStatusCode.NotFound;
            }
            else if (context.Exception is EntityNotUniqueException)
            {
                statusCode = HttpStatusCode.Conflict;
            }

            context.HttpContext.Response.StatusCode = (int)statusCode;
            context.ExceptionHandled = true;
        }

        #region Private members
        private IHostingEnvironment env;
        private ILogger logger;
        #endregion
    }
}
