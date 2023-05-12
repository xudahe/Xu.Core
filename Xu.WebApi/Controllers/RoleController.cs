using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Xu.Common;
using Xu.IServices;
using Xu.Model.Models;
using Xu.Model.ResultModel;
using Xu.Model.XmlModels;

namespace Xu.WebApi.Controllers
{
    /// <summary>
    /// 角色管理
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Permissions.Name)]
    public class RoleController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IRoleSvc _roleSvc;
        private readonly IMenuSvc _menuSvc;
        private readonly ISystemSvc _systemSvc;
        private readonly IPlatformSvc _platformSvc;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="roleSvc"></param>
        /// <param name="menuSvc"></param>
        /// <param name="systemSvc"></param>
        /// <param name="platformSvc"></param>
        public RoleController(IMapper mapper, IRoleSvc roleSvc, IMenuSvc menuSvc, ISystemSvc systemSvc, IPlatformSvc platformSvc)
        {
            _mapper = mapper;
            _roleSvc = roleSvc;
            _menuSvc = menuSvc;
            _systemSvc = systemSvc;
            _platformSvc = platformSvc;
        }

        /// <summary>
        /// 获取角色数据
        /// </summary>
        /// <param name="ids">角色id或guid集合（可空）</param>
        /// <param name="key">角色名称或角色简码（可空）</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<object> GetRole(string ids = "", string key = "")
        {
            var data = await _roleSvc.GetDataByids(ids);

            if (!string.IsNullOrEmpty(key))
                data = data.Where(a => a.RoleName.Contains(key) || a.RoleCode.Contains(key)).ToList();

            return new MessageModel<List<Role>>()
            {
                Message = "获取成功",
                Success = true,
                Response = data
            };
        }

        /// <summary>
        /// 获取全部角色并分页
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="roleName">角色名称（可空）</param>
        /// <returns></returns>
        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<object> GetRoleByPage(int page = 1, int pageSize = 50, string roleName = "")
        {
            var data = await _roleSvc.QueryPage(a => (a.RoleName != null && a.RoleName.Contains(roleName)), page, pageSize, " Id desc ");

            return new MessageModel<PageModel<Role>>()
            {
                Message = "获取成功",
                Success = true,
                Response = data
            };
        }

        /// <summary>
        /// 添加角色
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<object> PostRole([FromBody] Role model)
        {
            var data = new MessageModel<Role>() { Message = "添加成功", Success = true };

            var dataList = await _roleSvc.Query(a => a.RoleName == model.RoleName);
            if (dataList.Count > 0)
            {
                data.Message = "该角色已存在";
            }
            else
            {
                model.Id = await _roleSvc.Add(model);
                data.Response = model;
                data.Message = "添加成功";
            }

            return data;
        }

        /// <summary>
        /// 更新角色
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<object> PutRole([FromBody] Role model)
        {
            var data = new MessageModel<string>();

            if (model != null && model.Id > 0)
            {
                data.Success = await _roleSvc.Update(model);
                if (data.Success)
                {
                    data.Message = "更新成功";
                }
            }

            return data;
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="id">角色id（非空）</param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<object> DeleteRole(int id)
        {
            var data = new MessageModel<string>();
            if (id > 0)
            {
                var model = await _roleSvc.QueryById(id);
                model.DeleteTime = DateTime.Now;
                data.Success = await _roleSvc.Update(model);
                if (data.Success)
                {
                    data.Message = "删除成功";
                }
            }

            return data;
        }

        /// <summary>
        /// 禁用角色
        /// </summary>
        /// <param name="id">角色id（非空）</param>
        /// <param name="falg">true(禁用),false(启用)</param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<object> DisableRole(int id, bool falg)
        {
            var model = await _roleSvc.QueryById(id);
            model.Enabled = falg;

            var data = new MessageModel<string>()
            {
                Success = await _roleSvc.Update(model)
            };

            if (data.Success)
            {
                data.Message = falg ? "禁用成功" : "启用成功";
            }

            return data;
        }

        /// <summary>
        /// 角色-->平台-->系统-->菜单
        /// </summary>
        /// <param name="roleId">角色id或guid</param>
        /// <param name="platId">平台id或guid</param>
        /// <param name="systemId">系统id或guid</param>
        /// <param name="menuIds">菜单id或guid，小写逗号隔开","</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<object> RoleByMenuId(string roleId, string platId, string systemId, string menuIds)
        {
            try
            {
                var roleModal = (await _roleSvc.GetDataByids(roleId.ToString())).First();

                var platModal = (await _platformSvc.GetDataByids(platId.ToString())).First();

                var systemModal = (await _systemSvc.GetDataByids(systemId.ToString())).First();

                var menuModal = new List<Menu>();

                InfoPlatform infoPlatform = _mapper.Map<Platform, InfoPlatform>(platModal);
                InfoSystem infoSystem = _mapper.Map<Systems, InfoSystem>(systemModal);
                IList<InfoMenu> infoMenuList = new List<InfoMenu>();

                if (!string.IsNullOrEmpty(menuIds))
                {
                    menuModal = await _menuSvc.GetDataByids(menuIds);
                    infoMenuList = _mapper.Map<IList<Menu>, IList<InfoMenu>>(menuModal);
                }
                else
                {
                    return new MessageModel<string>()
                    {
                        Message = "关联的菜单为空！"
                    };
                }

                IList<InfoPlatform> defaultList = new List<InfoPlatform>();

                var platFalg = false;
                var menuIds_new = menuIds;

                if (roleModal.InfoList != null && roleModal.InfoList.Count > 0)
                {
                    defaultList = roleModal.InfoList;

                    for (int i = 0; i < defaultList.Count; i++)
                    {
                        //是否存在该平台
                        if (defaultList[i].Guid == infoPlatform.Guid)
                        {
                            platFalg = true;

                            var systemFlag = false;

                            if (defaultList[i].InfoSystemList != null && defaultList[i].InfoSystemList.Count > 0)
                            {
                                for (int j = 0; j < defaultList[i].InfoSystemList.Count; j++)
                                {
                                    //是否存在该系统
                                    if (defaultList[i].InfoSystemList[j].Guid == infoSystem.Guid)
                                    {
                                        systemFlag = true;
                                        defaultList[i].InfoSystemList[j].InfoMenuList = infoMenuList;
                                    }
                                    menuIds_new += "," + defaultList[i].InfoSystemList[j].InfoMenuList.Select(s => s.Guid).JoinToString(",");
                                }
                            }

                            if (!systemFlag)
                            {
                                infoSystem.InfoMenuList = infoMenuList;
                                defaultList[i].InfoSystemList.Add(infoSystem);
                            }
                        }
                    }
                }
                if (!platFalg)
                {
                    infoSystem.InfoMenuList = infoMenuList;
                    infoPlatform.InfoSystemList.Add(infoSystem);
                    defaultList.Add(infoPlatform);
                }

                roleModal.MenuIds = menuIds_new.TrimStart(',').TrimEnd(',');
                roleModal.InfoList = defaultList;
                await _roleSvc.Update(roleModal);

                return new MessageModel<string>()
                {
                    Success = true,
                    Message = "更新成功"
                };
            }
            catch (Exception)
            {
                return new MessageModel<string>()
                {
                    Message = "更新失败"
                };
            }
        }
    }
}