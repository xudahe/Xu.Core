using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Xu.Common;
using Xu.IRepository;
using Xu.Model.ResultModel;

namespace Xu.WebApi.Controllers
{
    /// <summary>
    /// 数据迁移
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Permissions.Name)]
    public class MigrateController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        //private readonly IRoleModulePermissionServices _roleModulePermissionServices;
        private readonly IWebHostEnvironment _env;

        public MigrateController(IUnitOfWork unitOfWork, IWebHostEnvironment env)
        {
            _unitOfWork = unitOfWork;
            //_roleModulePermissionServices = roleModulePermissionServices;
            _env = env;
        }

        /// <summary>
        /// 获取权限部分Map数据
        /// 生成到tsb文件
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public object GetRMPMaps(string className)
        {
            var data = new MessageModel<string>() { Success = true, Message = "" };
            if (_env.IsDevelopment())
            {
                // 获取数据，当然，你可以做个where查询
                //var rmps = await _roleModulePermissionServices.GetRMPMaps();
                //var serializeStr = JsonConvert.SerializeObject(rmps);
                // 将序列化的json生成到tsv文件
                //var roleModulePermissions = JsonConvert.DeserializeObject<List<RoleModulePermission>>(serializeStr);

                data.Success = true;
                data.Message = "生成成功！";
            }
            else
            {
                data.Success = false;
                data.Message = "当前不处于开发模式，代码生成不可用！";
            }

            return data;
        }

        /// <summary>
        /// 保存数据到数据库
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public object SaveDataToDb()
        {
            var data = new MessageModel<string>() { Success = true, Message = "" };
            if (_env.IsDevelopment())
            {
                // 取出json数据，反序列化
                var rmpSerializeStr = "";
                //var roleModulePermissions = JsonConvert.DeserializeObject<List<RoleModulePermission>>(rmpSerializeStr);

                try
                {
                    _unitOfWork.BeginTran();

                    //foreach (var item in roleModulePermissions)
                    //{
                    //    // 添加角色,返回rid
                    //    var role = item.Role;
                    //    // 添加菜单,返回mid
                    //    var module = item.Module;
                    //    // 添加接口,返回pid
                    //    var permission = item.Permission;

                    //    // 添加关系表

                    //}

                    _unitOfWork.CommitTran();
                }
                catch (Exception)
                {
                    _unitOfWork.RollbackTran();
                }

                data.Success = true;
                data.Message = "生成成功！";
            }
            else
            {
                data.Success = false;
                data.Message = "当前不处于开发模式，代码生成不可用！";
            }

            return data;
        }
    }
}