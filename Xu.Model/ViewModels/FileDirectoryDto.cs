namespace Xu.Model.ViewModels
{
    /// <summary>
    /// 显示文件夹目录
    /// </summary>
    public class FileDirectoryDto
    {
        /// <summary>
        /// 当前文件名显示等级
        /// </summary>
        public int Level { get; set; }

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