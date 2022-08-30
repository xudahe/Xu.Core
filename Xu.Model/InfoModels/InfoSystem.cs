using System.Collections.Generic;

namespace Xu.Model
{
    public class InfoSystem
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Guid
        /// </summary>
        public string Guid { get; set; }

        /// <summary>
        /// 系统名称
        /// </summary>
        public string SystemName { get; set; }

        public IList<InfoMenu> InfoMenuList { get; set; } = new List<InfoMenu>();
    }
}