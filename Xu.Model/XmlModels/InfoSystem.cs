﻿using System.Collections.Generic;

namespace Xu.Model.XmlModels
{
    public class InfoSystem : RootEntityTkey
    {
        /// <summary>
        /// 系统名称
        /// </summary>
        public string SystemName { get; set; }

        public IList<InfoMenu> InfoMenuList { get; set; } = new List<InfoMenu>();
    }
}