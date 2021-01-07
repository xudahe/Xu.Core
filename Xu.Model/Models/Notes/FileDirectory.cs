using System;
using System.Collections.Generic;
using System.Text;

namespace Xu.Model.Models
{
    /// <summary>
    /// 文件夹目录
    /// </summary>
    public class FileDirectory : ModelBase
    {
        /// <summary>
        /// 当前文件名显示等级
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// 当前文件父级文件夹Id
        /// </summary>
        public int? ParentId { get; set; }

        /// <summary>
        /// 当前文件父级文件夹名称
        /// </summary>
        public string ParentName { get; set; }

        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 当前文件是否为文件夹
        /// </summary>
        public int IsDirectory { get; set; }

        /// <summary>
        /// 实体路径
        /// </summary>
        public string EntityPath { get; set; }
    }
}