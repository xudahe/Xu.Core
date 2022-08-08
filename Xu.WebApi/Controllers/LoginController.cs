﻿using AutoMapper;
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
        private readonly PermissionRequirement _requirement;

        /// <summary>
        /// 构造函数注入
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="userSvc"></param>
        /// <param name="roleSvc"></param>
        /// <param name="menuSvc"></param>
        /// <param name="requirement"></param>
        public LoginController(IMapper mapper, IUserSvc userSvc, IRoleSvc roleSvc, IMenuSvc menuSvc, PermissionRequirement requirement)
        {
            _mapper = mapper;
            _userSvc = userSvc;
            _roleSvc = roleSvc;
            _menuSvc = menuSvc;
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

            if (RSACryption.IsBase64(pass)) pass = RSACryption.RSADecrypt(pass);

            var user = (await _userSvc.Query(d => d.Enabled == false && d.LoginName == name && d.LoginPwd == pass)).FirstOrDefault();
            if (user != null)
            {
                var userRoles = await _roleSvc.GetDataByids(user.RoleIds);

                //如果是基于用户的授权策略，这里要添加用户;如果是基于角色的授权策略，这里要添加角色
                var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, name),
                    new Claim(JwtRegisteredClaimNames.Jti, user.Id.ToString()),
                    new Claim(ClaimTypes.Expiration, DateTime.Now.AddSeconds(_requirement.Expiration.TotalSeconds).ToString()) };
                claims.AddRange(userRoles.Select(s => new Claim(ClaimTypes.Role, s.RoleName)));

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
        [AllowAnonymous]
        public async Task<object> RefreshToken(string token = "")
        {
            string jwtStr = string.Empty;

            if (string.IsNullOrEmpty(token))
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "传入的token参数不能为空！"
                });
            }
            var tokenModel = JwtHelper.SerializeJwt(token);
            if (tokenModel != null && JwtHelper.CustomSafeVerify(token) && tokenModel.Id > 0)
            {
                var user = await _userSvc.QueryById(tokenModel.Id);
                if (user != null)
                {
                    var userRoles = await _roleSvc.GetDataByids(user.RoleIds);

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
                            loginViewModel.RoleInfoList = _mapper.Map<IList<Role>, IList<RoleViewModel>>(roleList);

                            var menuIds = roleList.Select(s => s.MenuIds).ToList().JoinToString(",");

                            var menuData = await _menuSvc.Query();
                            var menuList = await _menuSvc.GetDataByids(menuIds, menuData);

                            if (menuList.Count > 0)
                            {
                                for (int i = 0; i < menuList.Count; i++)
                                {
                                    menuList[i].ParentName = menuList[i].ParentId.HasValue ? (menuData.FirstOrDefault(s => s.Id == menuList[i].ParentId).MenuName) : "";
                                }

                                var menuList1 = menuList.Where(s => !s.ParentId.HasValue).OrderBy(s => s.Index).ToList(); //获取一级菜单（顶部）
                                for (int i = 0; i < menuList1.Count; i++)
                                {
                                    var menuList2 = menuList.Where(s => s.ParentId == menuList1[i].Id).OrderBy(s => s.Index).ToList(); //获取二级菜单

                                    for (int j = 0; j < menuList2.Count; j++)
                                    {
                                        menuList2[j].Children = menuList.Where(s => s.ParentId == menuList2[j].Id).OrderBy(s => s.Index).ToList(); //获取三级菜单
                                    }
                                    menuList1[i].Children = menuList2;
                                }

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
        public string Name { get; set; }
        public string Pwd { get; set; }
    }
}