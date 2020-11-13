namespace Xu.Model.ViewModel
{
    public class Data
    {
        /// <summary>
        ///
        /// </summary>
        public string isbn { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string title { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string origintitle { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string subtitle { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string image { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string author { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string translator { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string publisher { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string pubdate { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string tags { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string kaiben { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string zhizhang { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string binding { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string taozhuang { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string series { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string pages { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string price { get; set; }

        public string author_intro { get; set; }

        public string summary { get; set; }

        public string catalog { get; set; }
    }

    public class DoubanViewModel
    {
        /// <summary>
        ///
        /// </summary>
        public string status { get; set; }

        /// <summary>
        ///
        /// </summary>
        public Data data { get; set; }

        /// <summary>
        /// 获取图书数据成功
        /// </summary>
        public string msg { get; set; }
    }
}