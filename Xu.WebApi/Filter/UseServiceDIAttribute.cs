using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Xu.WebApi
{
    /// <summary>
    /// 测试注入，【暂时无用】
    /// </summary>
    public class UseServiceDIAttribute : ActionFilterAttribute
    {
        //private readonly IBlogArticleServices _blogArticleServices;

        public UseServiceDIAttribute(ILogger<UseServiceDIAttribute> logger)
        {
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