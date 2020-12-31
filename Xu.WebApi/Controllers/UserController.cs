using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xu.Common;
using Xu.Extensions;
using Xu.IRepository;
using Xu.IServices;
using Xu.Model;
using Xu.Model.Models;
using Xu.Model.ResultModel;

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
        private readonly IMapper _mapper;
        private readonly IRoleSvc _roleSvc;
        private readonly IUserSvc _userSvc;

        public UserController(IUnitOfWork unitOfWork, IMapper mapper, IUserSvc userSvc, IRoleSvc roleSvc)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userSvc = userSvc;
            _roleSvc = roleSvc;
        }

        /// <summary>
        /// 获取用户数据
        /// </summary>
        /// <param name="ids">用户id或guid集合（可空）</param>
        /// <param name="key">用户名/姓名（可空）</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<object> GetUser(string ids = "", string key = "")
        {
            var data = await _userSvc.GetDataByids(ids);

            if (!string.IsNullOrEmpty(key))
                data = data.Where(a => a.LoginName.Contains(key) || a.RealName.Contains(key)).ToList();

            return new MessageModel<List<User>>()
            {
                Message = "获取成功",
                Success = true,
                Response = data
            };
        }

        /// <summary>
        /// 获取全部用户并分页
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="key">用户名/姓名（可空）</param>
        /// <returns></returns>
        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> GetUserByPage(int page = 1, int pageSize = 50, string key = "")
        {
            var data = await _userSvc.QueryPage(a => (a.LoginName != null && a.LoginName.Contains(key)) || (a.RealName != null && a.RealName.Contains(key)), page, pageSize, " Id desc ");

            return new MessageModel<PageModel<User>>()
            {
                Message = "获取成功",
                Success = true,
                Response = data
            };
        }

        /// <summary>
        /// 根据token获取用户详情
        /// </summary>
        /// <param name="token">令牌</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<object> GetUserByToken(string token)
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
        public async Task<object> PostUser([FromBody] User user)
        {
            var data = new MessageModel<User>() { Message = "添加成功", Success = true };

            //user.LoginPwd = MD5Helper.MD5Encrypt32(user.LoginPwd);
            var dataList = await _userSvc.Query(a => a.LoginName == user.LoginName);
            if (dataList.Count > 0)
            {
                data.Message = "该用户名称已存在";
                data.Success = false;
            }
            else
            {
                user.Id = await _userSvc.Add(user);
                data.Response = user;
            }

            return data;
        }

        /// <summary>
        /// 更新用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<object> PutUser([FromBody] User user)
        {
            // 这里使用事务处理
            var data = new MessageModel<string>();
            try
            {
                _unitOfWork.BeginTran();

                var roleList = await _roleSvc.GetDataByids(user.RoleIds);
                user.RoleInfoList = _mapper.Map<IList<Role>, IList<InfoRole>>(roleList);

                if (user != null && user.Id > 0)
                {
                    user.ModifyTime = DateTime.Now;
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
        /// <param name="id">用户id（非空）</param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<object> DeleteUser(int id)
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
        /// <param name="id">用户id（非空）</param>
        /// <param name="falg">true(禁用),false(启用)</param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<object> DisableUser(int id, bool falg)
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