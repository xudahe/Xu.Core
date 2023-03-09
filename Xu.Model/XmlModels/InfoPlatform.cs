using System.Collections.Generic;
using Xu.Model.XmlModels;

namespace Xu.Model.XmlModels
{
    public class InfoPlatform : RootEntityTkey
    {
        /// <summary>
        /// 平台名称
        /// </summary>
        public string PlatformName { get; set; }

        public IList<InfoSystem> InfoSystemList { get; set; } = new List<InfoSystem>();
    }
}