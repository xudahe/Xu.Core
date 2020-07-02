using System;
using System.IO;

namespace Xu.Common
{
    public class BaseDBConfig
    {
        private static readonly string sqliteConnection = Appsettings.App(new string[] { "AppSettings", "Sqlite", "SqliteConnection" });
        private static readonly bool isSqliteEnabled = (Appsettings.App(new string[] { "AppSettings", "Sqlite", "Enabled" })).ToBoolOrDefault();

        private static readonly string sqlServerConnection = Appsettings.App(new string[] { "AppSettings", "SqlServer", "SqlServerConnection" });
        private static readonly bool isSqlServerEnabled = (Appsettings.App(new string[] { "AppSettings", "SqlServer", "Enabled" })).ToBoolOrDefault();

        private static readonly string mySqlConnection = Appsettings.App(new string[] { "AppSettings", "MySql", "MySqlConnection" });
        private static readonly bool isMySqlEnabled = (Appsettings.App(new string[] { "AppSettings", "MySql", "Enabled" })).ToBoolOrDefault();

        private static readonly string oracleConnection = Appsettings.App(new string[] { "AppSettings", "Oracle", "OracleConnection" });
        private static readonly bool IsOracleEnabled = (Appsettings.App(new string[] { "AppSettings", "Oracle", "Enabled" })).ToBoolOrDefault();

        public static string ConnectionString => InitConn();
        public static DataBaseType DbType = DataBaseType.SqlServer;

        private static string InitConn()
        {
            if (isSqliteEnabled)
            {
                DbType = DataBaseType.Sqlite;
                return $"DataSource=" + Path.Combine(Environment.CurrentDirectory, sqliteConnection);
            }
            else if (isSqlServerEnabled)
            {
                DbType = DataBaseType.SqlServer;
                return DifDBConnOfSecurity(@"D:\my-file\dbCountPsw1.txt", @"c:\my-file\dbCountPsw1.txt", sqlServerConnection);
            }
            else if (isMySqlEnabled)
            {
                DbType = DataBaseType.MySql;
                return DifDBConnOfSecurity(@"D:\my-file\dbCountPsw1_MySqlConn.txt", @"c:\my-file\dbCountPsw1_MySqlConn.txt", mySqlConnection);
            }
            else if (IsOracleEnabled)
            {
                DbType = DataBaseType.Oracle;
                return DifDBConnOfSecurity(@"D:\my-file\dbCountPsw1_OracleConn.txt", @"c:\my-file\dbCountPsw1_OracleConn.txt", oracleConnection);
            }
            else
            {
                return "server=.;uid=sa;pwd=sa;database=WMXuDB";
            }
        }

        private static string DifDBConnOfSecurity(params string[] conn)
        {
            foreach (var item in conn)
            {
                try
                {
                    if (File.Exists(item))
                    {
                        return File.ReadAllText(item).Trim();
                    }
                }
                catch (System.Exception) { }
            }

            return conn[^1];
        }
    }

    public enum DataBaseType
    {
        MySql = 0,
        SqlServer = 1,
        Sqlite = 2,
        Oracle = 3,
        PostgreSQL = 4
    }
}