using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xu.Common;

namespace Xu.Tasks
{
    public class Job1Timed : IHostedService, IDisposable
    {
        private Timer _timer;

        // 构造函数
        public Job1Timed()
        {
        }

        /// <summary>
        /// 系统级任务执行启动
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Job 1 is starting.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(60 * 60));//一个小时

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            ConsoleHelper.WriteSuccessLine($"Job 1： {DateTime.Now}");
        }

        /// <summary>
        /// 系统级任务执行关闭
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Job 1 is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}