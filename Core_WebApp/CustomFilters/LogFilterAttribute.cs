using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Core_WebApp.CustomFilters
{

    public class LogFilterAttribute : ActionFilterAttribute
    {
        private void LogAction(RouteData route, string status)
        {
            var controllerName = route.Values["controller"].ToString();
            var actionName = route.Values["action"].ToString();

            var log = $"Current status is {status} " +
                $"for action method {actionName}" +
                $" in contriller {controllerName}";
            Debug.WriteLine(log);
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            LogAction(context.RouteData, "OnActionExecuting");
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            LogAction(context.RouteData, "OnActionExecuted");
        }
     
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            LogAction(context.RouteData, "OnResultExecuting");
        }
        public override void OnResultExecuted(ResultExecutedContext context)
        {
            LogAction(context.RouteData, "OnResultExecuted");
        }

    }
}
