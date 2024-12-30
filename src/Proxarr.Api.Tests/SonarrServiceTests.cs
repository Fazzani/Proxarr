using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Proxarr.Api.Configuration;
using Proxarr.Api.Core;
using Proxarr.Api.Core.Extensions;
using Proxarr.Api.Models;
using Proxarr.Api.Services;
using Sonarr.Http.Client;
using TMDbLib.Objects.General;
using TMDbLib.Objects.TvShows;

namespace Proxarr.Api.Tests
{
    public class SonarrServiceTests
    {
        private readonly Mock<ILogger<SonarrService>> _loggerMock;
        private readonly Mock<ITmdbProxy> _tmdbClientMock;
        private readonly Mock<IOptions<AppConfiguration>> _appConfigMock;
        private readonly Mock<SonarrClient> _sonarrClientMock;
        private readonly SonarrService _sonarrService;
        private readonly TagResource _qualifyTag;

        public SonarrServiceTests()
        {
            _loggerMock = new Mock<ILogger<SonarrService>>();
            _tmdbClientMock = new Mock<ITmdbProxy>();
            _appConfigMock = new Mock<IOptions<AppConfiguration>>();
            _sonarrClientMock = new Mock<SonarrClient>(null);

            var appConfig = new AppConfiguration
            {
                Clients = [new ClientConfiguration { Application = "Sonarr", BaseUrl = "http://localhost", ApiKey = "fake_api_key" }],
                WatchProviders = "US:Netflix, FR: Amazon Prime Video",
            };

            _appConfigMock.Setup(x => x.Value).Returns(appConfig);

            _sonarrService = new SonarrService(_loggerMock.Object, _tmdbClientMock.Object, _appConfigMock.Object, _sonarrClientMock.Object);
            _qualifyTag = new TagResource { Id = 30, Label = _appConfigMock.Object.Value.TagName };
        }

        [Fact]
        public async Task Qualify_ShouldReturnNotFound_WhenSeriesNotFound()
        {
            // Arrange
            var tvAdded = new TvAdded
            {
                ApplicationUrl = "http://localhost",
                EventType = "FULL_SCAN",
                Series = new Series { Id = 1, TmdbId = 123, Title = "Test Series" }
            };
            var cancellationToken = new CancellationToken();

            _tmdbClientMock.Setup(x => x.GetTvShowAsync(123, TvShowMethods.WatchProviders, null, null, cancellationToken))
                .ReturnsAsync(new TvShow { WatchProviders = new SingleResultContainer<Dictionary<string, WatchProviders>> { Results = [] } });

            _sonarrClientMock.Setup(x => x.SeriesGETAsync(1, false, cancellationToken))
                .ReturnsAsync((SeriesResource)null);

            // Act
            var result = await _sonarrService.Qualify(tvAdded, cancellationToken);

            // Assert
            Assert.Equal("NotFound", result);
        }

        [Fact]
        public async Task Qualify_ShouldUpdateTags_WhenSeriesFound()
        {
            // Arrange
            var tvAdded = new TvAdded
            {
                ApplicationUrl = "http://localhost",
                EventType = "FULL_SCAN",
                Series = new Series { Id = 1, TmdbId = 123, Title = "Test Series" },
            };
            var cancellationToken = new CancellationToken();

            var watchProviders = new SingleResultContainer<Dictionary<string, WatchProviders>>
            {
                Results = new Dictionary<string, WatchProviders>
                {
                    { "US", new WatchProviders { Free = [new WatchProviderItem { ProviderName = "Netflix" }] } }
                }
            };

            var seriesResource = new SeriesResource { Id = 1, Title = "Test Series", Tags = [] };

            _tmdbClientMock.Setup(x => x.GetTvShowAsync(123, TvShowMethods.WatchProviders, null, null, cancellationToken))
                .ReturnsAsync(new TvShow { WatchProviders = watchProviders });

            _sonarrClientMock.Setup(x => x.SeriesGETAsync(1, false, cancellationToken))
                .ReturnsAsync(seriesResource);

            _sonarrClientMock.Setup(x => x.TagAllAsync(cancellationToken))
                .ReturnsAsync([]);

            _sonarrClientMock.Setup(x => x.TagPOSTAsync(It.IsAny<TagResource>(), cancellationToken))
                .ReturnsAsync(new TagResource { Id = 1, Label = "Netflix" });

            // Act
            var result = await _sonarrService.Qualify(tvAdded, cancellationToken);

            // Assert
            _sonarrClientMock.Verify(x => x.SeriesPUTAsync(false, "1", seriesResource, cancellationToken), Times.Once);
            Assert.Empty(result);
        }

