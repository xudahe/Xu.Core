using Quartz;
using Quartz.Impl;
using Quartz.Impl.Triggers;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using System.Threading.Tasks;
using Xu.Model.Models;
using Xu.Model.ResultModel;
using Xu.Model.ViewModels;

namespace Xu.Tasks
{
    /// <summary>
    /// 任务调度管理中心
    /// </summary>
    public class SchedulerCenter : ISchedulerCenter
    {
        private Task<IScheduler> _scheduler; //用于与调度程序交互的主程序接口
        private readonly IJobFactory _iocjobFactory;

        public SchedulerCenter(IJobFactory jobFactory)
        {
            _iocjobFactory = jobFactory;
            _scheduler = GetSchedulerAsync();
        }

        /// <summary>
        /// 返回任务计划（调度器）
        /// </summary>
        /// <returns></returns>
        private Task<IScheduler> GetSchedulerAsync()
        {
            if (_scheduler != null)
                return _scheduler;
            else
            {
                // 从Factory中获取Scheduler实例
                NameValueCollection collection = new NameValueCollection
                {
                    { "quartz.serializer.type", "binary" },
                    //以下配置需要数据库表配合使用，表结构sql地址：https://github.com/quartznet/quartznet/tree/master/database/tables
                    //{ "quartz.jobStore.type","Quartz.Impl.AdoJobStore.JobStoreTX, Quartz"},
                    //{ "quartz.jobStore.driverDelegateType","Quartz.Impl.AdoJobStore.StdAdoDelegate, Quartz"},
                    //{ "quartz.jobStore.tablePrefix","QRTZ_"},
                    //{ "quartz.jobStore.dataSource","myDS"},
                    //{ "quartz.dataSource.myDS.connectionString",AppSettingHelper.MysqlConnection},//连接字符串
                    //{ "quartz.dataSource.myDS.provider","MySql"},
                    //{ "quartz.jobStore.useProperties","true"}
                };
                //新建一个调度器工厂
                StdSchedulerFactory factory = new StdSchedulerFactory(collection);
                //使用工厂生成一个调度器
                return _scheduler = factory.GetScheduler();
            }
        }

