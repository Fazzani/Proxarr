using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Proxarr.Api.Configuration;
using Proxarr.Api.Core;
using Proxarr.Api.Models;
using Proxarr.Api.Services;
using Radarr.Http.Client;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Movies;
using Movie = Proxarr.Api.Models.Movie;

namespace Proxarr.Api.Tests
{
    public class RadarrServiceTests
    {
        private readonly Mock<ILogger<RadarrService>> _loggerMock;
        private readonly Mock<ITmdbProxy> _tmdbClientMock;
        private readonly Mock<IOptions<AppConfiguration>> _appConfigMock;
        private readonly Mock<RadarrClient> _radarrClientMock;
        private readonly RadarrService _radarrService;

        public RadarrServiceTests()
        {
            _loggerMock = new Mock<ILogger<RadarrService>>();
            _tmdbClientMock = new Mock<ITmdbProxy>();
            _appConfigMock = new Mock<IOptions<AppConfiguration>>();
            _radarrClientMock = new Mock<RadarrClient>(null);

            var appConfig = new AppConfiguration
            {
                Clients = [new ClientConfiguration { Application = "Sonarr", BaseUrl = "http://localhost", ApiKey = "fake_api_key" }],
                WatchProviders = "US:Netflix, FR: Amazon Prime Video",
            };

            _appConfigMock.Setup(x => x.Value).Returns(appConfig);

            _radarrService = new RadarrService(_loggerMock.Object, _tmdbClientMock.Object, _appConfigMock.Object, _radarrClientMock.Object);
        }

        [Fact]
        public async Task Qualify_ShouldReturnNotFound_WhenMovieNotFound()
        {
            // Arrange
            var movieAdded = new MovieAdded
            {
                ApplicationUrl = "http://localhost",
                EventType = "FULL_SCAN",
                Movie = new Movie { Id = 1, TmdbId = 123, Title = "Test Movie" }
            };
            var cancellationToken = new CancellationToken();

            _tmdbClientMock.Setup(x => x.GetMovieAsync(123, MovieMethods.WatchProviders, cancellationToken))
                .ReturnsAsync(new TMDbLib.Objects.Movies.Movie { WatchProviders = new SingleResultContainer<Dictionary<string, WatchProviders>> { Results = [] } });

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
                EventType = "FULL_SCAN",
                Movie = new Models.Movie { Id = 1, TmdbId = 123, Title = "Test Movie" },
            };
            var cancellationToken = new CancellationToken();

            var watchProviders = new SingleResultContainer<Dictionary<string, WatchProviders>>
            {
                Results = new Dictionary<string, WatchProviders>
                {
                    { "US", new WatchProviders { Free = [new WatchProviderItem { ProviderName = "Netflix" }] } }
                }
            };

            var seriesResource = new MovieResource { Id = 1, Title = "Test Series", Tags = [] };

            _tmdbClientMock.Setup(x => x.GetMovieAsync(123, MovieMethods.WatchProviders, cancellationToken))
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
    }
}