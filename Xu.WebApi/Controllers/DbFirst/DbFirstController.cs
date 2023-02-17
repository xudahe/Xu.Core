using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using SqlSugar;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xu.Common;
using Xu.IServices;
using Xu.Model;
using Xu.Model.ResultModel;

namespace Xu.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DbFirstController : ControllerBase
    {
        private readonly SqlSugarScope _sqlSugarClient;
        private readonly IWebHostEnvironment Env;
        private readonly ITableData _tableData;

        /// <summary>
        /// 构造函数
        /// </summary>
        public DbFirstController(ISqlSugarClient sqlSugarClient, IWebHostEnvironment env, ITableData tableData)
        {
            _sqlSugarClient = sqlSugarClient as SqlSugarScope;
            Env = env;
            _tableData = tableData;
        }

        /// <summary>
        /// 获取 整体框架 文件(主库)(一般可用第一次生成)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public MessageModel<string> GetFrameFiles()
        {
            var data = new MessageModel<string>() { Success = true, Message = "" };
            data.Response += @"file path is:C:\my-file\}";
            var isMuti = AppSettings.App(new string[] { "MutiDBEnabled" }).ToBoolReq();
            if (Env.IsDevelopment())
            {
                data.Response += $"Controller层生成：{FrameSeed.CreateControllers(_sqlSugarClient)} || ";

                BaseDBConfig.MutiConnectionString.allDbs.ToList().ForEach(m =>
                {
                    _sqlSugarClient.ChangeDatabase(m.ConnId.ToLower());
                    data.Response += $"库{m.ConnId}-Model层生成：{FrameSeed.CreateModels(_sqlSugarClient, m.ConnId, isMuti)} || ";
                    data.Response += $"库{m.ConnId}-IRepositorys层生成：{FrameSeed.CreateIRepositorys(_sqlSugarClient, m.ConnId, isMuti)} || ";
                    data.Response += $"库{m.ConnId}-IServices层生成：{FrameSeed.CreateIServices(_sqlSugarClient, m.ConnId, isMuti)} || ";
                    data.Response += $"库{m.ConnId}-Repository层生成：{FrameSeed.CreateRepository(_sqlSugarClient, m.ConnId, isMuti)} || ";
                    data.Response += $"库{m.ConnId}-Services层生成：{FrameSeed.CreateServices(_sqlSugarClient, m.ConnId, isMuti)} || ";
                });

                // 切回主库
                _sqlSugarClient.ChangeDatabase(MainDb.CurrentDbConnId.ToLower());
            }
            else
            {
                data.Success = false;
                data.Message = "当前不处于开发模式，代码生成不可用！";
            }

            return data;
        }

        /// <summary>
        /// 获取仓储层和服务层(需指定表名和数据库)
        /// </summary>
        /// <param name="ConnID">数据库链接名称</param>
        /// <param name="tableNames">需要生成的表名</param>
        /// <returns></returns>
        [HttpPost]
        public MessageModel<string> GetFrameFilesByTableNames([FromBody] string[] tableNames, [FromQuery] string ConnID)
        {
            ConnID ??= MainDb.CurrentDbConnId.ToLower();

            var isMuti = AppSettings.App(new string[] { "MutiDBEnabled" }).ToBoolReq();
            var data = new MessageModel<string>() { Success = true, Message = "" };
            if (Env.IsDevelopment())
            {
                data.Response += $"库{ConnID}-IRepositorys层生成：{FrameSeed.CreateIRepositorys(_sqlSugarClient, ConnID, isMuti, tableNames)} || ";
                data.Response += $"库{ConnID}-IServices层生成：{FrameSeed.CreateIServices(_sqlSugarClient, ConnID, isMuti, tableNames)} || ";
                data.Response += $"库{ConnID}-Repository层生成：{FrameSeed.CreateRepository(_sqlSugarClient, ConnID, isMuti, tableNames)} || ";
                data.Response += $"库{ConnID}-Services层生成：{FrameSeed.CreateServices(_sqlSugarClient, ConnID, isMuti, tableNames)} || ";
            }
            else
            {
                data.Success = false;
                data.Message = "当前不处于开发模式，代码生成不可用！";
            }

            return data;
        }

        /// <summary>
        /// 获取实体(需指定表名和数据库)
        /// </summary>
        /// <param name="ConnID">数据库链接名称</param>
        /// <param name="tableNames">需要生成的表名</param>
        /// <returns></returns>
        [HttpPost]
        public MessageModel<string> GetFrameFilesByTableNamesForEntity([FromBody] string[] tableNames, [FromQuery] string ConnID)
        {
            ConnID ??= MainDb.CurrentDbConnId.ToLower();

            var isMuti = AppSettings.App(new string[] { "MutiDBEnabled" }).ToBoolReq();
            var data = new MessageModel<string>() { Success = true, Message = "" };
            if (Env.IsDevelopment())
            {
                data.Response += $"库{ConnID}-Models层生成：{FrameSeed.CreateModels(_sqlSugarClient, ConnID, isMuti, tableNames)}";
            }
            else
            {
                data.Success = false;
                data.Message = "当前不处于开发模式，代码生成不可用！";
            }
            return data;
        }

        /// <summary>
        /// 获取控制器(需指定表名和数据库)
        /// </summary>
        /// <param name="ConnID">数据库链接名称</param>
        /// <param name="tableNames">需要生成的表名</param>
        /// <returns></returns>
        [HttpPost]
        public MessageModel<string> GetFrameFilesByTableNamesForController([FromBody] string[] tableNames, [FromQuery] string ConnID)
        {
            ConnID ??= MainDb.CurrentDbConnId.ToLower();

            var isMuti = AppSettings.App(new string[] { "MutiDBEnabled" }).ToBoolReq();
            var data = new MessageModel<string>() { Success = true, Message = "" };
            if (Env.IsDevelopment())
            {
                data.Response += $"库{ConnID}-Controllers层生成：{FrameSeed.CreateControllers(_sqlSugarClient, ConnID, isMuti, tableNames)}";
            }
            else
            {
                data.Success = false;
                data.Message = "当前不处于开发模式，代码生成不可用！";
            }
            return data;
        }

        /// <summary>
        /// DbFrist 根据数据库表名 生成整体框架,包含Model层(一般可用第一次生成)
        /// </summary>
        /// <param name="ConnID">数据库链接名称</param>
        /// <param name="tableNames">需要生成的表名</param>
        /// <returns></returns>
        [HttpPost]
        public MessageModel<string> GetAllFrameFilesByTableNames([FromBody] string[] tableNames, [FromQuery] string ConnID)
        {
            ConnID ??= MainDb.CurrentDbConnId.ToLower();

            var isMuti = AppSettings.App(new string[] { "MutiDBEnabled" }).ToBoolReq();
            var data = new MessageModel<string>() { Success = true, Message = "" };
            if (Env.IsDevelopment())
            {
                _sqlSugarClient.ChangeDatabase(ConnID.ToLower());
                data.Response += $"Controller层生成：{FrameSeed.CreateControllers(_sqlSugarClient, ConnID, isMuti, tableNames)} || ";
                data.Response += $"库{ConnID}-Model层生成：{FrameSeed.CreateModels(_sqlSugarClient, ConnID, isMuti, tableNames)} || ";
                data.Response += $"库{ConnID}-IRepositorys层生成：{FrameSeed.CreateIRepositorys(_sqlSugarClient, ConnID, isMuti, tableNames)} || ";
                data.Response += $"库{ConnID}-IServices层生成：{FrameSeed.CreateIServices(_sqlSugarClient, ConnID, isMuti, tableNames)} || ";
                data.Response += $"库{ConnID}-Repository层生成：{FrameSeed.CreateRepository(_sqlSugarClient, ConnID, isMuti, tableNames)} || ";
                data.Response += $"库{ConnID}-Services层生成：{FrameSeed.CreateServices(_sqlSugarClient, ConnID, isMuti, tableNames)} || ";
                // 切回主库
                _sqlSugarClient.ChangeDatabase(MainDb.CurrentDbConnId.ToLower());
            }
            else
            {
                data.Success = false;
                data.Message = "当前不处于开发模式，代码生成不可用！";
            }

            return data;
        }

        /// <summary>
        /// 数据库基础表数据导出到json文件
        /// </summary>
        /// <param name="tableName">指定表名称</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<object> GetDataFilesAsync(string tableName = "")
        {
            return new MessageModel<string>()
            {
                Success = true,
                Message = "数据导出成功",
                Response = await _tableData.ExportTable(tableName, Path.Combine(Env.WebRootPath, "dataJson"))
            };
        }
    }
}