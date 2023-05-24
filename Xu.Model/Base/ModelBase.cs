using SqlSugar;
using System;

namespace Xu.Model
{
    /// <summary>
    /// 领域对象泛型基类
    /// </summary>
    //[Serializable]
    public abstract class ModelBase : RootEntityTkey
    {
        // ColumnName //列名
        // IsIgnore //是否忽略
        // IsPrimaryKey //是否是主键
        // IsIdentity //是否自增
        // MappingKeys //映射key
        // ColumnDescription //列描述
        // Length //长度
        // IsNullable //默为false 不可以为null
        // IsIgnore 为true表示不会生成字段到数据库
        // OldColumnName //旧的列名
        // ColumnDataType //列类型，自定义
        // DecimalDigits //dicimal精度
        // OracleSequenceName //Oracle序列名
        // IsOnlyIgnoreInsert //是否仅对添加忽略
        // IsOnlyIgnoreUpdate //是否仅对修改忽略

        //[SugarColumn(ColumnDataType = "varchar(4000)", IsJson = true)] //IsJson 支持JObject JArray ，实体 ，集合等类型

        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(IsNullable = true, IsOnlyIgnoreUpdate = true, ColumnDescription = "创建时间")]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        [SugarColumn(IsNullable = true, IsOnlyIgnoreInsert = true, ColumnDescription = "修改时间")]
        public DateTime? ModifyTime { get; set; }

        /// <summary>
        /// 删除时间（不为空代表该条数据已删除），逻辑上的删除，非物理删除
        /// </summary>
        [SugarColumn(IsNullable = true, IsOnlyIgnoreInsert = true, ColumnDescription = "删除时间")]
        public DateTime? DeleteTime { get; set; }
    }
}