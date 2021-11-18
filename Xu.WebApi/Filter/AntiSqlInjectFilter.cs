using Microsoft.AspNetCore.Mvc.Filters;
using System;
using Xu.Common.Helper;

namespace Xu.WebApi
{
    /// <summary>
    /// SQL注入过滤器
    /// </summary>
    public class AntiSqlInjectFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            if (context.ActionArguments.Count == 0) { return; }

            //获取参数集合
            var ps = context.ActionDescriptor.Parameters;
            //遍历参数集合
            foreach (var p in ps)
            {
                if (context.ActionArguments.ContainsKey(p.Name))
                {
                    //当参数是string，进行过滤
                    if (p.ParameterType.Equals(typeof(string)))
                    {
                        context.ActionArguments[p.Name] = AntiSqlInject.GetSafetySql(context.ActionArguments[p.Name].ToString());
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
                        //当参数是string，进行过滤
                        if (item.PropertyType.Equals(typeof(string)))
                        {
                            item.SetValue(obj, AntiSqlInject.GetSafetySql(item.GetValue(obj).ToString()));
                        }
                        else if (item.PropertyType.IsClass)//当参数是一个实体
                        {
                            item.SetValue(obj, PostModelFieldFilter(item.PropertyType, item.GetValue(obj)));
                        }
                        else
                        {
                            item.SetValue(obj, item.GetValue(obj));
                        }
                    }
                }
            }
            return obj;
        }
    }
}