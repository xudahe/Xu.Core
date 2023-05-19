using Newtonsoft.Json;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xu.Model.Models;

namespace Xu.Common
{
    public class DBSeed
    {
        private static string SeedDataFolder = "dataJson/{0}.json";

        /// <summary>
        /// 异步添加种子数据
        /// </summary>
        /// <param name="myContext"></param>
        /// <param name="WebRootPath"></param>
        /// <returns></returns>
        public static async Task SeedAsync(MyContext myContext, string WebRootPath)
        {
            try
            {
                if (string.IsNullOrEmpty(WebRootPath))
                {
                    throw new Exception("获取wwwroot路径时，异常！");
                }

                SeedDataFolder = Path.Combine(WebRootPath, SeedDataFolder);

                Console.WriteLine("************ WebApi DataBase Set *****************");
                Console.WriteLine($"Is multi-DataBase: {AppSettings.App(new string[] { "MutiDBEnabled" })}");
                Console.WriteLine($"Is CQRS: {AppSettings.App(new string[] { "CQRSEnabled" })}");
                Console.WriteLine();
                Console.WriteLine($"Master DB ConId: {MyContext.ConnId}");
                Console.WriteLine($"Master DB Type: {MyContext.DbType}");
                Console.WriteLine($"Master DB ConnectString: {MyContext.ConnectionString}");
                Console.WriteLine();
                if (AppSettings.App(new string[] { "MutiDBEnabled" }).ToBoolReq())
                {
                    var slaveIndex = 0;
                    BaseDBConfig.MutiConnectionString.allDbs.Where(x => x.ConnId != MainDb.CurrentDbConnId).ToList().ForEach(m =>
                    {
                        slaveIndex++;
                        Console.WriteLine($"Slave{slaveIndex} DB ID: {m.ConnId}");
                        Console.WriteLine($"Slave{slaveIndex} DB Type: {m.DbType}");
                        Console.WriteLine($"Slave{slaveIndex} DB ConnectString: {m.Connection}");
                        Console.WriteLine($"--------------------------------------");
                    });
                }
                else if (AppSettings.App(new string[] { "CQRSEnabled" }).ToBoolReq())
                {
                    var slaveIndex = 0;
                    BaseDBConfig.MutiConnectionString.slaveDbs.Where(x => x.ConnId != MainDb.CurrentDbConnId).ToList().ForEach(m =>
                    {
                        slaveIndex++;
                        Console.WriteLine($"Slave{slaveIndex} DB ID: {m.ConnId}");
                        Console.WriteLine($"Slave{slaveIndex} DB Type: {m.DbType}");
                        Console.WriteLine($"Slave{slaveIndex} DB ConnectString: {m.Connection}");
                        Console.WriteLine($"--------------------------------------");
                    });
                }
                else
                {
                }

                Console.WriteLine();

                // 创建数据库
                Console.WriteLine($"Create Database(The Db Id:{MyContext.ConnId})...");
                if (MyContext.DbType != SqlSugar.DbType.Oracle)
                {
                    myContext.Db.DbMaintenance.CreateDatabase();
                    ConsoleHelper.WriteSuccessLine($"Database created successfully!");
                }
                else
                {
                    //Oracle 数据库不支持该操作
                    ConsoleHelper.WriteSuccessLine($"Oracle 数据库不支持该操作，可手动创建Oracle数据库!");
                }

                // 创建数据库表，遍历指定命名空间下的class，注意不要把其他命名空间下的也添加进来。
                Console.WriteLine("Create Tables...");

                var path = AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory;
                var referencedAssemblies = System.IO.Directory.GetFiles(path, "Xu.Model.dll").Select(Assembly.LoadFrom).ToArray();
                var modelTypes = referencedAssemblies
                    .SelectMany(a => a.DefinedTypes)
                    .Select(type => type.AsType())
                    .Where(x => x.IsClass && x.Namespace is "Xu.Model.Models")
                    // .Where(s => !s.IsDefined(typeof(MultiTenantAttribute), false))
                    .ToList();
                modelTypes.ForEach(t =>
                {
                    // 这里只支持添加表，不支持删除
                    // 如果想要删除，数据库直接右键删除，或者联系SqlSugar作者；
                    if (!myContext.Db.DbMaintenance.IsAnyTable(t.Name))
                    {
                        Console.WriteLine(t.Name);
                        myContext.Db.CodeFirst.InitTables(t);
                    }
                });
                ConsoleHelper.WriteSuccessLine($"Tables created successfully!");
                Console.WriteLine();

                if (AppSettings.App(new string[] { "AppSettings", "SeedDBDataEnabled" }).ToBoolReq())
                {
                    JsonSerializerSettings setting = new JsonSerializerSettings();
                    JsonConvert.DefaultSettings = new Func<JsonSerializerSettings>(() =>
                    {
                        //日期类型默认格式化处理
                        setting.DateFormatHandling = DateFormatHandling.MicrosoftDateFormat;
                        setting.DateFormatString = "yyyy-MM-dd HH:mm:ss";

                        //空值处理
                        setting.NullValueHandling = NullValueHandling.Ignore;

                        //高级用法九中的Bool类型转换 设置
                        //setting.Converters.Add(new BoolConvert("是,否"));

                        return setting;
                    });

                    Console.WriteLine($"Seeding database data (The Db Id:{MyContext.ConnId})...");

                    #region User

                    if (!await myContext.Db.Queryable<User>().AnyAsync())
                    {
                        myContext.GetEntityDB<User>().InsertRange(JsonConvert.DeserializeObject<List<User>>(FileHelper.ReadFile(string.Format(SeedDataFolder, "User"), Encoding.UTF8)));
                        Console.WriteLine("Table:User created success!");
                    }
                    else
                    {
                        Console.WriteLine("Table:User already exists...");
                    }

                    #endregion User

                    #region Dept

                    if (!await myContext.Db.Queryable<Dept>().AnyAsync())
                    {
                        myContext.GetEntityDB<Dept>().InsertRange(JsonHelper.ParseFormByJson<List<Dept>>(FileHelper.ReadFile(string.Format(SeedDataFolder, "Dept"), Encoding.UTF8)));
                        Console.WriteLine("Table:Dept created success!");
                    }
                    else
                    {
                        Console.WriteLine("Table:Dept already exists...");
                    }

                    #endregion Dept

                    #region Role

                    if (!await myContext.Db.Queryable<Role>().AnyAsync())
                    {
                        myContext.GetEntityDB<Role>().InsertRange(JsonHelper.ParseFormByJson<List<Role>>(FileHelper.ReadFile(string.Format(SeedDataFolder, "Role"), Encoding.UTF8)));
                        Console.WriteLine("Table:Role created success!");
                    }
                    else
                    {
                        Console.WriteLine("Table:Role already exists...");
                    }

                    #endregion Role

                    #region Menu

                    if (!await myContext.Db.Queryable<Menu>().AnyAsync())
                    {
                        myContext.GetEntityDB<Menu>().InsertRange(JsonHelper.ParseFormByJson<List<Menu>>(FileHelper.ReadFile(string.Format(SeedDataFolder, "Menu"), Encoding.UTF8)));
                        Console.WriteLine("Table:Menu created success!");
                    }
                    else
                    {
                        Console.WriteLine("Table:Menu already exists...");
                    }

                    #endregion Menu

                    #region TasksQz

                    if (!await myContext.Db.Queryable<TasksQz>().AnyAsync())
                    {
                        myContext.GetEntityDB<TasksQz>().InsertRange(JsonHelper.ParseFormByJson<List<TasksQz>>(FileHelper.ReadFile(string.Format(SeedDataFolder, "TasksQz"), Encoding.UTF8)));
                        Console.WriteLine("Table:TasksQz created success!");
                    }
                    else
                    {
                        Console.WriteLine("Table:TasksQz already exists...");
                    }

                    #endregion TasksQz

                    #region Systems

                    if (!await myContext.Db.Queryable<Systems>().AnyAsync())
                    {
                        myContext.GetEntityDB<Systems>().InsertRange(JsonHelper.ParseFormByJson<List<Systems>>(FileHelper.ReadFile(string.Format(SeedDataFolder, "Systems"), Encoding.UTF8)));
                        Console.WriteLine("Table:Systems created success!");
                    }
                    else
                    {
                        Console.WriteLine("Table:Systems already exists...");
                    }

                    #endregion Systems

                    ConsoleHelper.WriteSuccessLine($"Done seeding database!");
                }

                Console.WriteLine();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}