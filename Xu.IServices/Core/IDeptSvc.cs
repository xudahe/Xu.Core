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
        /// ÃÌº”≤ø√≈
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        Task<Dept> SaveDept(Dept dept);
    }
}