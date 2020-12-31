using System.Collections.Generic;
using System.Threading.Tasks;
using Xu.Model.Models;

namespace Xu.IServices
{
    /// <summary>
    /// IDeptServices
    /// </summary>
    public interface IDeptSvc : IBaseSvc<Dept>
    {
        /// <summary>
        /// 根据部门id或guid集合 过滤数据
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        Task<List<Dept>> GetDataByids(string ids, List<Dept> list = null);
    }
}