        /// <summary>
        /// 开启任务调度
        /// </summary>
        /// <returns></returns>
        public async Task<MessageModel<string>> StartScheduleAsync()
        {
            var result = new MessageModel<string>();
            try
            {
                _scheduler.Result.JobFactory = _iocjobFactory;
                if (!_scheduler.Result.IsStarted)
                {
                    await _scheduler.Result.Start();
                    await Console.Out.WriteLineAsync("任务调度开启！");
                    result.Success = true;
                    result.Message = $"任务调度开启成功";
                    return result;
                }
                else
                {
                    result.Success = false;
                    result.Message = $"任务调度已经开启";
                    return result;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 停止任务调度
        /// </summary>
        /// <returns></returns>
        public async Task<MessageModel<string>> StopScheduleAsync()
        {
            var result = new MessageModel<string>();
            try
            {
                if (!_scheduler.Result.IsShutdown)
                {
                    //等待任务运行完成
                    await _scheduler.Result.Shutdown();
                    await Console.Out.WriteLineAsync("任务调度停止！");
                    result.Success = true;
                    result.Message = $"任务调度停止成功";
                    return result;
                }
                else
                {
                    result.Success = false;
                    result.Message = $"任务调度已经停止";
                    return result;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 添加一个计划任务（映射程序集指定IJob实现类）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tasksQz"></param>
        /// <returns></returns>
        public async Task<MessageModel<string>> AddScheduleJobAsync(TasksQz tasksQz)
        {
            var result = new MessageModel<string>();

            if (tasksQz != null)
            {
                try
                {
                    JobKey jobKey = new JobKey(tasksQz.Id.ToString(), tasksQz.JobGroup);
                    if (await _scheduler.Result.CheckExists(jobKey))
                    {
                        result.Success = false;
                        result.Message = $"任务计划已经在执行:【{tasksQz.JobName}】,请勿重复启动！";
                        return result;
                    }

                    #region 通过反射获取程序集类型和类

                    Assembly assembly = Assembly.Load(new AssemblyName(tasksQz.AssemblyName));
                    Type jobType = assembly.GetType(tasksQz.AssemblyName + "." + tasksQz.ClassName);

                    //传入反射出来的执行程序集
                    IJobDetail job = new JobDetailImpl(tasksQz.Id.ToString(), tasksQz.JobGroup, jobType);
                    job.JobDataMap.Add("JobParam", tasksQz.JobParams); //设置参数

                    #endregion 通过反射获取程序集类型和类

                    //判断任务调度是否开启
                    if (!_scheduler.Result.IsStarted)
                    {
                        await StartScheduleAsync();
                    }

                    // 创建触发器
                    ITrigger trigger;

                    #region 泛型传递

                    //IJobDetail job = JobBuilder.Create<JobQuartz>()
                    //    .WithIdentity(tasksQz.JobName, tasksQz.JobGroup)
                    //    .Build();

                    #endregion 泛型传递

                    //创建一个触发器
                    if (tasksQz.Cron != null && CronExpression.IsValidExpression(tasksQz.Cron) && tasksQz.TriggerType == "cron")
                    {
                        trigger = CreateCronTrigger(tasksQz);

                        ((CronTriggerImpl)trigger).MisfireInstruction = MisfireInstruction.CronTrigger.DoNothing;
                    }
                    else
                    {
                        trigger = CreateSimpleTrigger(tasksQz);
                    }

                    //将触发器和任务器绑定到调度器中
                    await _scheduler.Result.ScheduleJob(job, trigger);

                    result.Success = true;
                    result.Message = $"启动任务:【{tasksQz.JobName}】成功";
                    return result;
                }
                catch (Exception ex)
                {
                    result.Success = false;
                    result.Message = $"添加任务异常:【{ex.Message}】";
                    return result;
                }
            }
            else
            {
                result.Success = false;
                result.Message = $"任务计划不存在:【{tasksQz.JobName}】";
                return result;
            }
        }

        /// <summary>
        /// 暂停一个指定的计划任务
        /// </summary>
        /// <returns></returns>
        public async Task<MessageModel<string>> StopScheduleJobAsync(TasksQz tasksQz)
        {
            var result = new MessageModel<string>();
            try
            {
                JobKey jobKey = new JobKey(tasksQz.Id.ToString(), tasksQz.JobGroup);
                if (!await _scheduler.Result.CheckExists(jobKey))
                {
                    result.Message = $"未找到要暂停的任务:【{tasksQz.JobName}】";
                }
                else
                {
                    await _scheduler.Result.PauseJob(jobKey);
                    result.Success = true;
                    result.Message = $"暂停任务:【{tasksQz.JobName}】成功";
                }
            }
            catch (Exception)
            {
                result.Message = $"暂停任务:【{tasksQz.JobName}】失败";
            }

            return result;
        }

        /// <summary>
        /// 删除一个指定的计划任务
        /// </summary>
        /// <returns></returns>
        public async Task<MessageModel<string>> DeleteScheduleJobAsync(TasksQz tasksQz)
        {
            var result = new MessageModel<string>();
            try
            {
                JobKey jobKey = new JobKey(tasksQz.Id.ToString(), tasksQz.JobGroup);
                if (!await _scheduler.Result.CheckExists(jobKey))
                {
                    result.Message = $"未找到要删除的任务:【{tasksQz.JobName}】";
                }
                else
                {
                    await _scheduler.Result.DeleteJob(jobKey);
                    result.Success = true;
                    result.Message = $"删除任务:【{tasksQz.JobName}】成功";
                }
            }
            catch (Exception)
            {
                result.Message = $"删除任务:【{tasksQz.JobName}】失败";
            }

            return result;
        }

        /// <summary>
        /// 恢复一个指定的计划任务
        /// </summary>
        /// <param name="tasksQz"></param>
        /// <returns></returns>
        public async Task<MessageModel<string>> ResumeScheduleJobAsync(TasksQz tasksQz)
        {
            var result = new MessageModel<string>();
            try
            {
                JobKey jobKey = new JobKey(tasksQz.Id.ToString(), tasksQz.JobGroup);
                if (!await _scheduler.Result.CheckExists(jobKey))
                {
                    result.Message = $"未找到要恢复的任务:【{tasksQz.JobName}】,请先选择添加计划！";
                }
                else
                {
                    await _scheduler.Result.ResumeJob(jobKey);
                    result.Success = true;
                    result.Message = $"恢复计划任务:【{tasksQz.JobName}】成功";
                }
            }
            catch (Exception)
            {
                result.Message = $"恢复计划任务:【{tasksQz.JobName}】失败";
            }

            return result;
        }

        /// <summary>
        /// 立即执行 一个任务
        /// </summary>
        /// <param name="tasksQz"></param>
        /// <returns></returns>
        public async Task<MessageModel<string>> ExecuteJobAsync(TasksQz tasksQz)
        {
            var result = new MessageModel<string>();
            try
            {
                JobKey jobKey = new JobKey(tasksQz.Id.ToString(), tasksQz.JobGroup);

                //判断任务是否存在，存在则 触发一次，不存在则先添加一个任务，触发以后再 停止任务
                if (!await _scheduler.Result.CheckExists(jobKey))
                {
                    //不存在 则 添加一个计划任务
                    await AddScheduleJobAsync(tasksQz);

                    //触发执行一次
                    await _scheduler.Result.TriggerJob(jobKey);

                    //停止任务
                    await StopScheduleJobAsync(tasksQz);

                    result.Success = true;
                    result.Message = $"立即执行计划任务:【{tasksQz.JobName}】成功";
                }
                else
                {
                    await _scheduler.Result.TriggerJob(jobKey);
                    result.Success = true;
                    result.Message = $"立即执行计划任务:【{tasksQz.JobName}】成功";
                }
            }
            catch (Exception ex)
            {
                result.Message = $"立即执行计划任务失败:【{ex.Message}】";
            }

            return result;
        }

        /// <summary>
        /// 任务是否存在?
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsExistScheduleJobAsync(TasksQz tasksQz)
        {
            JobKey jobKey = new JobKey(tasksQz.Id.ToString(), tasksQz.JobGroup);
            if (await _scheduler.Result.CheckExists(jobKey))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #region 状态状态帮助方法

        /// <summary>
        /// 获取任务触发器状态
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<List<TaskInfoDto>> GetTaskStaus(TasksQz tasksQz)
        {
            var ls = new List<TaskInfoDto>();
            var noTask = new List<TaskInfoDto>{ new TaskInfoDto {
                jobId = tasksQz.Id.ToString(),
                jobGroup = tasksQz.JobGroup,
                triggerId = "",
                triggerGroup = "",
                triggerStatus = "不存在"
            } };
            JobKey jobKey = new JobKey(tasksQz.Id.ToString(), tasksQz.JobGroup);
            IJobDetail job = await this._scheduler.Result.GetJobDetail(jobKey);
            if (job == null)
            {
                return noTask;
            }
            var triggers = await this._scheduler.Result.GetTriggersOfJob(jobKey);
            if (triggers == null || triggers.Count == 0)
            {
                return noTask;
            }
            foreach (var trigger in triggers)
            {
                var triggerStaus = await this._scheduler.Result.GetTriggerState(trigger.Key);
                string state = GetTriggerState(triggerStaus.ToString());
                ls.Add(new TaskInfoDto
                {
                    jobId = job.Key.Name,
                    jobGroup = job.Key.Group,
                    triggerId = trigger.Key.Name,
                    triggerGroup = trigger.Key.Group,
                    triggerStatus = state
                });
            }
            return ls;
        }

        /// <summary>
        /// 获取触发器标识
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetTriggerState(string key)
        {
            string state = null;
            if (key != null)
                key = key.ToUpper();
            switch (key)
            {
                case "1":
                    state = "暂停";
                    break;

                case "2":
                    state = "完成";
                    break;

                case "3":
                    state = "出错";
                    break;

                case "4":
                    state = "阻塞";
                    break;

                case "0":
                    state = "正常";
                    break;

                case "-1":
                    state = "不存在";
                    break;

                case "BLOCKED":
                    state = "阻塞";
                    break;

                case "COMPLETE":
                    state = "完成";
                    break;

                case "ERROR":
                    state = "出错";
                    break;

                case "NONE":
                    state = "不存在";
                    break;

                case "NORMAL":
                    state = "正常";
                    break;

                case "PAUSED":
                    state = "暂停";
                    break;
            }
            return state;
        }

        #endregion 状态状态帮助方法

        #region 创建触发器帮助方法

        /// <summary>
        /// 创建SimpleTrigger触发器（简单触发器）
        /// </summary>
        /// <param name="tasksQz"></param>
        /// <param name="starRunTime"></param>
        /// <param name="endRunTime"></param>
        /// <returns></returns>
        private static ITrigger CreateSimpleTrigger(TasksQz tasksQz)
        {
            DateTimeOffset starRunTime = DateBuilder.NextGivenSecondDate(tasksQz.StartTime.HasValue ? tasksQz.StartTime : DateTime.Now, 1);
            DateTimeOffset endRunTime = DateBuilder.NextGivenSecondDate(tasksQz.EndTime.HasValue ? tasksQz.EndTime : DateTime.MaxValue.AddDays(-1), 1);

            if (tasksQz.CycleRunTimes > 0)
            {
                ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity(tasksQz.Id.ToString(), tasksQz.JobGroup)
                .StartAt(starRunTime)  //开始时间
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(tasksQz.IntervalSecond) //执行时间间隔，单位:秒
                    .WithRepeatCount(tasksQz.CycleRunTimes - 1))  //执行次数、默认从0开始
                .EndAt(endRunTime)  //结束时间
                .Build();
                return trigger;
            }
            else
            {
                ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity(tasksQz.Id.ToString(), tasksQz.JobGroup)
                .StartAt(starRunTime)
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(tasksQz.IntervalSecond)
                    .RepeatForever()//无限循环
                )
                .EndAt(endRunTime)
                .Build();
                return trigger;
            }
        }

        /// <summary>
        /// 创建类型Cron的触发器
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        private static ITrigger CreateCronTrigger(TasksQz tasksQz)
        {
            DateTimeOffset starRunTime = DateBuilder.NextGivenSecondDate(tasksQz.StartTime ?? DateTime.Now, 1);
            DateTimeOffset endRunTime = DateBuilder.NextGivenSecondDate(tasksQz.EndTime ?? DateTime.MaxValue.AddDays(-1), 1);

            // 作业触发器
            return TriggerBuilder.Create()
                   .WithIdentity(tasksQz.Id.ToString(), tasksQz.JobGroup)
                   .StartAt(starRunTime)//开始时间
                   .EndAt(endRunTime)//结束数据
                   .WithCronSchedule(tasksQz.Cron)//指定cron表达式
                   .ForJob(tasksQz.Id.ToString(), tasksQz.JobGroup)//作业名称
                   .Build();
        }

        #endregion 创建触发器帮助方法
    }
}