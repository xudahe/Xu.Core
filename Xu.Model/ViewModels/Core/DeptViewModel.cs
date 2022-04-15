using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xu.Model.ViewModels
{
    public class DeptViewModel : ModelBase
    {
        /// <summary>
        /// 部门简码
        /// </summary>
        public string DeptCode { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        public string DeptName { get; set; }

        /// <summary>
        /// 部门负责人
        /// </summary>
        public string DeptManager { get; set; }

        /// <summary>
        /// 上级部门
        /// </summary>
        public int? ParentId { get; set; }

        /// <summary>
        /// 上级部门名称
        /// </summary>
        public string ParentName { get; set; }

        /// <summary>
        /// 是否禁用
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}