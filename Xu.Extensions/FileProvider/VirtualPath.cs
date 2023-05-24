using Microsoft.Extensions.FileProviders;
using System.Collections.Generic;

namespace Xu.Extensions
{
    /// <summary>
    /// 虚拟目录
    /// </summary>
    public class VirtualPath
    {
        /// <summary>
        /// 物理真实地址
        /// </summary>
        public string RealPath { get; set; }

        /// <summary>
        /// 请求地址
        /// </summary>
        public string RequestPath { get; set; }

        /// <summary>
        /// 地址对应别名
        /// </summary>
        public string Alias { get; set; }
    }

    /// <summary>
    /// 映射实体
    /// </summary>
    public class VirtualPathConfig
    {
        public List<VirtualPath> VirtualPath { get; set; }
    }

    public class FileProvider : PhysicalFileProvider
    {
        public FileProvider(string root, string alias) : base(root)
        {
            this.Alias = alias;
        }

        public FileProvider(string root, Microsoft.Extensions.FileProviders.Physical.ExclusionFilters filters, string alias) : base(root, filters)
        {
            this.Alias = alias;
        }

        /// <summary>
        /// 别名
        /// </summary>
        public string Alias { get; set; }
    }
}