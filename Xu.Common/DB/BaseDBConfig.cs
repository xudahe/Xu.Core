﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Xu.Common
{
    public class BaseDBConfig
    {
        /*
         * 目前是多库操作，默认加载的是appsettings.json设置为true的第一个db连接。
         */

        //public static (List<MutiDBOperate>, List<MutiDBOperate>) MutiConnectionString => MutiInitConn();
        public static (List<MutiDBOperate> allDbs, List<MutiDBOperate> slaveDbs) MutiConnectionString => MutiInitConn();

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

            return conn[conn.Length - 1];
        }

        public static (List<MutiDBOperate>, List<MutiDBOperate>) MutiInitConn()
        {
            List<MutiDBOperate> listdatabase = AppSettings.App<MutiDBOperate>("DBS").Where(i => i.Enabled).ToList();
            foreach (var i in listdatabase)
            {
                SpecialDbString(i);
            }
            List<MutiDBOperate> listdatabaseSimpleDB = new List<MutiDBOperate>();//单库
            List<MutiDBOperate> listdatabaseSlaveDB = new List<MutiDBOperate>();//从库

            // 单库，且不开启读写分离，只保留一个
            if (!AppSettings.App(new string[] { "CQRSEnabled" }).ToBoolReq() && !AppSettings.App(new string[] { "MutiDBEnabled" }).ToBoolReq())
            {
                if (listdatabase.Count == 1)
                {
                    return (listdatabase, listdatabaseSlaveDB);
                }
                else
                {
                    var dbFirst = listdatabase.FirstOrDefault(d => d.ConnId == AppSettings.App(new string[] { "MainDB" }).ToString());
                    if (dbFirst == null)
                    {
                        dbFirst = listdatabase.FirstOrDefault();
                    }
                    listdatabaseSimpleDB.Add(dbFirst);
                    return (listdatabaseSimpleDB, listdatabaseSlaveDB);
                }
            }

            // 读写分离，且必须是单库模式，获取从库
            if (AppSettings.App(new string[] { "CQRSEnabled" }).ToBoolReq() && !AppSettings.App(new string[] { "MutiDBEnabled" }).ToBoolReq())
            {
                if (listdatabase.Count > 1)
                {
                    listdatabaseSlaveDB = listdatabase.Where(d => d.ConnId != AppSettings.App(new string[] { "MainDB" }).ToString()).ToList();
                }
            }

            return (listdatabase, listdatabaseSlaveDB);
        }

        private static MutiDBOperate SpecialDbString(MutiDBOperate mutiDBOperate)
        {
            if (mutiDBOperate.DbType == DataBaseType.Sqlite)
            {
                mutiDBOperate.Connection = $"DataSource=" + Path.Combine(Environment.CurrentDirectory, mutiDBOperate.Connection);
            }
            else if (mutiDBOperate.DbType == DataBaseType.SqlServer)
            {
                mutiDBOperate.Connection = DifDBConnOfSecurity(@"D:\my-file\dbcountpsw1.txt", @"D:\my-file\dbcountpsw1.txt", mutiDBOperate.Connection);
            }
            else if (mutiDBOperate.DbType == DataBaseType.MySql)
            {
                mutiDBOperate.Connection = DifDBConnOfSecurity(@"D:\my-file\dbCountPsw1_MySqlConn.txt", @"D:\my-file\dbCountPsw1_MySqlConn.txt", mutiDBOperate.Connection);
            }
            else if (mutiDBOperate.DbType == DataBaseType.Oracle)
            {
                mutiDBOperate.Connection = DifDBConnOfSecurity(@"D:\my-file\dbCountPsw1_OracleConn.txt", @"D:\my-file\dbCountPsw1_OracleConn.txt", mutiDBOperate.Connection);
            }

            return mutiDBOperate;
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

    public class MutiDBOperate
    {
        /// <summary>
        /// 连接启用开关
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// 连接ID
        /// </summary>
        public string ConnId { get; set; }

        /// <summary>
        /// 从库执行级别，越大越先执行
        /// </summary>
        public int HitRate { get; set; }

        /// <summary>
        /// 连接字符串
        /// </summary>
        public string Connection { get; set; }

        /// <summary>
        /// 数据库类型
        /// </summary>
        public DataBaseType DbType { get; set; }
    }
}