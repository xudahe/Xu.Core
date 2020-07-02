using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xu.Common;

namespace Xu.Tasks
{
    public class Job2Timed : IHostedService, IDisposable
    {
        private Timer _timer;

        public Job2Timed()
        {
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Job 2 is starting.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(60 * 10));//十分钟

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            ConsoleHelper.WriteWarningLine($"Job 2： {DateTime.Now}");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Job 2 is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}