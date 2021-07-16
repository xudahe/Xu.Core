using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Xu.Common
{
    public class LogLock
    {
        private static readonly ReaderWriterLockSlim LogWriteLock = new ReaderWriterLockSlim();
        private static string _contentRoot = string.Empty;

        public LogLock(string contentPath)
        {
            _contentRoot = contentPath;
        }

        /// <summary>
        /// 日志输出
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="dataParas"></param>
        /// <param name="IsHeader"></param>
        public static void OutSql2Log(string filename, string[] dataParas, bool IsHeader = true)
        {
            try
            {
                //设置读写锁为写入模式独占资源，其他写入请求需要等待本次写入结束之后才能继续写入
                //注意：长时间持有读线程锁或写线程锁会使其他线程发生饥饿 (starve)。 为了得到最好的性能，需要考虑重新构造应用程序以将写访问的持续时间减少到最小。
                //      从性能方面考虑，请求进入写入模式应该紧跟文件操作之前，在此处进入写入模式仅是为了降低代码复杂度
                //      因进入与退出写入模式应在同一个try finally语句块内，所以在请求进入写入模式之前不能触发异常，否则释放次数大于请求次数将会触发异常
                LogWriteLock.EnterWriteLock();

                var path = Path.Combine(_contentRoot, "Log", DateTime.Now.ToString("yyyyMMdd"));
                if (!Directory.Exists(path))//如果路径不存在
                {
                    Directory.CreateDirectory(path);//创建一个路径的文件夹
                }

                string logFilePath = Path.Combine(path, $@"{filename}.log");

                string logContent = string.Join("\r\n", dataParas);
                if (IsHeader)
                {
                    logContent = (
                       "--------------------------------\r\n" +
                       DateTime.Now + "|\r\n" +
                       string.Join("\r\n", dataParas) + "\r\n"
                       );
                }

                File.AppendAllText(logFilePath, logContent);
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
            }
            finally
            {
                //退出写入模式，释放资源占用
                //注意：一次请求对应一次释放
                //      若释放次数大于请求次数将会触发异常[写入锁定未经保持即被释放]
                //      若请求处理完成后未释放将会触发异常[此模式不下允许以递归方式获取写入锁定]
                LogWriteLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// 读取log文件内容
        /// </summary>
        /// <param name="Path"></param>
        /// <param name="encode"></param>
        /// <returns></returns>
        public static string ReadLog(string Path, Encoding encode)
        {
            string s = "";
            try
            {
                LogWriteLock.EnterReadLock();

                if (!File.Exists(Path))
                {
                    s = null;
                }
                else
                {
                    StreamReader f2 = new StreamReader(Path, encode);
                    s = f2.ReadToEnd();
                    f2.Close();
                    f2.Dispose();
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                LogWriteLock.ExitReadLock();
            }
            return s;
        }

        /// <summary>
        /// 获取所有日志
        /// </summary>
        /// <param name="num">前N条数据</param>
        /// <returns></returns>
        public static List<LogInfo> GetLogData(int num = 100)
        {
            List<LogInfo> aopLogs = GetAopLogByDate();
            List<LogInfo> excLogs = GetExcepLogByDate();
            List<LogInfo> sqlLogs = GetSqlLogByDate();
            List<LogInfo> reqLogs = GetIpLogByDate();

            if (excLogs != null)
            {
                aopLogs.AddRange(excLogs);
            }
            if (sqlLogs != null)
            {
                aopLogs.AddRange(sqlLogs);
            }
            if (reqLogs != null)
            {
                aopLogs.AddRange(reqLogs);
            }
            aopLogs = aopLogs.OrderByDescending(d => d.Import).ThenByDescending(d => d.Datetime).Take(num).ToList();

            return aopLogs;
        }

        /// <summary>
        /// 服务层请求响应AOP日志
        /// </summary>
        /// <returns></returns>
        public static List<LogInfo> GetAopLogByDate()
        {
            List<LogInfo> aopLogs = new List<LogInfo>();

            try
            {
                var aoplogContent = ReadLog(Path.Combine(_contentRoot, "Log", DateTime.Now.ToString("yyyyMMdd"), "AOPLog.log"), Encoding.UTF8);

                if (!string.IsNullOrEmpty(aoplogContent))
                {
                    aopLogs = aoplogContent.Split("--------------------------------")
                                           .Where(d => !string.IsNullOrEmpty(d) && d != "\n" && d != "\r\n")
                                           .Select(d => new LogInfo
                                           {
                                               Datetime = d.Split("|")[0].ToDateTimeReq(),
                                               Content = d.Split("|")[1]?.Replace("\r\n", "<br>"),
                                               LogColor = "AOP",
                                           }).ToList();
                }
            }
            catch (Exception) { }

            return aopLogs;
        }

        /// <summary>
        /// Api请求响应日志
        /// </summary>
        /// <returns></returns>
        public static List<LogInfo> GetResponseLogByDate()
        {
            List<LogInfo> resLogs = new List<LogInfo>();

            try
            {
                var exclogContent = ReadLog(Path.Combine(_contentRoot, "Log", DateTime.Now.ToString("yyyyMMdd"), "RequestResponseLog.log"), Encoding.UTF8);

                resLogs = exclogContent.Split("--------------------------------")
                                       .Where(d => !string.IsNullOrEmpty(d) && d != "\n" && d != "\r\n")
                                       .Select(d => new LogInfo
                                       {
                                           Datetime = d.Split("|")[0].ToDateTimeReq(),
                                           Content = d.Split("|")[1]?.Replace("\r\n", "<br>"),
                                           LogColor = "RRS",
                                       }).ToList();
            }
            catch (Exception) { }

            return resLogs;
        }

        /// <summary>
        /// 全局异常日志
        /// </summary>
        /// <returns></returns>
        public static List<LogInfo> GetExcepLogByDate()
        {
            List<LogInfo> excLogs = new List<LogInfo>();

            try
            {
                var exclogContent = ReadLog(Path.Combine(_contentRoot, "Log", $"GlobalExcepLogs_{DateTime.Now:yyyMMdd}.log"), Encoding.GetEncoding("gb2312"));

                if (!string.IsNullOrEmpty(exclogContent))
                {
                    excLogs = exclogContent.Split("--------------------------------")
                                 .Where(d => !string.IsNullOrEmpty(d) && d != "\n" && d != "\r\n")
                                 .Select(d => new LogInfo
                                 {
                                     Datetime = (d.Split("|")[0]).Split(',')[0].Split('：')[1].ToDateTimeReq(),
                                     Content = d.Split("|")[1]?.Replace("\r\n", "<br>"),
                                     LogColor = "EXC",
                                     Import = 9,
                                 }).ToList();
                }
            }
            catch (Exception) { }

            return excLogs;
        }

        /// <summary>
        /// Sql数据库操作日志
        /// </summary>
        /// <returns></returns>
        public static List<LogInfo> GetSqlLogByDate()
        {
            List<LogInfo> sqlLogs = new List<LogInfo>();

            try
            {
                var sqllogContent = ReadLog(Path.Combine(_contentRoot, "Log", DateTime.Now.ToString("yyyyMMdd"), "SqlLog.log"), Encoding.UTF8);

                if (!string.IsNullOrEmpty(sqllogContent))
                {
                    sqlLogs = sqllogContent.Split("--------------------------------")
                                           .Where(d => !string.IsNullOrEmpty(d) && d != "\n" && d != "\r\n")
                                           .Select(d => new LogInfo
                                           {
                                               Datetime = d.Split("|")[0].ToDateTimeReq(),
                                               Content = d.Split("|")[1]?.Replace("\r\n", "<br>"),
                                               LogColor = "SQL",
                                           }).ToList();
                }
            }
            catch (Exception) { }

            return sqlLogs;
        }

        /// <summary>
        /// IP请求日志
        /// </summary>
        /// <returns></returns>
        public static List<LogInfo> GetIpLogByDate()
        {
            List<LogInfo> reqLogs = new List<LogInfo>();

            try
            {
                var s = ReadLog(Path.Combine(_contentRoot, "Log", DateTime.Now.ToString("yyyyMMdd"), "RequestIpInfoLog.log"), Encoding.UTF8);
                var Logs = JsonConvert.DeserializeObject<List<RequestInfo>>("[" + ReadLog(Path.Combine(_contentRoot, "Log", DateTime.Now.ToString("yyyyMMdd"), "RequestIpInfoLog.log"), Encoding.UTF8) + "]");

                Logs = Logs.Where(d => d.Datetime.ToDateTimeReq() >= DateTime.Today).ToList();

                reqLogs = Logs.Select(d => new LogInfo
                {
                    Datetime = d.Datetime.ToDateTimeReq(),
                    Content = $"IP:{d.ClientIP}<br>{d.Url}",
                    ClientIP = d.ClientIP,
                    LogColor = "Req",
                }).ToList();
            }
            catch (Exception) { }

            return reqLogs;
        }

        /// <summary>
        /// Api请求访问日志
        /// </summary>
        /// <returns></returns>
        public static List<UserAccessModel> GetAccessLogByDate()
        {
            List<UserAccessModel> aceLogs = new List<UserAccessModel>();

            try
            {
                var Logs = JsonConvert.DeserializeObject<List<UserAccessModel>>("[" + LogLock.ReadLog(Path.Combine(_contentRoot, "Log", DateTime.Now.ToString("yyyyMMdd"), "RecordAccessLogs.log"), Encoding.UTF8) + "]");

                aceLogs = Logs.Where(d => d.BeginTime.ToDateTime() >= DateTime.Today).OrderByDescending(d => d.BeginTime).Take(50).ToList();
            }
            catch (Exception) { }

            return aceLogs;
        }

        public static List<string> GetFileList(string type = "")
        {
            List<string> filList = new List<string>();//定义list变量，存放获取到的路径
            List<string> diiList = new List<string>();

            DirectoryInfo dir = new DirectoryInfo(Path.Combine(_contentRoot, "Log"));
            FileInfo[] fil = dir.GetFiles();  // 获取子文件夹内的文件列表
            DirectoryInfo[] dii = dir.GetDirectories(); // 获取子文件夹内的文件夹列表

            filList.Clear();
            diiList.Clear();
            foreach (FileInfo f in fil)
            {
                filList.Add(f.FullName);//添加文件的路径到列表
            }
            foreach (DirectoryInfo d in dii)
            {
                diiList.Add(d.FullName);//添加文件夹的路径到列表
            }

            if (type == "fil")
                return filList;
            else if (type == "dii")
                return diiList;
            else
                return filList.Union(diiList).ToList();
        }
    }
}