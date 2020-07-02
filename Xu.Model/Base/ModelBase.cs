using SqlSugar;
using System;

namespace Xu.Model
{
    /// <summary>
    /// 领域对象泛型基类，领域对象类名不能超过50个字符
    /// </summary>
    //[Serializable]
    public abstract class ModelBase
    {
        // ColumnName //列名
        // IsIgnore //是否忽略
        // IsPrimaryKey //是否是主键
        // IsIdentity //是否自增
        // MappingKeys //映射key
        // ColumnDescription //列描述
        // Length //长度
        // IsNullable //是否为空
        // IsIgnore 为true表示不会生成字段到数据库
        // OldColumnName //旧的列名
        // ColumnDataType //列类型，自定义
        // DecimalDigits //dicimal精度
        // OracleSequenceName //Oracle序列名
        // IsOnlyIgnoreInsert //是否仅对添加忽略

        /// <summary>
        /// Id，领域对象标识
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(IsNullable = true, IsOnlyIgnoreUpdate = true)]
        public DateTime CreateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 修改时间
        /// </summary>
        [SugarColumn(IsNullable = true, IsOnlyIgnoreInsert = true)]
        public DateTime? ModifyTime { get; set; }

        /// <summary>
        /// 删除时间（不为空代表该条数据已删除），逻辑上的删除，非物理删除
        /// </summary>
        [SugarColumn(IsNullable = true, IsOnlyIgnoreInsert = true)]
        public DateTime? DeleteTime { get; set; }
    }
}