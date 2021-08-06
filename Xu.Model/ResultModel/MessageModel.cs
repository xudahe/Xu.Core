namespace Xu.Model.ResultModel
{
    /// <summary>
    /// 通用返回信息类
    /// </summary>
    public class MessageModel<T>
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public int Status { get; set; } = 200;

        /// <summary>
        /// 操作是否成功
        /// </summary>
        public bool Success { get; set; } = false;

        /// <summary>
        /// 返回信息
        /// </summary>
        public string Message { get; set; } = "服务器异常";

        /// <summary>
        /// 返回数据集合
        /// </summary>
        public T Response { get; set; }

        /// <summary>
        /// 返回消息
        /// </summary>
        /// <param name="success">失败/成功</param>
        /// <param name="msg">消息</param>
        /// <param name="response">数据</param>
        /// <returns></returns>
        public static MessageModel<T> Msg(bool success, string msg, T response = default)
        {
            return new MessageModel<T>() { Message = msg, Response = response, Success = success };
        }
    }

    public class MessageModel<T1, T2>
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public int Status { get; set; } = 200;

        /// <summary>
        /// 操作是否成功
        /// </summary>
        public bool Success { get; set; } = false;

        /// <summary>
        /// 返回信息
        /// </summary>
        public string Message { get; set; } = "服务器异常";

        /// <summary>
        /// 返回数据集合
        /// </summary>
        public (T1, T2) Response { get; set; }
    }
}