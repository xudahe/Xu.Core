using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xu.Common;

namespace Xu.Model
{
    public class DBSeed
    {
        // 这是我的在线demo数据，比较多，且杂乱
        // gitee 源数据
        //private readonly static string GitJsonFileFormat = "https://gitee.com/laozhangIsPhi/Xu.Data.Share/raw/master/XuCore.Data.json/{0}.tsv";

        // 这里我把重要的权限数据提出来的精简版，默认一个Admin_Role + 一个管理员用户，
        // 然后就是菜单+接口+权限分配，注意没有其他博客信息了，下边seeddata 的时候，删掉即可。

        // gitee 源数据
        //private readonly static string GitJsonFileFormat2 = "https://gitee.com/laozhangIsPhi/Xu.Data.Share/tree/master/Student.Achieve.json/{0}.tsv";

        /// <summary>
        /// 异步添加种子数据
        /// </summary>
        /// <param name="myContext"></param>
        /// <returns></returns>
        public static async Task SeedAsync(MyContext myContext)
        {
            try
            {
                // 创建数据库
                //SqlSugar 5.x 版本已经可以支持自动创建数据库了，可以不用手动创建数据库，反之需要手动创建空的数据库。
                Console.WriteLine($"Create Database...");
                myContext.Db.DbMaintenance.CreateDatabase();

                ConsoleHelper.WriteSuccessLine($"Database created successfully!");

                // 创建数据库表，遍历指定命名空间下的class，
                // 注意不要把其他命名空间下的也添加进来。
                Console.WriteLine("Create Tables...");
                var modelTypes = from t in Assembly.GetExecutingAssembly().GetTypes()
                                 where t.IsClass && t.Namespace == "Xu.Model.Models"
                                 select t;
                modelTypes.ToList().ForEach(t =>
                {
                    if (!myContext.Db.DbMaintenance.IsAnyTable(t.Name))
                    {
                        Console.WriteLine(t.Name);
                        myContext.Db.CodeFirst.InitTables(t);  //生成表不会备份表
                    }
                });
                ConsoleHelper.WriteSuccessLine($"Tables created successfully!");
                Console.WriteLine();

                if (Appsettings.App(new string[] { "AppSettings", "SeedDBDataEnabled" }).ToBoolReq())
                {
                    Console.WriteLine("Seeding database...");

                    #region Role

                    //if (!await myContext.Db.Queryable<Role>().AnyAsync())
                    //{
                    //    myContext.GetEntityDB<Role>().InsertRange(JsonHelper.ParseFormByJson<List<Role>>(GetNetData.Get(string.Format(GitJsonFileFormat, "Role"))));
                    //    Console.WriteLine("Table:Role created success!");

                    //    //插入一条数据
                    //    //myContext.GetEntityDB<Role>().Insert( new Role(){});
                    //}
                    //else
                    //{
                    //    Console.WriteLine("Table:Role already exists...");
                    //}

                    #endregion Role

                    Console.WriteLine("Done seeding database.");
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