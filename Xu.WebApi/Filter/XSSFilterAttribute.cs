using Microsoft.AspNetCore.Mvc.Filters;
using System;
using Xu.Common;

namespace Xu.WebApi
{
    /// <summary>
    /// XSS 过滤器
    /// </summary>
    /// <remarks>在需要进行XSS过滤的控制器或者action上加上对应的[XSSFilt]属性即可</remarks>
    public class XSSFilterAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// OnActionExecuting
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            if (context.ActionArguments.Count == 0) { return; }

            //获取参数集合
            var ps = context.ActionDescriptor.Parameters;
            //遍历参数集合
            foreach (var p in ps)
            {
                if (context.ActionArguments[p.Name] != null)
                {
                    //当参数是string
                    if (p.ParameterType.Equals(typeof(string)))
                    {
                        context.ActionArguments[p.Name] = XSSHelper.XssFilter(context.ActionArguments[p.Name].ToString());
                    }
                    else if (p.ParameterType.IsClass)//当参数是一个实体
                    {
                        PostModelFieldFilter(p.ParameterType, context.ActionArguments[p.Name]);
                    }
                }
            }
        }

        /// <summary>
        /// 遍历实体的字符串属性
        /// </summary>
        /// <param name="type">数据类型</param>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        private object PostModelFieldFilter(Type type, object obj)
        {
            if (obj != null)
            {
                foreach (var item in type.GetProperties())
                {
                    if (item.GetValue(obj) != null)
                    {
                        //当参数是string
                        if (item.PropertyType.Equals(typeof(string)))
                        {
                            string value = item.GetValue(obj).ToString();
                            item.SetValue(obj, XSSHelper.XssFilter(value));
                        }
                        else if (item.PropertyType.IsClass)//当参数是一个实体
                        {
                            item.SetValue(obj, PostModelFieldFilter(item.PropertyType, item.GetValue(obj)));
                        }
                    }
                }
            }
            return obj;
        }
    }
}