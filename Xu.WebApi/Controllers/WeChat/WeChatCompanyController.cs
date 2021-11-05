using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Xu.Common;
using Xu.IServices;
using Xu.Model;
using Xu.Model.Models;
using Xu.Model.ResultModel;

namespace Blog.Core.Controllers
{
    /// <summary>
	/// WeChatCompanyController
	/// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Permissions.Name)]
    public partial class WeChatCompanyController : Controller
    {
        private readonly IWeChatCompanyServices _WeChatCompanyServices;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="iWeChatCompanyServices"></param>
        public WeChatCompanyController(IWeChatCompanyServices iWeChatCompanyServices)
        {
            _WeChatCompanyServices = iWeChatCompanyServices;
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="pagination">分页条件</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<MessageModel<PageModel<WeChatCompany>>> Get([FromQuery] PaginationModel pagination)
        {
            var data = await _WeChatCompanyServices.QueryPage(pagination);
            return new MessageModel<PageModel<WeChatCompany>> { Success = true, Response = data };
        }

        /// <summary>
        /// 获取(id)
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<MessageModel<WeChatCompany>> Get(string id)
        {
            var data = await _WeChatCompanyServices.QueryById(id);
            return new MessageModel<WeChatCompany> { Success = true, Response = data };
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<MessageModel<string>> Post([FromBody] WeChatCompany obj)
        {
            await _WeChatCompanyServices.Add(obj);
            return new MessageModel<string> { Success = true };
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        public async Task<MessageModel<string>> Put([FromBody] WeChatCompany obj)
        {
            await _WeChatCompanyServices.Update(obj);
            return new MessageModel<string> { Success = true };
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        public async Task<MessageModel<string>> Delete(string id)
        {
            await _WeChatCompanyServices.DeleteById(id);
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
            await _WeChatCompanyServices.DeleteByIds(i);
            return new MessageModel<string> { Success = true };
        }
    }
}