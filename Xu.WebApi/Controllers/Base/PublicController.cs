﻿using Microsoft.AspNetCore.Mvc;
using Xu.Common;
using Xu.Model.ResultModel;

namespace Xu.WebApi.Controllers
{
    /// <summary>
    /// 公共方法
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PublicController : ControllerBase
    {
        public PublicController()
        {
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
    }
}