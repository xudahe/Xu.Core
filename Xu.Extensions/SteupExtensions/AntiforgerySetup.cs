using Microsoft.Extensions.DependencyInjection;
using System;

namespace Xu.Extensions
{
    /// <summary>
    /// CSRF 启动服务
    /// </summary>
    /// <remarks>
    /// CSRF是跨站请求伪造,简单的说,就是我有一个表单,我可以添加一个人员,
    /// 然后直接在浏览器输入https://xxx.movies/Add?Title=123这样也可以添加内容,这个明显是不安全的,
    /// 我只要求,只能在我的浏览器上使用表单添加信息,换一个浏览器只要不是登录了使用表单,就不允许加入信息,
    /// </remarks>
    public static class AntiforgerySetup
    {
        public static void AddAntiforgerySetup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            //防止CSRF攻击
            services.AddAntiforgery(options =>
            {
                //使用cookiebuilder属性设置cookie属性。
                options.FormFieldName = "AntiforgeryKey";
                options.HeaderName = "X-CSRF-TOKEN";
                options.SuppressXFrameOptionsHeader = false;
            });
        }
    }
}
