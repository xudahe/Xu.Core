using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xu.Common;
using Xu.IServices;
using Xu.Model.Models;

namespace Xu.Services
{
    public class TableData : ITableData
    {
        private readonly IUserSvc _userSvc;
        private readonly IDeptSvc _deptSvc;
        private readonly IRoleSvc _roleSvc;
        private readonly IMenuSvc _menuSvc;
        private readonly ISystemSvc _systemsSvc;
        private readonly IPlatformSvc _platformSvc;
        private readonly ITasksQzSvc _tasksQzSvc;

        public TableData(IUserSvc userSvc, IDeptSvc deptSvc, IRoleSvc roleSvc, IMenuSvc menuSvc, ISystemSvc systemsSvc, IPlatformSvc platformSvc, ITasksQzSvc tasksQzSvc)
        {
            _userSvc = userSvc;
            _deptSvc = deptSvc;
            _roleSvc = roleSvc;
            _menuSvc = menuSvc;
            _systemsSvc = systemsSvc;
            _platformSvc = platformSvc;
            _tasksQzSvc = tasksQzSvc;
        }

        /// <summary>
        /// 数据库基础表数据导出到json文件
        /// </summary>
        /// <param name="tableName">指定表名称</param>
        /// <param name="path">存放路径</param>
        /// <returns></returns>
        public async Task<string> ExportTable(string tableName, string path)
        {
            string outPaths = string.Empty;
            Dictionary<string, string> dic = new Dictionary<string, string>();

            switch (tableName.ToLower())
            {
                case "user":
                    dic.Add(typeof(User).Name, JsonHelper.JSON(await _userSvc.Query()));
                    break;

                case "dept":
                    dic.Add(typeof(Dept).Name, JsonHelper.JSON(await _deptSvc.Query()));
                    break;

                case "role":
                    dic.Add(typeof(Role).Name, JsonHelper.JSON(await _roleSvc.Query()));
                    break;

                case "menu":
                    dic.Add(typeof(Menu).Name, JsonHelper.JSON(await _menuSvc.Query()));
                    break;

                case "tasksqz":
                    dic.Add(typeof(TasksQz).Name, JsonHelper.JSON(await _tasksQzSvc.Query()));
                    break;

                case "systems":
                    dic.Add(typeof(Systems).Name, JsonHelper.JSON(await _systemsSvc.Query()));
                    break;

                case "platform":
                    dic.Add(typeof(Platform).Name, JsonHelper.JSON(await _platformSvc.Query()));
                    break;

                default:
                    dic.Add(typeof(User).Name, JsonHelper.JSON(await _userSvc.Query()));
                    dic.Add(typeof(Dept).Name, JsonHelper.JSON(await _deptSvc.Query()));
                    dic.Add(typeof(Role).Name, JsonHelper.JSON(await _roleSvc.Query()));
                    dic.Add(typeof(Menu).Name, JsonHelper.JSON(await _menuSvc.Query()));
                    dic.Add(typeof(Systems).Name, JsonHelper.JSON(await _systemsSvc.Query()));
                    dic.Add(typeof(Platform).Name, JsonHelper.JSON(await _platformSvc.Query()));
                    dic.Add(typeof(TasksQz).Name, JsonHelper.JSON(await _tasksQzSvc.Query()));
                    break;
            }

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            foreach (var item in dic)
            {
                string filePath = Path.Combine(path, $@"{item.Key}.json");

                using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    fs.SetLength(0); //清空文件内容
                    using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                    {
                        sw.WriteLine(item.Value);
                    }
                }

                outPaths += $"表{item.Key}-数据生成：{filePath} || ";
            }
            return outPaths[0..^4];
        }
    }
}