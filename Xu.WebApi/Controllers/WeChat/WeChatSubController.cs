using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Xu.Common;
using Xu.IServices;
using Xu.Model;
using Xu.Model.Models;
using Xu.Model.ResultModel;

namespace Xu.WebApi.Controllers
{
    /// <summary>
    /// WeChatSubController
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Permissions.Name)]
    public partial class WeChatSubController : Controller
    {
        private readonly IWeChatSubServices _WeChatSubServices;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="iWeChatSubServices"></param>
        public WeChatSubController(IWeChatSubServices iWeChatSubServices)
        {
            _WeChatSubServices = iWeChatSubServices;
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="pagination">分页条件</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<MessageModel<PageModel<WeChatSub>>> Get([FromQuery] PaginationModel pagination)
        {
            var data = await _WeChatSubServices.QueryPage(pagination);
            return new MessageModel<PageModel<WeChatSub>> { Success = true, Response = data };
        }

        /// <summary>
        /// 获取(id)
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<MessageModel<WeChatSub>> Get(string id)
        {
            var data = await _WeChatSubServices.QueryById(id);
            return new MessageModel<WeChatSub> { Success = true, Response = data };
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<MessageModel<string>> Post([FromBody] WeChatSub obj)
        {
            await _WeChatSubServices.Add(obj);
            return new MessageModel<string> { Success = true };
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        public async Task<MessageModel<string>> Put([FromBody] WeChatSub obj)
        {
            await _WeChatSubServices.Update(obj);
            return new MessageModel<string> { Success = true };
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        public async Task<MessageModel<string>> Delete(string id)
        {
            await _WeChatSubServices.DeleteById(id);
            return new MessageModel<string> { Success = true };
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        public async Task<MessageModel<string>> BatchDelete(string ids)
        {
            var i = ids.Split(",");
            await _WeChatSubServices.DeleteByIds(i);
            return new MessageModel<string> { Success = true };
        }
    }
}