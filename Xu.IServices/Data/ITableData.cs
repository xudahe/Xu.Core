using System.Threading.Tasks;

namespace Xu.IServices
{
    public interface ITableData
    {
        /// <summary>
        /// 数据库基础表数据导出到json文件
        /// </summary>
        /// <param name="tableName">指定表名称</param>
        /// <param name="path">存放路径</param>
        /// <returns></returns>
        Task<string> ExportTable(string tableName, string path);
    }
}