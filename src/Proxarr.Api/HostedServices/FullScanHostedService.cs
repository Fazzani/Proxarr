using Proxarr.Api.Core;
using Proxarr.Api.Services;

namespace Proxarr.Api.HostedServices
{
    public class FullScanHostedService : CronJobService
    {
        private IRadarrService _radarrService;
        private ISonarrService _sonarrService;
        private IServiceScope _scope;
        private readonly ILogger<FullScanHostedService> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public FullScanHostedService(IScheduleConfig<FullScanHostedService> config,
                                     ILogger<FullScanHostedService> logger,
                                     IServiceScopeFactory serviceScopeFactory) : base(config.CronExpression, config.TimeZoneInfo, logger)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("{ServiceName} starts.", nameof(FullScanHostedService));
            _scope = _serviceScopeFactory.CreateScope();
            _radarrService = _scope.ServiceProvider.GetRequiredService<IRadarrService>();
            _sonarrService = _scope.ServiceProvider.GetRequiredService<ISonarrService>();
            return base.StartAsync(cancellationToken);
        }

        public override Task DoWork(CancellationToken cancellationToken)
        {
            _logger.LogInformation("CronJob {ServiceName} is working.", nameof(FullScanHostedService));
            return Task.WhenAll(_radarrService.FullScan(cancellationToken), _sonarrService.FullScan(cancellationToken));
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("CronJob {ServiceName} is stopping.", nameof(FullScanHostedService));
            return base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            _scope?.Dispose();
            base.Dispose();
        }
    }
}
