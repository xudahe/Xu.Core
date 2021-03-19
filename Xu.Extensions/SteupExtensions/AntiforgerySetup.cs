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
                options.FormFieldName = "AntiforgeryKey"; //防伪系统用于呈现防伪令牌在视图中的隐藏的窗体字段的名称。
                options.HeaderName = "X-CSRF-TOKEN";  //防伪系统使用的标头的名称。 如果null，系统会认为只有窗体数据。
                options.SuppressXFrameOptionsHeader = false; //指定是否禁止显示生成X - Frame - Options标头。 默认情况下，值为"SAMEORIGIN"生成标头。 默认为 false。
            });
        }
    }
}