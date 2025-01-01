using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Proxarr.Api.Configuration;
using Proxarr.Api.Core;
using Proxarr.Api.Core.Extensions;
using Proxarr.Api.Models;
using Proxarr.Api.Services;
using Radarr.Http.Client;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Movies;
using Movie = Proxarr.Api.Models.Movie;
using FluentAssertions;

namespace Proxarr.Api.Tests
{
    public class RadarrServiceTests
    {
        private readonly Mock<ILogger<RadarrService>> _loggerMock;
        private readonly Mock<ITmdbProxy> _tmdbClientMock;
        private readonly Mock<IOptions<AppConfiguration>> _appConfigMock;
        private readonly Mock<RadarrClient> _radarrClientMock;
        private readonly RadarrService _radarrService;
        private readonly TagResource _qualifyTag;

        public RadarrServiceTests()
        {
            _loggerMock = new Mock<ILogger<RadarrService>>();
            _tmdbClientMock = new Mock<ITmdbProxy>();
            _appConfigMock = new Mock<IOptions<AppConfiguration>>();
            _radarrClientMock = new Mock<RadarrClient>(null);

            var appConfig = new AppConfiguration
            {
                Clients = [new ClientConfiguration { Application = "Sonarr", BaseUrl = "http://localhost", ApiKey = "fake_api_key" }],
                WatchProviders = "US:Netflix, FR: Amazon Prime Video, US:Netflix with ads",
            };

            _appConfigMock.Setup(x => x.Value).Returns(appConfig);

            _radarrService = new RadarrService(_loggerMock.Object, _tmdbClientMock.Object, _appConfigMock.Object, _radarrClientMock.Object);
            _qualifyTag = new TagResource { Id = 30, Label = _appConfigMock.Object.Value.TagName };
        }

        [Fact]
        public async Task Ctor_ShouldThrowArgNullEx_WhenAppConfigurationNull()
        {
            Action act = () => new RadarrService(null, null, null, null);
            act.Should().Throw<ArgumentNullException>().WithParameterName("appConfiguration");
        }

        [Fact]
        public async Task Qualify_ShouldReturnNotFound_WhenMovieNotFoundIntoRadarr()
        {
            // Arrange
            var movieAdded = new MovieAdded
            {
                ApplicationUrl = "http://localhost",
                EventType = "FULL_SCAN",
                Movie = new Movie { Id = 1, TmdbId = 123, Title = "Test Movie" }
            };
            var cancellationToken = new CancellationToken();

            _tmdbClientMock.Setup(x => x.GetMovieAsync(123, cancellationToken, MovieMethods.WatchProviders))
                .ReturnsAsync(new TMDbLib.Objects.Movies.Movie { WatchProviders = new SingleResultContainer<Dictionary<string, WatchProviders>> { Results = [] } });

            _radarrClientMock.Setup(x => x.MovieGET2Async(1, cancellationToken))
                .ReturnsAsync((MovieResource)null);

            // Act
            var result = await _radarrService.Qualify(movieAdded, cancellationToken);

            // Assert
            Assert.Equal("NotFound", result);
        }

        [Fact]
        public async Task Qualify_ShouldReturnNotFound_WhenMovieNotFoundIntoTMDB()
        {
            // Arrange
            var movieAdded = new MovieAdded
            {
                ApplicationUrl = "http://localhost",
                EventType = "FULL_SCAN",
                Movie = new Movie { Id = 1, TmdbId = 123, Title = "Test Movie" }
            };
            var cancellationToken = new CancellationToken();

            _tmdbClientMock.Setup(x => x.GetMovieAsync(123, cancellationToken, MovieMethods.WatchProviders))
                .ReturnsAsync((TMDbLib.Objects.Movies.Movie)null);

            _radarrClientMock.Setup(x => x.MovieGET2Async(1, cancellationToken))
                .ReturnsAsync((MovieResource)null);

            // Act
            var result = await _radarrService.Qualify(movieAdded, cancellationToken);

            // Assert
            Assert.Equal("NotFound", result);
        }

        [Fact]
        public async Task Qualify_ShouldUpdateTags_WhenMovieFound()
        {
            // Arrange
            var tvAdded = new MovieAdded
            {
                ApplicationUrl = "http://localhost",
                EventType = "MovieAdded",
                Movie = new Movie { Id = 1, TmdbId = 123, Title = "Test Movie" },
            };
            var cancellationToken = new CancellationToken();

            var watchProviders = new SingleResultContainer<Dictionary<string, WatchProviders>>
            {
                Results = new Dictionary<string, WatchProviders>
                {
                    { "US", new WatchProviders { Free = [new WatchProviderItem { ProviderName = "Netflix" }], FlatRate = [new WatchProviderItem { ProviderName = "Netflix with Ads" }] } }
                }
            };

            var seriesResource = new MovieResource { Id = 1, Title = "Test Series", Tags = [] };

            _tmdbClientMock.Setup(x => x.GetMovieAsync(123, cancellationToken, MovieMethods.WatchProviders))
                .ReturnsAsync(new TMDbLib.Objects.Movies.Movie { WatchProviders = watchProviders });

            _radarrClientMock.Setup(x => x.MovieGET2Async(1, cancellationToken))
                .ReturnsAsync(seriesResource);

            _radarrClientMock.Setup(x => x.TagAllAsync(cancellationToken))
                .ReturnsAsync([]);

            _radarrClientMock.Setup(x => x.TagPOSTAsync(It.IsAny<TagResource>(), cancellationToken))
                .ReturnsAsync(new TagResource { Id = 1, Label = "Netflix" });

            // Act
            var result = await _radarrService.Qualify(tvAdded, cancellationToken);

            // Assert
            _radarrClientMock.Verify(x => x.MoviePUTAsync(false, "1", seriesResource, cancellationToken), Times.Once);
            Assert.Empty(result);
        }