        [Fact]
        public async Task Qualify_Should_NotBeTagged_When_MatchedWatchProvider()
        {
            // Arrange
            var tvAdded = new TvAdded
            {
                ApplicationUrl = "http://localhost",
                EventType = "SerieAdd",
                Series = new Series { Id = 1, TmdbId = 123, Title = "Test Series" },
            };
            var cancellationToken = new CancellationToken();

            var watchProviders = new SingleResultContainer<Dictionary<string, WatchProviders>>
            {
                Results = new Dictionary<string, WatchProviders>
                {
                    { "US", new WatchProviders { Free = [new WatchProviderItem { ProviderName = "Netflix" }] } }
                }
            };

            var seriesResource = new SeriesResource { Id = 1, Title = "Test Series", Tags = [] };

            _tmdbClientMock.Setup(x => x.GetTvShowAsync(123, TvShowMethods.WatchProviders, null, null, cancellationToken))
                .ReturnsAsync(new TvShow { WatchProviders = watchProviders });

            _sonarrClientMock.Setup(x => x.SeriesGETAsync(1, false, cancellationToken))
                .ReturnsAsync(seriesResource);

            _sonarrClientMock.Setup(x => x.TagAllAsync(cancellationToken))
                .ReturnsAsync([]);

            _sonarrClientMock.Setup(x => x.TagPOSTAsync(It.IsAny<TagResource>(), cancellationToken))
                .ReturnsAsync(new TagResource { Id = 1, Label = "Netflix" });

            // Act
            var result = await _sonarrService.Qualify(tvAdded, cancellationToken);
            var tagResource = new TagResource { Id = 1, Label = "Netflix" };

            // Assert
            _sonarrClientMock.Verify(x => x.SeriesPUTAsync(false, "1", seriesResource, cancellationToken), Times.Once);
            Assert.Empty(result);
            _sonarrClientMock.Verify(x => x.TagPOSTAsync(It.Is<TagResource>(x => x.Label == tagResource.Slugify()!.Label), cancellationToken), Times.Once);
            Assert.Single(seriesResource.Tags);
            Assert.True(seriesResource.Tags.All(x => x != _qualifyTag.Id));
        }

        [Fact]
        public async Task Qualify_Should_BeTagged_When_MatchedWatchProvider()
        {
            // Arrange
            var tvAdded = new TvAdded
            {
                ApplicationUrl = "http://localhost",
                EventType = "SerieAdd",
                Series = new Series { Id = 1, TmdbId = 123, Title = "Test Series" },
            };
            var cancellationToken = new CancellationToken();

            var watchProviders = new SingleResultContainer<Dictionary<string, WatchProviders>>
            {
                Results = new Dictionary<string, WatchProviders>
                {
                    { "NL", new WatchProviders { Free = [new WatchProviderItem { ProviderName = "Netflix" }] } }
                }
            };

            var seriesResource = new SeriesResource { Id = 1, Title = "Test Series", Tags = [] };

            _tmdbClientMock.Setup(x => x.GetTvShowAsync(123, TvShowMethods.WatchProviders, null, null, cancellationToken))
               .ReturnsAsync(new TvShow { WatchProviders = watchProviders });

            _sonarrClientMock.Setup(x => x.SeriesGETAsync(1, false, cancellationToken))
                .ReturnsAsync(seriesResource);

            _sonarrClientMock.Setup(x => x.TagAllAsync(cancellationToken))
                .ReturnsAsync([_qualifyTag]);

            _sonarrClientMock.Setup(x => x.TagPOSTAsync(It.IsAny<TagResource>(), cancellationToken))
                .ReturnsAsync(new TagResource { Id = 1, Label = "Netflix" });

            // Act
            var result = await _sonarrService.Qualify(tvAdded, cancellationToken);

            // Assert
            _sonarrClientMock.Verify(x => x.SeriesPUTAsync(false, "1", seriesResource, cancellationToken), Times.Once);
            Assert.Empty(result);
            Assert.Single(seriesResource.Tags);
            Assert.Contains(seriesResource.Tags, x => x == _qualifyTag.Id);
        }
    }
}