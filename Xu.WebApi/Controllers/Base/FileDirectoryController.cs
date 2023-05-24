using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Xu.Common;
using Xu.Extensions;
using Xu.Model.ResultModel;
using Xu.Model.ViewModels;

namespace Xu.Controllers
{
    /// <summary>
    /// 树结构制作文件目录
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class FileDirectoryController : Controller
    {
        // 在statups当中已经配置好的文件路径数据
        private readonly VirtualPathConfig virtualPathConfig;

        private int oldLevel;

        public FileDirectoryController(IOptions<VirtualPathConfig> options)
        {
            virtualPathConfig = options.Value;
        }

        /// <summary>
        /// 获取文件夹目录
        /// E:\\test1\\Files 物理真实地址，电脑中一定要存在
        /// </summary>
        /// <param name="alias">（非空）物理真实地址对应别名(first second third) 具体看appsettings.json的VirtualPath配置</param>
        /// <param name="jobNo">（可空）File文件夹下面的文件夹名称</param>
        /// <returns></returns>
        [HttpGet]
        public MessageModel<object> GetFileDirectory(string alias = "first", string jobNo = "")
        {
            var result = new MessageModel<object>();

            //方法一
            // string? fileBaseUrl = virtualPathConfig.VirtualPath.Find(d => d.Alias.Contains(alias.Trim()))?.RealPath;

            //方法二
            var virtualPath = AppSettings.App<VirtualPath>("VirtualPath").ToList();
            string? fileBaseUrl = virtualPath.Find(d => d.Alias.Contains(alias.Trim()))?.RealPath;

            string filePath = Path.Combine(fileBaseUrl, jobNo);
            // 创建文件目录
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            // 记录当前路径下所有文件
            var fileDirectoruList = new List<FileDirectoryDto>();

            DirectoryInfo folder = new DirectoryInfo(filePath);

            #region 获取folder下所有的文件

            var _cloneFileDirModel = new FileDirectoryDto { IsDirectory = 0 };
            _cloneFileDirModel.Level = 0;
            _cloneFileDirModel.FileName = folder.Name;
            _cloneFileDirModel.EntityPath = folder.FullName.Split("Files")[1];
            fileDirectoruList.Add(_cloneFileDirModel);

            AddFiles(folder, 0, ref fileDirectoruList);

            #endregion 获取folder下所有的文件

            // 获取当前文件夹下所有子文件夹
            var dirInfo = folder.GetDirectories();

            if (dirInfo.Length > 0)
            {
                GetFileDirList(dirInfo, 1, ref fileDirectoruList);
            }

            result = new MessageModel<object>()
            {
                Message = "获取成功",
                Success = true,
                Response = fileDirectoruList
            };
            return result;
        }

        /// <summary>
        /// 获取文件夹List
        /// </summary>
        /// <param name="dirInfo"></param>
        /// <param name="fileDirectoruList"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        private void GetFileDirList(DirectoryInfo[] dirInfo, int level, ref List<FileDirectoryDto> fileDirectoruList)
        {
            foreach (var item in dirInfo)
            {
                oldLevel = level;
                var _cloneFileDirModel = new FileDirectoryDto { IsDirectory = 0 };
                _cloneFileDirModel.FileName = item.Name;
                _cloneFileDirModel.Level = level;
                _cloneFileDirModel.EntityPath = item.FullName.Split("Files")[1];
                _cloneFileDirModel.ParentName = item.Parent?.Name;

                if (item.GetDirectories().Length > 0)
                {
                    // 递归回调
                    GetFileDirList(item.GetDirectories(), ++oldLevel, ref fileDirectoruList);
                }
                fileDirectoruList.Add(_cloneFileDirModel);
                AddFiles(item, level, ref fileDirectoruList);
            }
        }

        /// <summary>
        /// 获取文件夹中的文件list
        /// </summary>
        /// <param name="directoryInfo"></param>
        /// <param name="level"></param>
        /// <param name="fileDirectoryModels"></param>
        private void AddFiles(DirectoryInfo directoryInfo, int level, ref List<FileDirectoryDto> fileDirectoryModels)
        {
            level++;
            var _fileDirModels = new List<FileDirectoryDto>();
            directoryInfo.GetFiles().ToList().ForEach(item =>
            {
                var _cloneFileDirModel = new FileDirectoryDto { IsDirectory = 1 };
                _cloneFileDirModel.Level = level;
                _cloneFileDirModel.ParentName = Directory.GetParent(item.FullName)?.Name;
                _cloneFileDirModel.EntityPath = item.FullName.Split("Files")[1];
                _cloneFileDirModel.FileName = item.Name;
                _fileDirModels.Add(_cloneFileDirModel);
            });
            if (_fileDirModels.Count > 0)
                fileDirectoryModels = fileDirectoryModels.Concat(_fileDirModels).ToList();
        }
    }
}