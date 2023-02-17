using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Profiling;
using Xu.Common;
using Xu.Model.ResultModel;

namespace Xu.WebApi.Controllers
{
    /// <summary>
    /// 公共接口
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PublicController : ControllerBase
    {
        private readonly IHttpContextAccessor _accessor;


        public PublicController(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        /// <summary>
        /// 生成PEM格式的公钥和密钥
        /// </summary>
        ///  <param name="strength">长度</param>
        /// <returns>Item1:公钥；Item2:私钥；</returns>
        [HttpGet]
        public object GetRSACryption(int strength = 1024)
        {
            return new MessageModel<string, string>()
            {
                Message = "生成成功",
                Success = true,
                Response = RSACryption.CreateKeyPair(strength)
            };
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
    }
}