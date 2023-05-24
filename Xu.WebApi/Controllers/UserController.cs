using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Xu.Common;
using Xu.Extensions;
using Xu.IServices;
using Xu.Model;
using Xu.Model.Models;
using Xu.Model.ResultModel;
using Xu.Repository;

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
        private readonly IUnitOfWorkManage _unitOfWorkManage;
        private readonly IMapper _mapper;
        private readonly IRoleSvc _roleSvc;
        private readonly IUserSvc _userSvc;
        private readonly ILogger<UserController> _logger;

        public UserController(IUnitOfWorkManage unitOfWorkManage, IMapper mapper, IUserSvc userSvc, IRoleSvc roleSvc, ILogger<UserController> logger)
        {
            _unitOfWorkManage = unitOfWorkManage;
            _mapper = mapper;
            _userSvc = userSvc;
            _roleSvc = roleSvc;
            _logger = logger;
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
                    var model = await _userSvc.QueryById(tokenModel.Id);
                    if (model != null)
                    {
                        data.Response = model;
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
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<object> PostUser([FromBody] User model)
        {
            var data = new MessageModel<User>() { Message = "添加成功", Success = true };

            //model.LoginPwd = MD5Helper.MD5Encrypt32(model.LoginPwd);
            var dataList = await _userSvc.Query(a => a.LoginName == model.LoginName);
            if (dataList.Count > 0)
            {
                data.Message = "该用户名称已存在";
                data.Success = false;
            }
            else
            {
                model.Id = await _userSvc.Add(model);
                data.Response = model;
                data.Message = "添加成功";
            }

            return data;
        }

        /// <summary>
        /// 更新用户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<object> PutUser([FromBody] User model)
        {
            // 这里使用事务处理
            var data = new MessageModel<string>();
            try
            {
                _unitOfWorkManage.BeginTran();

                if (model != null && model.Id > 0)
                {
                    var roleList = await _roleSvc.GetDataByids(model.RoleIds);
                    model.RoleInfoList = _mapper.Map<IList<Role>, IList<InfoRole>>(roleList);

                    data.Success = await _userSvc.Update(model);

                    if (data.Success)
                    {
                        data.Message = "更新成功";
                    }

                    _unitOfWorkManage.CommitTran();
                }
            }
            catch (Exception)
            {
                _unitOfWorkManage.RollbackTran();
            }

            return data;
        }

        /// <summary>
        /// 修改用户密码
        /// </summary>
        /// <param name="userId">用户id或guid</param>
        /// <param name="oldPwd">原密码</param>
        /// <param name="newPwd">新密码</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<object> UpdatePassword([FromForm] string userId, [FromForm] string oldPwd, [FromForm] string newPwd)
        {
            var data = new MessageModel<string>();

            if (RSACryption.IsBase64(oldPwd)) oldPwd = RSACryption.RSADecrypt(oldPwd); //解密
            if (RSACryption.IsBase64(newPwd)) newPwd = RSACryption.RSADecrypt(newPwd); //解密

            if (string.IsNullOrEmpty(oldPwd))
                data.Message = "原密码不能为空";
            else if (string.IsNullOrEmpty(newPwd))
                data.Message = "新密码不能为空";
            else
            {
                var model = (await _userSvc.GetDataByids(userId)).FirstOrDefault();
                if (model != null && model.Id > 0)
                {
                    if (!model.LoginPwd.Equals(oldPwd))
                    {
                        data.Message = "原密码验证错误";
                    }
                    else
                    {
                        model.LoginPwd = newPwd;
                        model.CriticalModifyTime = DateTime.Now;
                        data.Success = await _userSvc.Update(model);
                        if (data.Success)
                        {
                            data.Message = "更新成功";
                        }
                    }
                }
                else
                {
                    data.Message = "用户不存在，请联系管理员";
                }
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
            var model = await _userSvc.QueryById(id);
            model.Enabled = falg;

            var data = new MessageModel<string>()
            {
                Success = await _userSvc.Update(model)
            };

            if (data.Success)
            {
                data.Message = falg ? "禁用成功" : "启用成功";
            }

            return data;
        }

        /// <summary>
        /// 用户-->角色
        /// </summary>
        /// <param name="userId">用户id或guid</param>
        /// <param name="roleIds">角色id或guid，小写逗号隔开","</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<object> UserByRoleId([FromForm] string userId, [FromForm] string roleIds)
        {
            var data = new MessageModel<string>();
            var userInfo = (await _userSvc.GetDataByids(userId)).FirstOrDefault();

            if (userInfo != null && userInfo.Id > 0)
            {
                var roleList = await _roleSvc.GetDataByids(roleIds);
                userInfo.RoleInfoList = _mapper.Map<IList<Role>, IList<InfoRole>>(roleList);
                userInfo.RoleIds = roleIds;

                data.Success = await _userSvc.Update(userInfo);
                if (data.Success)
                {
                    data.Message = "更新成功";
                }
            }
            else
            {
                data.Message = "用户不存在，请联系管理员";
            }

            return data;
        }
    }
}