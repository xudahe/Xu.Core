using System;
using System.Threading;
using System.Threading.Tasks;
using Xu.IServices;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Xu.Tasks;
using Xu.Common;
using SqlSugar.Extensions;
using Xu.EnumHelper;

namespace Xu.Extensions.HostedService;

public class QuartzJobHostedService : IHostedService
{
    private readonly ITasksQzSvc _tasksQzSvc;
    private readonly ISchedulerCenter _schedulerCenter;
    private readonly ILogger<QuartzJobHostedService> _logger;

    public QuartzJobHostedService(ITasksQzSvc tasksQzSvc, ISchedulerCenter schedulerCenter, ILogger<QuartzJobHostedService> logger)
    {
        _tasksQzSvc = tasksQzSvc;
        _schedulerCenter = schedulerCenter;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // _logger.LogInformation("Start QuartzJob Service!");
        await DoWork();
    }

    private async Task DoWork()
    {
        try
        {
            if (AppSettings.App("Middleware", "QuartzNetJob", "Enabled").ObjToBool())
            {
                var allQzServices = await _tasksQzSvc.Query();
                foreach (var item in allQzServices)
                {
                    if (item.JobStatus == JobStatus.运行中)
                    {
                        var ResuleModel = await _schedulerCenter.AddScheduleJobAsync(item);
                        if (ResuleModel.Success)
                        {
                            Console.WriteLine($"QuartzNetJob{item.JobName}启动成功！");
                        }
                        else
                        {
                            Console.WriteLine($"QuartzNetJob{item.JobName}启动失败！错误信息：{ResuleModel.Message}");
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error was reported when starting the job service.");
            throw;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        // _logger.LogInformation("Stop QuartzJob Service!");
        return Task.CompletedTask;
    }
}