using SqlSugar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Xu.Model
{
    public class FrameSeed
    {
        /// <summary>
        /// 生成Controller层
        /// </summary>
        /// <param name="sqlSugarClient">sqlsugar实例</param>
        /// <param name="ConnId">数据库链接ID</param>
        /// <param name="tableNames">数据库表名数组，默认空，生成所有表</param>
        /// <param name="isMuti"></param>
        /// <returns></returns>
        public static bool CreateControllers(SqlSugarClient sqlSugarClient, string ConnId = null, bool isMuti = false, string[] tableNames = null)
        {
            try
            {
                Create_Controller_ClassFileByDBTalbe(sqlSugarClient, ConnId, $@"d:\my-file\Xu.WebApi.Controllers", "Xu.WebApi.Controllers", tableNames, "", isMuti);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 生成Model层
        /// </summary>
        /// <param name="sqlSugarClient">sqlsugar实例</param>
        /// <param name="ConnId">数据库链接ID</param>
        /// <param name="tableNames">数据库表名数组，默认空，生成所有表</param>
        /// <param name="isMuti"></param>
        /// <returns></returns>
        public static bool CreateModels(SqlSugarClient sqlSugarClient, string ConnId, bool isMuti = false, string[] tableNames = null)
        {
            try
            {
                Create_Model_ClassFileByDBTalbe(sqlSugarClient, ConnId, $@"d:\my-file\Xu.Model", "Xu.Model.Models", tableNames, "", isMuti);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 生成IRepository层
        /// </summary>
        /// <param name="sqlSugarClient">sqlsugar实例</param>
        /// <param name="ConnId">数据库链接ID</param>
        /// <param name="isMuti"></param>
        /// <param name="tableNames">数据库表名数组，默认空，生成所有表</param>
        /// <returns></returns>
        public static bool CreateIRepositorys(SqlSugarClient sqlSugarClient, string ConnId, bool isMuti = false, string[] tableNames = null)
        {
            try
            {
                Create_IRepository_ClassFileByDBTalbe(sqlSugarClient, ConnId, $@"d:\my-file\Xu.IRepository", "Xu.IRepository", tableNames, "", isMuti);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 生成 IService 层
        /// </summary>
        /// <param name="sqlSugarClient">sqlsugar实例</param>
        /// <param name="ConnId">数据库链接ID</param>
        /// <param name="isMuti"></param>
        /// <param name="tableNames">数据库表名数组，默认空，生成所有表</param>
        /// <returns></returns>
        public static bool CreateIServices(SqlSugarClient sqlSugarClient, string ConnId, bool isMuti = false, string[] tableNames = null)
        {
            try
            {
                Create_IServices_ClassFileByDBTalbe(sqlSugarClient, ConnId, $@"d:\my-file\Xu.IServices", "Xu.IServices", tableNames, "", isMuti);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 生成 Repository 层
        /// </summary>
        /// <param name="sqlSugarClient">sqlsugar实例</param>
        /// <param name="ConnId">数据库链接ID</param>
        /// <param name="isMuti"></param>
        /// <param name="tableNames">数据库表名数组，默认空，生成所有表</param>
        /// <returns></returns>
        public static bool CreateRepository(SqlSugarClient sqlSugarClient, string ConnId, bool isMuti = false, string[] tableNames = null)
        {
            try
            {
                Create_Repository_ClassFileByDBTalbe(sqlSugarClient, ConnId, $@"d:\my-file\Xu.Repository", "Xu.Repository", tableNames, "", isMuti);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 生成 Service 层
        /// </summary>
        /// <param name="sqlSugarClient">sqlsugar实例</param>
        /// <param name="ConnId">数据库链接ID</param>
        /// <param name="isMuti"></param>
        /// <param name="tableNames">数据库表名数组，默认空，生成所有表</param>
        /// <returns></returns>
        public static bool CreateServices(SqlSugarClient sqlSugarClient, string ConnId, bool isMuti = false, string[] tableNames = null)
        {
            try
            {
                Create_Services_ClassFileByDBTalbe(sqlSugarClient, ConnId, $@"d:\my-file\Xu.Services", "Xu.Services", tableNames, "", isMuti);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #region 根据数据库表生产Controller层

        /// <summary>
        /// 功能描述:根据数据库表生产Controller层
        /// </summary>
        /// <param name="sqlSugarClient"></param>
        /// <param name="ConnId">数据库链接ID</param>
        /// <param name="strPath">实体类存放路径</param>
        /// <param name="strNameSpace">命名空间</param>
        /// <param name="lstTableNames">生产指定的表</param>
        /// <param name="strInterface">实现接口</param>
        /// <param name="isMuti"></param>
        /// <param name="blnSerializable">是否序列化</param>
        private static void Create_Controller_ClassFileByDBTalbe(
          SqlSugarClient sqlSugarClient,
          string ConnId,
          string strPath,
          string strNameSpace,
          string[] lstTableNames,
          string strInterface,
          bool isMuti = false,
          bool blnSerializable = false)
        {
            var IDbFirst = sqlSugarClient.DbFirst;
            if (lstTableNames != null && lstTableNames.Length > 0)
            {
                IDbFirst = IDbFirst.Where(lstTableNames);
            }
            var ls = IDbFirst.IsCreateDefaultValue().IsCreateAttribute()

                 .SettingClassTemplate(p => p =
@"using Xu.IServices;
using Xu.Common;
using Xu.Model;
using Xu.Model.Models;
using Xu.Model.ResultModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace " + strNameSpace + @"
{
	[Route(""api/[controller]/[action]"")]
	[ApiController]
    [Authorize(Permissions.Name)]
     public class {ClassName}Controller : ControllerBase
        {
             /// <summary>
             /// 服务器接口，因为是模板生成，所以首字母是大写的，自己可以重构下
             /// </summary>
            private readonly I{ClassName}Svc _{ClassName}Svc;

            public {ClassName}Controller(I{ClassName}Svc {ClassName}Svc)
            {
                _{ClassName}Svc = {ClassName}Svc;
            }

            [HttpGet]
            public async Task<object> Get(int page = 1, int pageSize = 50, string key = """")
            {
                Expression<Func<{ClassName}, bool>> whereExpression = a => a.id > 0;

                return new MessageModel<PageModel<{ClassName}>>()
                {
                    Message = ""获取成功"",
                    Success = true,
                    Response = await _{ClassName}Svc.QueryPage(whereExpression, page, pageSize)
                };
    }

    [HttpGet(""{id}"")]
    public async Task<object> Get(int id = 0)
    {
        return new MessageModel<{ClassName}>()
        {
            Message = ""获取成功"",
            Success = true,
            Response = await _{ClassName}Svc.QueryById(id)
        };
    }

    [HttpPost]
    public async Task<object> Post([FromBody] {ClassName} request)
    {
        var data = new MessageModel<string>();

        var id = await _{ClassName}Svc.Add(request);
        data.Success = id > 0;

        if (data.Success)
        {
            data.Response = id.ToString();
            data.Message = ""添加成功"";
        }

        return data;
    }

    [HttpPut]
    public async Task<object> Put([FromBody] {ClassName} request)
    {
        var data = new MessageModel<string>();
        if (request.Id > 0)
        {
            data.Success = await _{ClassName}Svc.Update(request);
            if (data.Success)
            {
                data.Message = ""更新成功"";
                data.Response = request?.Id.ToString();
            }
        }

        return data;
    }

    [HttpDelete(""{id}"")]
    public async Task<object> Delete(int id = 0)
    {
        var data = new MessageModel<string>();
        if (id > 0)
        {
            var detail = await _{ClassName}Svc.QueryById(id);

            detail.DeleteTime = DateTime.Now;

                if (detail != null)
                {
                    data.Success = await _{ClassName}Svc.Update(detail);
                    if (data.Success)
                    {
                        data.Message = ""删除成功"";
                        data.Response = detail?.Id.ToString();
                    }
                }
        }

        return data;
    }
}
}")

                  .ToClassStringList(strNameSpace);

            Dictionary<string, string> newdic = new Dictionary<string, string>();
            //循环处理 首字母小写 并插入新的 Dictionary
            foreach (KeyValuePair<string, string> item in ls)
            {
                string newkey = "_" + item.Key.First().ToString().ToLower() + item.Key.Substring(1);
                string newvalue = item.Value.Replace("_" + item.Key, newkey);
                newdic.Add(item.Key, newvalue);
            }
            CreateFilesByClassStringList(newdic, strPath, "{0}Controller");
        }

        #endregion 根据数据库表生产Controller层

        #region 根据数据库表生产Model层

        /// <summary>
        /// 功能描述:根据数据库表生产Model层
        /// </summary>
        /// <param name="sqlSugarClient"></param>
        /// <param name="ConnId">数据库链接ID</param>
        /// <param name="strPath">实体类存放路径</param>
        /// <param name="strNameSpace">命名空间</param>
        /// <param name="lstTableNames">生产指定的表</param>
        /// <param name="strInterface">实现接口</param>
        /// <param name="isMuti"></param>
        /// <param name="blnSerializable">是否序列化</param>
        private static void Create_Model_ClassFileByDBTalbe(
          SqlSugarClient sqlSugarClient,
          string ConnId,
          string strPath,
          string strNameSpace,
          string[] lstTableNames,
          string strInterface,
          bool isMuti = false,
          bool blnSerializable = false)
        {
            //多库文件分离
            if (isMuti)
            {
                strPath = strPath + @"\Models\" + ConnId;
                strNameSpace = strNameSpace + "." + ConnId;
            }

            var IDbFirst = sqlSugarClient.DbFirst;
            if (lstTableNames != null && lstTableNames.Length > 0)
            {
                IDbFirst = IDbFirst.Where(lstTableNames);
            }
            var ls = IDbFirst.IsCreateDefaultValue().IsCreateAttribute()

                  .SettingClassTemplate(p => p =
@"{using}

namespace " + strNameSpace + @"
{
{ClassDescription}
    [SugarTable( ""{ClassName}"", """ + ConnId + @""")]" + (blnSerializable ? "\n    [Serializable]" : "") + @"
    public class {ClassName}" + (string.IsNullOrEmpty(strInterface) ? "" : (" : " + strInterface)) + @"
    {
        public {ClassName}()
        {
        }
        {PropertyName}
    }
}")
                  .SettingPropertyDescriptionTemplate(p => p = string.Empty)
                  .SettingPropertyTemplate(p => p =
@"{SugarColumn}
           public {PropertyType} {PropertyName} { get; set; }")

                   //.SettingConstructorTemplate(p => p = "              this._{PropertyName} ={DefaultValue};")

                   .ToClassStringList(strNameSpace);
            CreateFilesByClassStringList(ls, strPath, "{0}");
        }

        #endregion 根据数据库表生产Model层

        #region 根据数据库表生产IRepository层

        /// <summary>
        /// 功能描述:根据数据库表生产IRepository层
        /// </summary>
        /// <param name="sqlSugarClient"></param>
        /// <param name="ConnId">数据库链接ID</param>
        /// <param name="strPath">实体类存放路径</param>
        /// <param name="strNameSpace">命名空间</param>
        /// <param name="lstTableNames">生产指定的表</param>
        /// <param name="strInterface">实现接口</param>
        /// <param name="isMuti"></param>
        private static void Create_IRepository_ClassFileByDBTalbe(
          SqlSugarClient sqlSugarClient,
          string ConnId,
          string strPath,
          string strNameSpace,
          string[] lstTableNames,
          string strInterface,
          bool isMuti = false
            )
        {
            //多库文件分离
            if (isMuti)
            {
                strPath = strPath + @"\" + ConnId;
                strNameSpace = strNameSpace + "." + ConnId;
            }

            var IDbFirst = sqlSugarClient.DbFirst;
            if (lstTableNames != null && lstTableNames.Length > 0)
            {
                IDbFirst = IDbFirst.Where(lstTableNames);
            }
            var ls = IDbFirst.IsCreateDefaultValue().IsCreateAttribute()

                 .SettingClassTemplate(p => p =
@"using Xu.IRepository;
using Xu.Model.Models" + (isMuti ? "." + ConnId + "" : "") + @";

namespace " + strNameSpace + @"
{
	/// <summary>
	/// I{ClassName}Repository
	/// </summary>
    public interface I{ClassName}Repo : IBaseRepo<{ClassName}>" + (string.IsNullOrEmpty(strInterface) ? "" : (" , " + strInterface)) + @"
    {
    }
}")

                  .ToClassStringList(strNameSpace);
            CreateFilesByClassStringList(ls, strPath, "I{0}Repo");
        }

        #endregion 根据数据库表生产IRepository层

        #region 根据数据库表生产IServices层

        /// <summary>
        /// 功能描述:根据数据库表生产IServices层
        /// </summary>
        /// <param name="sqlSugarClient"></param>
        /// <param name="ConnId">数据库链接ID</param>
        /// <param name="strPath">实体类存放路径</param>
        /// <param name="strNameSpace">命名空间</param>
        /// <param name="lstTableNames">生产指定的表</param>
        /// <param name="strInterface">实现接口</param>
        /// <param name="isMuti"></param>
        private static void Create_IServices_ClassFileByDBTalbe(
          SqlSugarClient sqlSugarClient,
          string ConnId,
          string strPath,
          string strNameSpace,
          string[] lstTableNames,
          string strInterface,
          bool isMuti = false)
        {
            //多库文件分离
            if (isMuti)
            {
                strPath = strPath + @"\" + ConnId;
                strNameSpace = strNameSpace + "." + ConnId;
            }

            var IDbFirst = sqlSugarClient.DbFirst;
            if (lstTableNames != null && lstTableNames.Length > 0)
            {
                IDbFirst = IDbFirst.Where(lstTableNames);
            }
            var ls = IDbFirst.IsCreateDefaultValue().IsCreateAttribute()

                  .SettingClassTemplate(p => p =
@"using Xu.IServices;
using Xu.Model.Models" + (isMuti ? "." + ConnId + "" : "") + @";

namespace " + strNameSpace + @"
{
	/// <summary>
	/// I{ClassName}Services
	/// </summary>
    public interface I{ClassName}Svc :IBaseSvc<{ClassName}>" + (string.IsNullOrEmpty(strInterface) ? "" : (" , " + strInterface)) + @"
	{
    }
}")

                   .ToClassStringList(strNameSpace);
            CreateFilesByClassStringList(ls, strPath, "I{0}Svc");
        }

        #endregion 根据数据库表生产IServices层

        #region 根据数据库表生产 Repository 层

        /// <summary>
        /// 功能描述:根据数据库表生产 Repository 层
        /// </summary>
        /// <param name="sqlSugarClient"></param>
        /// <param name="ConnId">数据库链接ID</param>
        /// <param name="strPath">实体类存放路径</param>
        /// <param name="strNameSpace">命名空间</param>
        /// <param name="lstTableNames">生产指定的表</param>
        /// <param name="strInterface">实现接口</param>
        /// <param name="isMuti"></param>
        private static void Create_Repository_ClassFileByDBTalbe(
          SqlSugarClient sqlSugarClient,
          string ConnId,
          string strPath,
          string strNameSpace,
          string[] lstTableNames,
          string strInterface,
          bool isMuti = false)
        {
            //多库文件分离
            if (isMuti)
            {
                strPath = strPath + @"\" + ConnId;
                strNameSpace = strNameSpace + "." + ConnId;
            }

            var IDbFirst = sqlSugarClient.DbFirst;
            if (lstTableNames != null && lstTableNames.Length > 0)
            {
                IDbFirst = IDbFirst.Where(lstTableNames);
            }
            var ls = IDbFirst.IsCreateDefaultValue().IsCreateAttribute()

                  .SettingClassTemplate(p => p =
@"using Xu.IRepository" + (isMuti ? "." + ConnId + "" : "") + @";
using Xu.Model.Models" + (isMuti ? "." + ConnId + "" : "") + @";

namespace " + strNameSpace + @"
{
	/// <summary>
	/// {ClassName}Repository
	/// </summary>
    public class {ClassName}Repo : BaseRepo<{ClassName}>, I{ClassName}Repo" + (string.IsNullOrEmpty(strInterface) ? "" : (" , " + strInterface)) + @"
    {
        public {ClassName}Repo(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}")
                  .ToClassStringList(strNameSpace);

            CreateFilesByClassStringList(ls, strPath, "{0}Repo");
        }

        #endregion 根据数据库表生产 Repository 层

        #region 根据数据库表生产 Services 层

        /// <summary>
        /// 功能描述:根据数据库表生产 Services 层
        /// </summary>
        /// <param name="sqlSugarClient"></param>
        /// <param name="ConnId">数据库链接ID</param>
        /// <param name="strPath">实体类存放路径</param>
        /// <param name="strNameSpace">命名空间</param>
        /// <param name="lstTableNames">生产指定的表</param>
        /// <param name="strInterface">实现接口</param>
        /// <param name="isMuti"></param>
        private static void Create_Services_ClassFileByDBTalbe(
          SqlSugarClient sqlSugarClient,
          string ConnId,
          string strPath,
          string strNameSpace,
          string[] lstTableNames,
          string strInterface,
          bool isMuti = false)
        {
            //多库文件分离
            if (isMuti)
            {
                strPath = strPath + @"\" + ConnId;
                strNameSpace = strNameSpace + "." + ConnId;
            }

            var IDbFirst = sqlSugarClient.DbFirst;
            if (lstTableNames != null && lstTableNames.Length > 0)
            {
                IDbFirst = IDbFirst.Where(lstTableNames);
            }
            var ls = IDbFirst.IsCreateDefaultValue().IsCreateAttribute()

                  .SettingClassTemplate(p => p =
@"using Xu.IRepository" + (isMuti ? "." + ConnId + "" : "") + @";
using Xu.IServices" + (isMuti ? "." + ConnId + "" : "") + @";
using Xu.Model.Models" + (isMuti ? "." + ConnId + "" : "") + @";
using Xu.Services;

namespace " + strNameSpace + @"
{
    /// <summary>
	/// I{ClassName}Services
	/// </summary>
    public partial class {ClassName}Svc : BaseSvc<{ClassName}>, I{ClassName}Svc" + (string.IsNullOrEmpty(strInterface) ? "" : (" , " + strInterface)) + @"
    {
        private readonly IBaseRepo<{ClassName}> _dal;
        public {ClassName}Svc(IBaseRepo<{ClassName}> dal)
        {
            this._dal = dal;
            base.BaseDal = dal;
        }
    }
}")
                  .ToClassStringList(strNameSpace);

            CreateFilesByClassStringList(ls, strPath, "{0}Svc");
        }

        #endregion 根据数据库表生产 Services 层

        #region 根据模板内容批量生成文件

        /// <summary>
        /// 根据模板内容批量生成文件
        /// </summary>
        /// <param name="ls">类文件字符串list</param>
        /// <param name="strPath">生成路径</param>
        /// <param name="fileNameTp">文件名格式模板</param>
        private static void CreateFilesByClassStringList(Dictionary<string, string> ls, string strPath, string fileNameTp)
        {
            foreach (var item in ls)
            {
                var fileName = $"{string.Format(fileNameTp, item.Key)}.cs";
                var fileFullPath = Path.Combine(strPath, fileName);
                if (!Directory.Exists(strPath))
                {
                    Directory.CreateDirectory(strPath);
                }
                File.WriteAllText(fileFullPath, item.Value);
            }
        }

        #endregion 根据模板内容批量生成文件
    }
}