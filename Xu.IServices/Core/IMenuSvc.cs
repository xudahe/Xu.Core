﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Xu.Model.Models;

namespace Xu.IServices
{
    public interface IMenuSvc : IBaseSvc<Menu>
    {
        /// <summary>
        /// 根据菜单id或guid集合 过滤数据
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        Task<List<Menu>> GetDataByids(string ids, List<Menu> list = null);
    }
}