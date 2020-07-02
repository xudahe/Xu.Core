using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Collections.Generic;
using Xu.Common;
using Xu.Model;

namespace Xu.WebApi
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class VauleController : ControllerBase
    {
        private IMapper _mapper;
        private readonly IUser _user;

        public VauleController(IMapper mapper, IUser user)
        {
            _mapper = mapper;
            _user = user;
        }

        /// <summary>
        /// 通过 HttpContext 获取用户信息
        /// </summary>
        /// <param name="ClaimType">声明类型，默认 jti </param>
        /// <returns></returns>
        //[HttpGet]
        //public MessageModel<List<string>> GetUserInfo(string ClaimType = "jti")
        //{
        //    var getUserInfoByToken = _user.GetUserInfoFromToken(ClaimType);
        //    return new MessageModel<List<string>>()
        //    {
        //        Success = _user.IsAuthenticated(),
        //        Msg = _user.IsAuthenticated() ? _user.Name.ObjToString() : "未登录",
        //        Response = _user.GetClaimValueByType(ClaimType)
        //    };
        //}
    }
}