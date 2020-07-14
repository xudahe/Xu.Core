using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;

namespace Xu.WebApi
{
    public class UseServiceDIAttribute : ActionFilterAttribute
    {
        protected readonly ILogger<UseServiceDIAttribute> _logger;
        private readonly string _name;

        public UseServiceDIAttribute(ILogger<UseServiceDIAttribute> logger, string Name = "")
        {
            _logger = logger;
            _name = Name;
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);
            DeleteSubscriptionFiles();
        }

        private void DeleteSubscriptionFiles()
        {
            try
            {
                // ...
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error Delete Subscription Files");
            }
        }
    }
}