using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Profiling;
using System.Threading.Tasks;
using Xu.Common;
using Xu.Common.HttpPolly;
using Xu.EnumHelper;
using Xu.EventBus;
using Xu.Extensions.EventHandling;

namespace Xu.WebApi.Controllers
{
    /// <summary>
    /// 用户管理
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Permissions.Name)]
    public class ValueController : ControllerBase
    {
        private readonly IHttpContextAccessor _accessor;

        private readonly IHttpPollyHelper _httpPollyHelper;

        public ValueController(IHttpContextAccessor accessor, IHttpPollyHelper httpPollyHelper)
        {
            _accessor = accessor;
            // httpPolly
            _httpPollyHelper = httpPollyHelper;
        }

        /// <summary>
        /// 测试RabbitMQ事件总线
        /// </summary>
        /// <param name="_eventBus"></param>
        /// <param name="blogId"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public void EventBusTry([FromServices] IEventBus _eventBus, int blogId = 1)
        {
            var piblishEvent = new ProductPriceChangedIntegrationEvent(blogId);//声明事件源
            _eventBus.Publish(piblishEvent);//发布事件
        }

        /// <summary>
        /// 获取MiniProfiler的html代码片段
        /// </summary>
        /// <remarks>将生成的内容拷贝出来粘贴在Swagger的index.html顶部</remarks>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public string GetHtmlMiniProfiler()
        {
            var html = MiniProfiler.Current.RenderIncludes(_accessor.HttpContext);

            return html.Value;
        }

        #region HttpPolly

        [HttpPost]
        [AllowAnonymous]
        public async Task<string> HttpPollyPost()
        {
            var response = await _httpPollyHelper.PostAsync(HttpEnum.LocalHost, "/api/ElasticDemo/EsSearchTest", "{\"from\": 0,\"size\": 10,\"word\": \"非那雄安\"}");

            return response;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<string> HttpPollyGet()
        {
            return await _httpPollyHelper.GetAsync(HttpEnum.LocalHost, "/api/ElasticDemo/GetDetailInfo?esid=3130&esindex=chinacodex");
        }

        #endregion HttpPolly
    }
}