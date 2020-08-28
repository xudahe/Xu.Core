using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xu.Common;
using Xu.Extensions;
using Xu.IServices;

namespace Xu.WebApi.Controllers
{
    /// <summary>
    /// 登录管理
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    [Authorize(Permissions.Name)]
    public class LoginController : Controller
    {
        private readonly IUserSvc _userSvc;
        private readonly IRoleSvc _roleSvc;
        private readonly PermissionRequirement _requirement;

        /// <summary>
        /// 构造函数注入
        /// </summary>
        /// <param name="userSvc"></param>
        /// <param name="roleSvc"></param>
        /// <param name="requirement"></param>
        public LoginController(IUserSvc userSvc, IRoleSvc roleSvc, PermissionRequirement requirement)
        {
            _userSvc = userSvc;
            _roleSvc = roleSvc;
            _requirement = requirement;
        }

        /// <summary>
        /// 获取JWT的方法（登录接口）
        /// </summary>
        /// <param name="name">登录名</param>
        /// <param name="pass">登录密码</param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<object> GetJwtToken(string name = "", string pass = "")
        {
            string jwtStr = string.Empty;

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(pass))
            {
                return new JsonResult(new
                {
                    Status = false,
                    message = "用户名或密码不能为空"
                });
            }

            if(RSACryption.IsBase64(pass))  pass = RSACryption.RSADecrypt(pass);

            var user = (await _userSvc.Query(d => d.Enabled == false && d.LoginName == name && d.LoginPwd == pass)).FirstOrDefault();
            if (user != null)
            {
                var userRoles = await _roleSvc.QueryByIds(user.RoleIds.Split(","));
                //如果是基于用户的授权策略，这里要添加用户;如果是基于角色的授权策略，这里要添加角色
                var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, name),
                    new Claim(JwtRegisteredClaimNames.Jti, user.Id.ToString()),
                    new Claim(ClaimTypes.Expiration, DateTime.Now.AddSeconds(_requirement.Expiration.TotalSeconds).ToString()) };
                claims.AddRange(userRoles.Select(s => new Claim(ClaimTypes.Role, s.RoleName)));

                //var data = await _roleModulePermissionServices.RoleModuleMaps();
                //var list = (from item in data
                //            where item.IsDeleted == false
                //            orderby item.Id
                //            select new PermissionItem
                //            {
                //                Url = item.Module?.LinkUrl,
                //                Role = item.Role?.Name,
                //            }).ToList();

                //_requirement.Permissions = list;

                _requirement.Permissions = (from item in userRoles
                                            orderby item.Id
                                            select new PermissionItem
                                            {
                                                Url = "",
                                                Role = item?.RoleName
                                            }).ToList();

                //用户标识
                var identity = new ClaimsIdentity(JwtBearerDefaults.AuthenticationScheme);
                identity.AddClaims(claims);

                var token = JwtToken.BuildJwtToken(claims.ToArray(), _requirement);
                return new JsonResult(token);
            }
            else
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "认证失败"
                });
            }
        }

        /// <summary>
        /// 请求刷新Token（以旧换新）
        /// </summary>
        /// <param name="token">token</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<object> RefreshToken(string token = "")
        {
            string jwtStr = string.Empty;

            if (string.IsNullOrEmpty(token))
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "token无效，请重新登录！"
                });
            }
            var tokenModel = JwtHelper.SerializeJwt(token);
            if (tokenModel != null && tokenModel.Id > 0)
            {
                var user = await _userSvc.QueryById(tokenModel.Id);
                if (user != null)
                {
                    var userRoles = await _roleSvc.QueryByIds(user.RoleIds.Split(","));
                    //如果是基于用户的授权策略，这里要添加用户;如果是基于角色的授权策略，这里要添加角色
                    var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, user.LoginName),
                    new Claim(JwtRegisteredClaimNames.Jti, tokenModel.Id.ToString()),
                    new Claim(ClaimTypes.Expiration, DateTime.Now.AddSeconds(_requirement.Expiration.TotalSeconds).ToString()) };
                    claims.AddRange(userRoles.Select(s => new Claim(ClaimTypes.Role, s.RoleName)));

                    //用户标识
                    var identity = new ClaimsIdentity(JwtBearerDefaults.AuthenticationScheme);
                    identity.AddClaims(claims);

                    var refreshToken = JwtToken.BuildJwtToken(claims.ToArray(), _requirement);
                    return new JsonResult(refreshToken);
                }
            }

            return new JsonResult(new
            {
                success = false,
                message = "认证失败"
            });
        }
    }
}