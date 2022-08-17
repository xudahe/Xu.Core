﻿using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Xu.WebApi
{
    public class UseServiceDIAttribute : ActionFilterAttribute
    {
        protected readonly ILogger<UseServiceDIAttribute> _logger;

        //private readonly IBlogArticleServices _blogArticleServices;

        public UseServiceDIAttribute(ILogger<UseServiceDIAttribute> logger)
        {
            _logger = logger;
            //_blogArticleServices = blogArticleServices;
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            //var dd =await _blogArticleServices.Query();
            base.OnActionExecuted(context);
            DeleteSubscriptionFiles();
        }

        private void DeleteSubscriptionFiles()
        {
        }
    }
}