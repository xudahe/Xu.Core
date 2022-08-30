using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xu.Model.Models;

namespace Xu.IServices
{
    public interface IPlatformSvc : IBaseSvc<Platform>
    {
        /// <summary>
        /// 根据平台id或guid集合 过滤数据
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        Task<List<Platform>> GetDataByids(string ids, List<Platform> list = null);
    }
}