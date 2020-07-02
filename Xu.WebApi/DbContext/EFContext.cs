using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Xu.Common;

namespace Xu.Model
{
    public partial class EFContext : DbContext
    {
        public EFContext()
        {
        }

        public EFContext(DbContextOptions<EFContext> options) : base(options)
        {
        }

        public virtual DbSet<Student> student { get; set; }

        private static readonly string _connectionString = BaseDBConfig.ConnectionString;

        /// <summary>
        /// 数据库链接配置
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //Add-Migration Init  //其中Init是你自定义的版本名称
                //update-database Init //更新数据库操作 init为版本名称
                optionsBuilder.UseMySql(_connectionString);
            }
        }

#pragma warning disable CS1572 // XML 注释中有“modelBuilder”的 param 标记，但是没有该名称的参数
        /// <summary>
        /// 模型创建重载,重写DbContext默认的OnModelCreating方法,使用自定义的方法动态加载实体映射类型
        /// </summary>
        /// <param name="modelBuilder">模型创建器</param>
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);
        //}

        //[Table]：表名称
        //[Key]：表主键
        //[Column(“F_ENCODE”, TypeName = “varchar(200)”)]：表字段名称和类型
        //[MaxLength]：字符串长度
        //[ForeignKey：表外键约束
        //[NotMapped]：排除该字段，在更新添加时排除
        //[Required]：非空
        //[Display(Name = "姓名")]：注释

        [Table("Student")]
#pragma warning restore CS1572 // XML 注释中有“modelBuilder”的 param 标记，但是没有该名称的参数
        public class Student
        {
            [Key]
            public int ID { get; set; }

            [Display(Name = "姓名")]
            public string Name { get; set; }

            [Display(Name = "年龄")]
            public int Age { get; set; }
        }
    }
}