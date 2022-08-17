using Xu.Common;
using Xu.Common.HttpPolly;
using Xu.EventBus;
using Xu.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;
using Xu.Extensions.EventHandling;
using Xu.Model.ResultModel;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using StackExchange.Profiling;
using Xu.EnumHelper;
using Xu.IServices;

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

        private readonly IAspNetUser _aspNetUser;

        public ValueController(IHttpContextAccessor accessor, IHttpPollyHelper httpPollyHelper, IAspNetUser aspNetUser)
        {
            _accessor = accessor;
            // httpPolly
            _httpPollyHelper = httpPollyHelper;
            // 测试 Httpcontext
            _aspNetUser = aspNetUser;
        }

        /// <summary>
        /// 测试Redis消息队列
        /// </summary>
        /// <param name="_redisBasketRepository"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task RedisMq([FromServices] IRedisBasketRepository _redisBasketRepository)
        {
            var msg = $"这里是一条日志{DateTime.Now}";
            await _redisBasketRepository.ListLeftPushAsync(RedisMqKey.Loging, msg);
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
        /// 通过 HttpContext 获取用户信息
        /// </summary>
        /// <param name="ClaimType">声明类型，默认 jti </param>
        /// <returns></returns>
        [HttpGet]
        [Route("/api/values/UserInfo")]
        public MessageModel<List<string>> GetUserInfo(string ClaimType = "jti")
        {
            var getUserInfoByToken = _aspNetUser.GetUserInfoFromToken(ClaimType);
            return new MessageModel<List<string>>()
            {
                Success = _aspNetUser.IsAuthenticated(),
                Message = _aspNetUser.IsAuthenticated() ? _aspNetUser.Name.ObjToString() : "未登录",
                Response = _aspNetUser.GetClaimValueByType(ClaimType)
            };
        }

        /// <summary>
        /// 测试Fulent做参数校验
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<string> FluentVaTest([FromBody] UserRegisterVo param)
        {
            await Task.CompletedTask;
            return "Okay";
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
            return await _httpPollyHelper.PostAsync(HttpEnum.LocalHost, "/api/ElasticDemo/EsSearchTest", "{\"from\": 0,\"size\": 10,\"word\": \"非那雄安\"}");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<string> HttpPollyGet()
        {
            return await _httpPollyHelper.GetAsync(HttpEnum.LocalHost, "/api/ElasticDemo/GetDetailInfo?esid=3130&esindex=chinacodex");
        }

        #endregion HttpPolly

        #region Apollo 配置

        /// <summary>
        /// 测试接入Apollo获取配置信息
        /// </summary>
        [HttpGet("/apollo")]
        [AllowAnonymous]
        public async Task<IEnumerable<KeyValuePair<string, string>>> GetAllConfigByAppllo([FromServices] IConfiguration configuration)
        {
            return await Task.FromResult(configuration.AsEnumerable());
        }

        /// <summary>
        /// 通过此处的key格式为 xx:xx:x
        /// </summary>
        [HttpGet("/apollo/{key}")]
        [AllowAnonymous]
        public async Task<string> GetConfigByAppllo(string key)
        {
            return await Task.FromResult(Appsettings.App(key));
        }

        #endregion Apollo 配置
    }
}