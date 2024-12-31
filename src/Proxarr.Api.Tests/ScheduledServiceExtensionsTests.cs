using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Proxarr.Api.Core;
using Proxarr.Api.HostedServices;

namespace Proxarr.Api.Tests
{
    public class ScheduledServiceExtensionsTests
    {
        [Fact]
        public void AddCronJob_ShouldThrowNullException_WhenOptionsNull()
        {
            IServiceCollection services = new ServiceCollection();
            Action act = () => services.AddCronJob<FullScanHostedService>(null);
            act.Should().Throw<ArgumentNullException>().WithParameterName("options").WithMessage("Please provide Schedule Configurations. (Parameter 'options')");
        }

        [Fact]
        public void AddCronJob_ShouldThrowNullException_WhenCronIsNull()
        {
            IServiceCollection services = new ServiceCollection();
            Action act = () => services.AddCronJob<FullScanHostedService>(x => x.CronExpression = string.Empty);
            act.Should().Throw<ArgumentNullException>().WithParameterName("options").WithMessage("Empty Cron Expression is not allowed. (Parameter 'options')");
        }

        [Fact]
        public void AddCronJob_ShouldAddHostedService()
        {
            IServiceCollection services = new ServiceCollection();
            Action act = () => services.AddCronJob<FullScanHostedService>(x => x.CronExpression = "* * * * *");
            act.Should().NotThrow();
        }
    }
}