        [Fact]
        public async Task Qualify_Should_NotBeTagged_When_MatchedWatchProvider()
        {
            // Arrange
            var tvAdded = new MovieAdded
            {
                ApplicationUrl = "http://localhost",
                EventType = "MovieAdded",
                Movie = new Movie { Id = 1, TmdbId = 123, Title = "Test Movie" },
            };
            var cancellationToken = new CancellationToken();

            var watchProviders = new SingleResultContainer<Dictionary<string, WatchProviders>>
            {
                Results = new Dictionary<string, WatchProviders>
                {
                    { "US", new WatchProviders { Free = [new WatchProviderItem { ProviderName = "Netflix" }] } }
                }
            };

            var movieResource = new MovieResource { Id = 1, Title = "Test Series", Tags = [] };

            _tmdbClientMock.Setup(x => x.GetMovieAsync(123, cancellationToken, MovieMethods.WatchProviders))
                .ReturnsAsync(new TMDbLib.Objects.Movies.Movie { WatchProviders = watchProviders });

            _radarrClientMock.Setup(x => x.MovieGET2Async(1, cancellationToken))
                .ReturnsAsync(movieResource);

            _radarrClientMock.Setup(x => x.TagAllAsync(cancellationToken))
                .ReturnsAsync([_qualifyTag]);

            var tagResource = new TagResource { Id = 1, Label = "Netflix" };

            _radarrClientMock.Setup(x => x.TagPOSTAsync(It.IsAny<TagResource>(), cancellationToken))
                .ReturnsAsync(tagResource);

            // Act
            var result = await _radarrService.Qualify(tvAdded, cancellationToken);

            // Assert
            _radarrClientMock.Verify(x => x.MoviePUTAsync(false, "1", movieResource, cancellationToken), Times.Once);
            Assert.Empty(result);
            _radarrClientMock.Verify(x => x.TagPOSTAsync(It.Is<TagResource>(x => x.Label == tagResource.Slugify()!.Label), cancellationToken), Times.Once);
            Assert.Single(movieResource.Tags);
            Assert.True(movieResource.Tags.All(x => x != _qualifyTag.Id));
        }

        [Fact]
        public async Task Qualify_Should_BeTagged_When_MatchedWatchProvider()
        {
            // Arrange
            var tvAdded = new MovieAdded
            {
                ApplicationUrl = "http://localhost",
                EventType = "MovieAdded",
                Movie = new Movie { Id = 1, TmdbId = 123, Title = "Test Movie" },
            };
            var cancellationToken = new CancellationToken();

            var watchProviders = new SingleResultContainer<Dictionary<string, WatchProviders>>
            {
                Results = new Dictionary<string, WatchProviders>
                {
                    { "FR", new WatchProviders { Free = [new WatchProviderItem { ProviderName = "Netflix" }] } }
                }
            };

            var movieResource = new MovieResource { Id = 1, Title = "Test Series", Tags = [] };

            _tmdbClientMock.Setup(x => x.GetMovieAsync(123, cancellationToken, MovieMethods.WatchProviders))
                .ReturnsAsync(new TMDbLib.Objects.Movies.Movie { WatchProviders = watchProviders });

            _radarrClientMock.Setup(x => x.MovieGET2Async(1, cancellationToken))
                .ReturnsAsync(movieResource);

            _radarrClientMock.Setup(x => x.TagAllAsync(cancellationToken))
                .ReturnsAsync([_qualifyTag]);

            var tagResource = new TagResource { Id = 1, Label = "Netflix" };

            _radarrClientMock.Setup(x => x.TagPOSTAsync(It.IsAny<TagResource>(), cancellationToken))
                .ReturnsAsync(tagResource);

            // Act
            var result = await _radarrService.Qualify(tvAdded, cancellationToken);

            // Assert
            _radarrClientMock.Verify(x => x.MoviePUTAsync(false, "1", movieResource, cancellationToken), Times.Once);
            Assert.Empty(result);
            Assert.Single(movieResource.Tags);
            Assert.Contains(movieResource.Tags, x => x == _qualifyTag.Id);
        }

        [Fact]
        public async Task Qualify_Should_BeTagged_When_NoWatchProviders()
        {
            // Arrange
            var tvAdded = new MovieAdded
            {
                ApplicationUrl = "http://localhost",
                EventType = "MovieAdded",
                Movie = new Movie { Id = 1, TmdbId = 123, Title = "Test Movie" },
            };
            var cancellationToken = new CancellationToken();

            var movieResource = new MovieResource { Id = 1, Title = "Test Series", Tags = [] };

            _tmdbClientMock.Setup(x => x.GetMovieAsync(123, cancellationToken, MovieMethods.WatchProviders))
                .ReturnsAsync(new TMDbLib.Objects.Movies.Movie());

            _radarrClientMock.Setup(x => x.MovieGET2Async(1, cancellationToken))
                .ReturnsAsync(movieResource);

            _radarrClientMock.Setup(x => x.TagAllAsync(cancellationToken))
                .ReturnsAsync([_qualifyTag]);

            var tagResource = new TagResource { Id = 1, Label = "Netflix" };

            _radarrClientMock.Setup(x => x.TagPOSTAsync(It.IsAny<TagResource>(), cancellationToken))
                .ReturnsAsync(tagResource);

            // Act
            var result = await _radarrService.Qualify(tvAdded, cancellationToken);

            // Assert
            _radarrClientMock.Verify(x => x.MoviePUTAsync(false, "1", movieResource, cancellationToken), Times.Once);
            Assert.Empty(result);
            Assert.Single(movieResource.Tags);
            Assert.Contains(movieResource.Tags, x => x == _qualifyTag.Id);
        }
    }
}