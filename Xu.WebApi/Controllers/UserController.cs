using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xu.Common;
using Xu.IRepository;
using Xu.IServices;
using Xu.Model;
using Xu.Model.Models;

namespace Xu.WebApi.Controllers
{
    /// <summary>
    /// 用户管理
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Permissions.Name)]
    public class UserController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserSvc _userSvc;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="userSvc"></param>
        public UserController(IUnitOfWork unitOfWork, IUserSvc userSvc)
        {
            _unitOfWork = unitOfWork;
            _userSvc = userSvc;
        }

        /// <summary>
        /// 获取全部用户并分页
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<object> Get(int page = 1, int pageSize = 50, string key = "")
        {
            var data = await _userSvc.QueryPage(a => a.DeleteTime == null && ((a.LoginName != null && a.LoginName.Contains(key)) || (a.RealName != null && a.RealName.Contains(key))), page, pageSize, " Id desc ");

            return new MessageModel<PageModel<User>>()
            {
                Message = "获取成功",
                Success = true,
                Response = data
            };
        }

        /// <summary>
        /// 根据用户Ids集合获取用户
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<object> GetByIds(string ids)
        {
            var data = new MessageModel<List<User>>();
            if (!string.IsNullOrEmpty(ids))
            {
                var userList = await _userSvc.QueryByIds(ids.Split(","));

                data.Response = userList;
                data.Success = true;
                data.Message = "获取成功";
            }

            return data;
        }

        /// <summary>
        /// 根据token获取用户详情
        /// </summary>
        /// <param name="token">令牌</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<object> GetInfoByToken(string token)
        {
            var data = new MessageModel<User>();
            if (!string.IsNullOrEmpty(token))
            {
                var tokenModel = JwtHelper.SerializeJwt(token);
                if (tokenModel != null && tokenModel.Id > 0)
                {
                    var user = await _userSvc.QueryById(tokenModel.Id);
                    if (user != null)
                    {
                        data.Response = user;
                        data.Success = true;
                        data.Message = "获取成功";
                    }
                }
            }
            return data;
        }

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<object> Post([FromBody] User user)
        {
            user.LoginPwd = MD5Helper.MD5Encrypt32(user.LoginPwd);
            var model = await _userSvc.SaveUser(user);

            return new MessageModel<User>()
            {
                Message = "添加成功",
                Success = true,
                Response = model
            };
        }

        /// <summary>
        /// 更新用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<object> Put([FromBody] User user)
        {
            // 这里使用事务处理
            var data = new MessageModel<string>();
            try
            {
                _unitOfWork.BeginTran();

                if (user != null && user.Id > 0)
                {
                    data.Success = await _userSvc.Update(user);

                    _unitOfWork.CommitTran();

                    if (data.Success)
                    {
                        data.Message = "更新成功";
                        data.Response = user?.Id.ToString();
                    }
                }
            }
            catch (Exception)
            {
                _unitOfWork.RollbackTran();
            }

            return data;
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<object> Delete(int id)
        {
            var data = new MessageModel<string>();
            if (id > 0)
            {
                var menu = await _userSvc.QueryById(id);
                menu.DeleteTime = DateTime.Now;
                data.Success = await _userSvc.Update(menu);
                if (data.Success)
                {
                    data.Message = "删除成功";
                    data.Response = menu?.Id.ToString();
                }
            }

            return data;
        }

        /// <summary>
        /// 禁用用户
        /// </summary>
        /// <param name="id"></param>
        /// <param name="falg"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<object> Disable(int id, bool falg)
        {
            var user = await _userSvc.QueryById(id);
            user.Enabled = falg;

            var data = new MessageModel<string>()
            {
                Success = await _userSvc.Update(user)
            };

            if (data.Success)
            {
                data.Message = falg ? "禁用成功" : "启用成功";
                data.Response = user?.Id.ToString();
            }

            return data;
        }
    }
}