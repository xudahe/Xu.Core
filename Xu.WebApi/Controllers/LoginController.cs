using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Xu.Common;
using Xu.Extensions;
using Xu.IServices;
using Xu.Model.Models;
using Xu.Model.ResultModel;
using Xu.Model.ViewModels;

namespace Xu.WebApi.Controllers
{
    /// <summary>
    /// 登录管理
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    [Authorize(Permissions.Name)]
    public class LoginController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUserSvc _userSvc;
        private readonly IRoleSvc _roleSvc;
        private readonly IMenuSvc _menuSvc;
        private readonly ISystemSvc _systemSvc;
        private readonly PermissionRequirement _requirement;

        /// <summary>
        /// 构造函数注入
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="userSvc"></param>
        /// <param name="roleSvc"></param>
        /// <param name="menuSvc"></param>
        /// <param name="requirement"></param>
        /// <param name="systemSvc"></param>
        public LoginController(IMapper mapper, IUserSvc userSvc, IRoleSvc roleSvc, IMenuSvc menuSvc, PermissionRequirement requirement, ISystemSvc systemSvc)
        {
            _mapper = mapper;
            _userSvc = userSvc;
            _roleSvc = roleSvc;
            _menuSvc = menuSvc;
            _systemSvc = systemSvc;
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
                return MessageModel<string>.Msg(false, "用户名或密码不能为空");
            }

            if (RSACryption.IsBase64(pass)) pass = RSACryption.RSADecrypt(pass);

            var user = (await _userSvc.Query(d => d.Enabled == false && d.LoginName == name && d.LoginPwd == pass)).FirstOrDefault();
            if (user != null)
            {
                var userRoles = await _roleSvc.GetDataByids(user.RoleIds);

                //如果是基于用户的授权策略，这里要添加用户;如果是基于角色的授权策略，这里要添加角色
                var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, name),
                    new Claim(JwtRegisteredClaimNames.Jti, user.Id.ToString()),
                    new Claim("TenantId", user.TenantId.ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToString()),
                    new Claim(ClaimTypes.Expiration, DateTime.Now.AddSeconds(_requirement.Expiration.TotalSeconds).ToString())
                };
                claims.AddRange(userRoles.Select(s => new Claim(ClaimTypes.Role, s.RoleName)));

                if (userRoles.Count == 0)
                {
                    return MessageModel<string>.Msg(false, "用户未绑定相关角色");
                }

                // ids4和jwt切换
                // jwt
                if (!Permissions.IsUseIds4)
                {
                    _requirement.Permissions = (from item in userRoles
                                                orderby item.Id
                                                select new PermissionItem
                                                {
                                                    Url = "",
                                                    Role = item?.RoleName
                                                }).ToList();
                }

                var token = JwtToken.BuildJwtToken(claims.ToArray(), _requirement);

                return MessageModel<TokenInfoViewModel>.Msg(true, "获取成功", token);
            }
            else
            {
                return MessageModel<string>.Msg(false, "认证失败");
            }
        }

        /// <summary>
        /// 请求刷新Token（以旧换新）
        /// </summary>
        /// <param name="token">token</param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<object> RefreshToken(string token = "")
        {
            string jwtStr = string.Empty;

            if (string.IsNullOrEmpty(token))
            {
                return MessageModel<string>.Msg(false, "传入的token参数不能为空！");
            }
            var tokenModel = JwtHelper.SerializeJwt(token);
            if (tokenModel != null && JwtHelper.CustomSafeVerify(token) && tokenModel.Id > 0)
            {
                var user = await _userSvc.QueryById(tokenModel.Id);
                var value = User.Claims.SingleOrDefault(s => s.Type == JwtRegisteredClaimNames.Iat)?.Value;
                if (value != null && user.CriticalModifyTime > value.ToDateTimeReq())
                {
                    return MessageModel<string>.Msg(false, "很抱歉,授权已失效,请重新授权！");
                }
                if (user != null && !(value != null && user.CriticalModifyTime > value.ToDateTimeReq()))
                {
                    var userRoles = await _roleSvc.GetDataByids(user.RoleIds);

                    //如果是基于用户的授权策略，这里要添加用户;如果是基于角色的授权策略，这里要添加角色
                    var claims = new List<Claim> {
                        new Claim(ClaimTypes.Name, user.LoginName),
                        new Claim(JwtRegisteredClaimNames.Jti, tokenModel.Id.ToString()),
                        new Claim("TenantId", user.TenantId.ToString()),
                        new Claim(ClaimTypes.Expiration, DateTime.Now.AddSeconds(_requirement.Expiration.TotalSeconds).ToString())
                    };
                    claims.AddRange(userRoles.Select(s => new Claim(ClaimTypes.Role, s.RoleName)));

                    //用户标识
                    var identity = new ClaimsIdentity(JwtBearerDefaults.AuthenticationScheme);
                    identity.AddClaims(claims);

                    var refreshToken = JwtToken.BuildJwtToken(claims.ToArray(), _requirement);
                    return MessageModel<TokenInfoViewModel>.Msg(true, "获取成功", refreshToken);
                }
            }

            return MessageModel<string>.Msg(false, "认证失败");
        }

        /// <summary>
        /// swagger登录
        /// </summary>
        /// <param name="loginRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public dynamic SwgLogin([FromBody] SwaggerLoginRequest loginRequest)
        {
            // 这里可以查询数据库等各种校验
            if (loginRequest?.Name == "admin" && loginRequest?.Pwd == "admin")
            {
                HttpContext.Session.SetString("swagger-code", "success");
                return new { result = true };
            }

            return new { result = false };
        }

        /// <summary>
        /// 根据token获取登录信息
        /// </summary>
        /// <param name="token">令牌</param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<object> GetLoginByToken(string token)
        {
            var data = new MessageModel<LoginViewModel>();
            var loginViewModel = new LoginViewModel();

            if (!string.IsNullOrEmpty(token))
            {
                var tokenModel = JwtHelper.SerializeJwt(token);
                if (tokenModel != null && tokenModel.Id > 0)
                {
                    var model = await _userSvc.QueryById(tokenModel.Id);
                    if (model != null)
                    {
                        loginViewModel = _mapper.Map<LoginViewModel>(model);

                        if (model.RoleIds != "")
                        {
                            var roleList = await _roleSvc.GetDataByids(model.RoleIds);

                            var menuData = await _menuSvc.Query();
                            var menuList = await _menuSvc.GetDataByids(roleList.Select(s => s.MenuIds).JoinToString(","), menuData);

                            var systemList = await _systemSvc.Query();

                            loginViewModel.RoleInfoList = _mapper.Map<IList<Role>, IList<RoleViewModel>>(roleList);

                            if (menuList.Count > 0)
                            {
                                //获取数据结构
                                var menuList1 = await _menuSvc.GetMenuTree(menuData, menuList, systemList);

                                loginViewModel.MenuInfoList = _mapper.Map<IList<Menu>, IList<MenuViewModel>>(menuList1);

                                data.Success = true;
                                data.Message = "登陆成功";
                                data.Response = loginViewModel;
                            }
                            else
                            {
                                data.Message = "该角色没有绑定菜单！";
                            }
                        }
                        else
                        {
                            data.Message = "该用户没有绑定角色！";
                        }
                    }
                    else
                    {
                        data.Message = "该用户不存在，请核实！";
                    }
                }
            }

            return data;
        }
    }

    public class SwaggerLoginRequest
    {
        public string? Name { get; set; }
        public string? Pwd { get; set; }
    }
}