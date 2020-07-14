﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using SqlSugar;
using System.Linq;
using Xu.Common;
using Xu.Model;
using Xu.Model.ResultModel;

namespace Xu.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Permissions.Name)]
    public class DbFirstController : ControllerBase
    {
        private readonly SqlSugarClient _sqlSugarClient;
        private readonly IWebHostEnvironment Env;

        /// <summary>
        /// 构造函数
        /// </summary>
        public DbFirstController(ISqlSugarClient sqlSugarClient, IWebHostEnvironment env)
        {
            _sqlSugarClient = sqlSugarClient as SqlSugarClient;
            Env = env;
        }

        /// <summary>
        /// 获取 整体框架 文件
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public object GetFrameFiles()
        {
            var data = new MessageModel<string>() { Success = true, Message = "" };
            data.Response += @"file path is:D:\my-file\}";
            var isMuti = Appsettings.App(new string[] { "MutiDBEnabled" }).ObjToBool();
            if (Env.IsDevelopment())
            {
                data.Response += $"Controller层生成：{FrameSeed.CreateControllers(_sqlSugarClient)} || ";

                BaseDBConfig.MutiConnectionString.Item1.ToList().ForEach(m =>
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
        /// 根据数据库表名 生成整体框架
        /// 仅针对通过CodeFirst生成表的情况
        /// </summary>
        /// <param name="ConnID">数据库链接名称</param>
        /// <param name="tableNames">需要生成的表名</param>
        /// <returns></returns>
        [HttpPost]
        public object GetFrameFilesByTableNames([FromBody] string[] tableNames, [FromQuery] string ConnID = null)
        {
            //ConnID = ConnID == null ? MainDb.CurrentDbConnId.ToLower() : ConnID;
            //ConnID = ConnID ?? MainDb.CurrentDbConnId.ToLower();
            ConnID ??= MainDb.CurrentDbConnId.ToLower();

            var isMuti = Appsettings.App(new string[] { "MutiDBEnabled" }).ObjToBool();
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
        /// DbFrist 根据数据库表名 生成整体框架,包含Model层
        /// </summary>
        /// <param name="ConnID">数据库链接名称</param>
        /// <param name="tableNames">需要生成的表名</param>
        /// <returns></returns>
        [HttpPost]
        public object GetAllFrameFilesByTableNames([FromBody] string[] tableNames, [FromQuery] string ConnID = null)
        {
            ConnID ??= MainDb.CurrentDbConnId.ToLower();

            var isMuti = Appsettings.App(new string[] { "MutiDBEnabled" }).ObjToBool();
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
    }
}