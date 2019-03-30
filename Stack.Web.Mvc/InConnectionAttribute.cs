using Microsoft.AspNetCore.Mvc.Filters;
using Stack.Data;

namespace Stack.Web.Mvc
{
    public class InConnectionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            Database.Connect();

            base.OnActionExecuting(context);
        }
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);

            Database.Disconnect();
        }
    }
}
