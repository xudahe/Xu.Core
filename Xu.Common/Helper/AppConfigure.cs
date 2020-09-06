using Microsoft.Extensions.Configuration;
using System.Collections.Concurrent;

namespace Xu.Common
{
    public static class AppConfigure
    {
        // 缓存字典
        private static readonly ConcurrentDictionary<string, IConfigurationRoot> _cacheDict;

        static AppConfigure()
        {
            _cacheDict = new ConcurrentDictionary<string, IConfigurationRoot>();
        }

        // 传入 JSON 文件夹路径与当前的环境变量值
        public static IConfigurationRoot GetConfigurationRoot(string jsonDir, string environmentName = null)
        {
            // 设置缓存的 KEY
            var cacheKey = $"{jsonDir}#{environmentName}";

            // 添加默认的 JSON 配置
            var builder = new ConfigurationBuilder().SetBasePath(jsonDir).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            // 根据环境变量添加相应的 JSON 配置文件
            if (!string.IsNullOrEmpty(environmentName))
            {
                builder = builder.AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true);
            }

            // 返回构建成功的 IConfigurationRoot 对象
            return builder.Build();
        }
    }
}