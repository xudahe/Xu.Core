using Microsoft.AspNetCore.Hosting;
using Quartz;
using System.Threading.Tasks;
using Xu.IServices;

/// <summary>
/// 这里要注意下，命名空间和程序集是一样的，不然反射不到
/// </summary>
namespace Xu.Tasks
{
    public class JobQuartz : JobBase, IJob
    {
        private readonly IWebHostEnvironment _environment;

        public JobQuartz(IWebHostEnvironment environment, ITasksQzSvc tasksQzSvc)
        {
            _environment = environment;
            _tasksQzSvc = tasksQzSvc;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var executeLog = await ExecuteJob(context, async () => await Run(context));
        }

        public async Task Run(IJobExecutionContext context)
        {
            // 也可以通过数据库配置，获取传递过来的参数
            JobDataMap data = context.JobDetail.JobDataMap;
            //int jobId = data.GetInt("JobParam");
        }
    }
}