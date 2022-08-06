﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xu.Model.Models;

namespace Xu.IServices
{
         public interface ISystemSvc : IBaseSvc<Systems>
    {
        /// <summary>
        /// 根据平台id或guid集合 过滤数据
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        Task<List<Systems>> GetDataByids(string ids, List<Systems> list = null);
    }
}