using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Xu.Common;
using Xu.Common.HttpContextUser;
using Xu.IServices;

namespace Xu.Extensions
{
    /// <summary>
    /// 定义响应处理器
    /// </summary>
    public class ApiResponseHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IAspNetUser _aspNetUser;

        public ApiResponseHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IAspNetUser aspNetUser) : base(options, logger, encoder, clock)
        {
            _aspNetUser = aspNetUser;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            throw new NotImplementedException();
        }

        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.ContentType = "application/json";
            Response.StatusCode = StatusCodes.Status401Unauthorized;
            await Response.WriteAsync(JsonConvert.SerializeObject((new ApiResponse(StatusCode.CODE401)).MessageModel));
        }

        protected override async Task HandleForbiddenAsync(AuthenticationProperties properties)
        {
            Response.ContentType = "application/json";
            if (_aspNetUser.MessageModel != null)
            {
                Response.StatusCode = _aspNetUser.MessageModel.Status;
                await Response.WriteAsync(JsonConvert.SerializeObject(_aspNetUser.MessageModel));
            }
            else
            {
                Response.StatusCode = StatusCodes.Status403Forbidden;
                await Response.WriteAsync(JsonConvert.SerializeObject((new ApiResponse(StatusCode.CODE403)).MessageModel));
            }
        }
    }
}