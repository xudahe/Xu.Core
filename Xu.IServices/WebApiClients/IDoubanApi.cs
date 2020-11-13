using WebApiClient;
using WebApiClient.Attributes;
using Xu.Model.ViewModel;

namespace Xu.IServices.WebApiClients
{
    /// <summary>
    /// 豆瓣视频管理
    /// </summary>
    [TraceFilter(OutputTarget = OutputTarget.Console)] // 输出到控制台窗口
    public interface IDoubanApi : IHttpApi
    {
        /// <summary>
        /// 获取电影详情
        /// </summary>
        /// <param name="isbn"></param>
        [HttpGet("api/bookinfo")]
        [Timeout(10 * 1000)] // 10s超时
        ITask<DoubanViewModel> VideoDetailAsync(string isbn);
    }
}