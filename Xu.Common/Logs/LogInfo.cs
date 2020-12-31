﻿using System;

namespace Xu.Common
{
    public class LogInfo
    {
        public DateTime Datetime { get; set; }
        public string Content { get; set; }
        public string ClientIP { get; set; }
        public string LogColor { get; set; }
        public int Import { get; set; } = 0;
    }
}