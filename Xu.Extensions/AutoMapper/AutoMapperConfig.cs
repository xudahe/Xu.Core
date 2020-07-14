using AutoMapper;

namespace Xu.Extensions
{
    /// <summary>
    /// 静态全局 AutoMapper 配置文件
    /// </summary>
    /// <remarks>
    /// Automapper是一种实体转换关系的模型，AutoMapper是一个.NET的对象映射工具。
    /// 主要作用是进行领域对象与模型（DTO）之间的转换、数据库查询结果映射至实体对象。
    /// </remarks>
    public class AutoMapperConfig
    {
        public static MapperConfiguration RegisterMappings()
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new CustomProfile());
            });
        }
    }
}