using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace Xu.Common
{
    public class ChatHub : Hub<IChatClient>
    {
        /// <summary>
        /// 向指定群组发送信息
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <param name="message">信息内容</param>
        /// <returns></returns>
        public async Task SendMessageToGroupAsync(string groupName, string message)
        {
            await Clients.Group(groupName).ReceiveMessage(message);
        }

        /// <summary>
        /// 加入指定组
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <returns></returns>
        public async Task AddToGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        /// <summary>
        /// 退出指定组
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <returns></returns>
        public async Task RemoveFromGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }

        /// <summary>
        /// 向指定成员发送信息
        /// </summary>
        /// <param name="user">成员名</param>
        /// <param name="message">信息内容</param>
        /// <returns></returns>
        public async Task SendPrivateMessage(string user, string message)
        {
            await Clients.User(user).ReceiveMessage(message);
        }

        /// <summary>
        /// 客户端连接服务端-重要代码
        /// </summary>
        /// <returns></returns>
        public override Task OnConnectedAsync()
        {
            Console.WriteLine($"客户端ConnectionId=>【{Context.ConnectionId}】已连接服务器！");
            return base.OnConnectedAsync();
        }

        /// <summary>
        /// 客户端断开连接-重要代码
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public override Task OnDisconnectedAsync(Exception exception)
        {
            Console.WriteLine($"客户端ConnectionId=>【{Context.ConnectionId}】已断开服务器连接！");
            return base.OnDisconnectedAsync(exception);
        }

        //接收来自客户端的消息
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.ReceiveMessage(user, message);
        }

        //定于一个通讯管道，用来管理我们和客户端的连接
        //1、客户端调用 GetLatestCount，就像订阅
        public async Task GetLatestCount(string random)
        {
            //2、服务端主动向客户端发送数据，名字千万不能错
            await Clients.All.ReceiveUpdate(LogLock.GetLogData());

            //3、客户端再通过 ReceiveUpdate ，来接收
        }
    }
}