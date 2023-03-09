using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Xu.Common;
using SqlSugar.Extensions;

namespace Xu.Extensions;

/// <summary>
/// 生成种子数据中间件服务
/// </summary>
public sealed class SeedDataHostedService : IHostedService
{
    private readonly MyContext _myContext;
    private readonly ILogger<SeedDataHostedService> _logger;
    private readonly string _webRootPath;

    public SeedDataHostedService(
        MyContext myContext,
        IWebHostEnvironment webHostEnvironment,
        ILogger<SeedDataHostedService> logger)
    {
        _myContext = myContext;
        _logger = logger;
        _webRootPath = webHostEnvironment.WebRootPath;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // _logger.LogInformation("Start Initialization Db Seed Service!");
        await DoWork();
    }

    private async Task DoWork()
    {
        try
        {
            if (AppSettings.App("AppSettings", "SeedDBEnabled").ObjToBool() || AppSettings.App("AppSettings", "SeedDBDataEnabled").ObjToBool())
            {
                await DBSeed.SeedAsync(_myContext, _webRootPath);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occured seeding the Database.");
            throw;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        // _logger.LogInformation("Stop Initialization Db Seed Service!");
        return Task.CompletedTask;
    }
}