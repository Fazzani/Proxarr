﻿using Cronos;
using System.Diagnostics.CodeAnalysis;

namespace Proxarr.Api.Core
{
    [ExcludeFromCodeCoverage]
    public abstract class CronJobService(string cronExpression, TimeZoneInfo timeZoneInfo, ILogger logger) : IHostedService, IDisposable
    {
        private System.Timers.Timer? _timer;
        private readonly CronExpression _expression = CronExpression.Parse(cronExpression);
        private Task? _executingTask;
        private CancellationTokenSource _stoppingCts = new();
        private readonly SemaphoreSlim _schedulerCycle = new(0);

        public virtual Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("{JobName}: started with expression [{Expression}].", GetType().Name, cronExpression);
            _stoppingCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            _executingTask = ScheduleJob(_stoppingCts.Token);
            return _executingTask.IsCompleted ? _executingTask : Task.CompletedTask;
        }

        protected virtual async Task ScheduleJob(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var next = _expression.GetNextOccurrence(DateTimeOffset.Now, timeZoneInfo);

                    if (!next.HasValue)
                    {
                        continue;
                    }

                    logger.LogInformation("{JobName}: scheduled next run at {NextRun}", GetType().Name, next.ToString());
                    var delay = next.Value - DateTimeOffset.Now;

                    if (delay.TotalMilliseconds <= 0) // prevent non-positive values from being passed into Timer
                    {
                        logger.LogInformation("{LoggerName}: scheduled next run is in the past. Moving to next.", GetType().Name);
                        continue;
                    }

                    _timer = new System.Timers.Timer(delay.TotalMilliseconds);
                    _timer.Elapsed += async (_, _) =>
                    {
                        try
                        {
                            _timer.Dispose(); // reset and dispose timer
                            _timer = null;

                            if (!cancellationToken.IsCancellationRequested)
                            {
                                await DoWork(cancellationToken);
                            }
                        }
                        catch (OperationCanceledException)
                        {
                            logger.LogInformation("{LoggerName}: job received cancellation signal, stopping...", GetType().Name);
                        }
                        catch (Exception e)
                        {
                            logger.LogError(e, "{LoggerName}: an error happened during execution of the job", GetType().Name);
                        }
                        finally
                        {
                            _schedulerCycle.Release(); // Let the outer loop know that the next occurrence can be calculated.
                        }
                    };
                    _timer.Start();
                    await _schedulerCycle.WaitAsync(cancellationToken); // Wait nicely for any timer result.
                }
            }
            catch (OperationCanceledException)
            {
                logger.LogInformation("{LoggerName}: job received cancellation signal, stopping...", GetType().Name);
            }
        }

        public virtual async Task DoWork(CancellationToken cancellationToken)
        {
            await Task.Delay(5000, cancellationToken);  // do the work
        }

        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("{JobName}: stopping...", GetType().Name);
            _timer?.Stop();
            _timer?.Dispose();
            await _stoppingCts.CancelAsync();
            logger.LogInformation("{JobName}: stopped.", GetType().Name);
        }

        public virtual void Dispose()
        {
            _timer?.Dispose();
            _executingTask?.Dispose();
            _schedulerCycle.Dispose();
            _stoppingCts.Dispose();
            GC.SuppressFinalize(this);
        }
    }

#pragma warning disable S2326 // Unused type parameters should be removed
    public interface IScheduleConfig<T>
#pragma warning restore S2326 // Unused type parameters should be removed
    {
        string CronExpression { get; set; }
        TimeZoneInfo TimeZoneInfo { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class ScheduleConfig<T> : IScheduleConfig<T>
    {
        public string CronExpression { get; set; } = string.Empty;
        public TimeZoneInfo TimeZoneInfo { get; set; } = TimeZoneInfo.Local;
    }

    public static class ScheduledServiceExtensions
    {
        public static IServiceCollection AddCronJob<T>(this IServiceCollection services, Action<IScheduleConfig<T>> options) where T : CronJobService
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options), "Please provide Schedule Configurations.");
            }

            var config = new ScheduleConfig<T>();
            options.Invoke(config);

            if (string.IsNullOrWhiteSpace(config.CronExpression))
            {
                throw new ArgumentNullException(nameof(options), "Empty Cron Expression is not allowed.");
            }

            services.AddSingleton<IScheduleConfig<T>>(config);
            services.AddHostedService<T>();
            return services;
        }
    }
}
