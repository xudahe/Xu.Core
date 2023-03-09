using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xu.Common;
using Xu.IRepository;
using Xu.IServices;
using Xu.Model.Models;

namespace Xu.Services.Core
{
    public class MenuSvc : BaseSvc<Menu>, IMenuSvc
    {
        public MenuSvc(IBaseRepo<Menu> dalRepo)
        {
            base.BaseDal = dalRepo;
        }

        public async Task<List<Menu>> GetDataByids(string ids, List<Menu> list = null)
        {
            list = list ?? await base.Query();

            if (!string.IsNullOrEmpty(ids))
            {
                var idList = ids.Split(',');
                if (GUIDHelper.IsGuidByReg(idList[0]))
                    return list.Where(s => idList.Contains(s.Guid)).ToList();
                else
                    return list.Where(s => idList.ToInt32List().Contains(s.Id)).ToList();
            }

            return list;
        }

        public async Task<List<Menu>> GetMenuTree(List<Menu> menuData, List<Menu> menuList,List<Systems> systemList)
        {
            for (int i = 0; i < menuList.Count; i++)
            {
                if (!string.IsNullOrEmpty(menuList[i].SystemId))
                {
                    if (GUIDHelper.IsGuidByReg(menuList[i].SystemId))
                        menuList[i].SystemName = systemList.First(s => s.Guid == menuList[i].SystemId).SystemName;
                    else
                        menuList[i].SystemName = systemList.First(s => s.Id == menuList[i].SystemId.ToInt32()).SystemName;
                }

                if (!string.IsNullOrEmpty(menuList[i].ParentId))
                {
                    if (GUIDHelper.IsGuidByReg(menuList[i].ParentId))
                        menuList[i].ParentName = menuData.First(s => s.Guid == menuList[i].ParentId).MenuName;
                    else
                        menuList[i].ParentName = menuData.First(s => s.Id == menuList[i].ParentId.ToInt32()).MenuName;
                }
            }

            //获取一级菜单
            var menuList1 = menuList.Where(s => string.IsNullOrEmpty(s.ParentId)).OrderBy(s => s.Index).ToList();

            //获取二级菜单
            for (int i = 0; i < menuList1.Count; i++)
            {
                var menuList2 = menuList.Where(s => s.ParentId == menuList1[i].Guid).ToList();
                //var menuList2 = menuList.Where(s => s.ParentId == menuList1[i].Id.ToString()).ToList();

                //获取三级菜单
                for (int j = 0; j < menuList2.Count; j++)
                {
                    menuList2[j].Children = menuList.Where(s => s.ParentId == menuList2[j].Guid).OrderBy(s => s.Index).ToList();
                    //menuList2[j].Children = menuList.Where(s => s.ParentId == menuList2[j].Id.ToString()).ToList();
                }

                menuList1[i].Children = menuList2.OrderBy(s => s.Index).ToList();
            }

            return menuList1;
        }
    }
}