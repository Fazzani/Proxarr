using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Proxarr.Api.Controllers;
using Proxarr.Api.Models;
using Proxarr.Api.Services;

namespace Proxarr.Api.Tests
{
    public class QualifierControllerTests
    {
        private readonly Mock<ILogger<QualifierController>> _loggerMock;
        private readonly Mock<IRadarrService> _radarrServiceMock;
        private readonly Mock<ISonarrService> _sonarrServiceMock;
        private readonly QualifierController _controller;

        public QualifierControllerTests()
        {
            _loggerMock = new Mock<ILogger<QualifierController>>();
            _radarrServiceMock = new Mock<IRadarrService>();
            _sonarrServiceMock = new Mock<ISonarrService>();
            _controller = new QualifierController(_loggerMock.Object, _radarrServiceMock.Object, _sonarrServiceMock.Object);
        }

        [Fact]
        public async Task Qualified_ShouldReturnOk_WhenEventTypeIsTest()
        {
            // Arrange
            var media = new MediaAdded { EventType = "test", ApplicationUrl = "http://localhost" };
            var cancellationToken = new CancellationToken();

            // Act
            var result = await _controller.Qualified(media, cancellationToken);

            // Assert
            result.Should().BeOfType<OkResult>();
        }

        [Fact]
        public async Task Qualified_ShouldReturnOk_WhenEventTypeMovieAdded()
        {
            // Arrange
            var media = new MovieAdded { EventType = "MovieAdded", ApplicationUrl = "http://localhost", Movie=default };
            var cancellationToken = new CancellationToken();

            // Act
            var result = await _controller.Qualified(media, cancellationToken);

            // Assert
            result.Should().BeOfType<OkResult>();
        }

        [Fact]
        public async Task Qualified_ShouldReturnNotFound_WhenMovieNotFound()
        {
            // Arrange
            var movie = new MovieAdded { EventType = "add", ApplicationUrl = "http://localhost", Movie = new Movie { Id = 1, Title = "Test Movie", TmdbId = 66 } };
            var cancellationToken = new CancellationToken();

            _radarrServiceMock.Setup(x => x.Qualify(movie, cancellationToken)).ReturnsAsync("NotFound");

            // Act
            var result = await _controller.Qualified(movie, cancellationToken);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Qualified_ShouldReturnNotFound_WhenTvNotFound()
        {
            // Arrange
            var tvAdded = new TvAdded { EventType = "add", ApplicationUrl = "http://localhost", Series = new Series { Id = 2, Title = "Test Series" } };
            var cancellationToken = new CancellationToken();

            _sonarrServiceMock.Setup(x => x.Qualify(tvAdded, cancellationToken)).ReturnsAsync("NotFound");

            // Act
            var result = await _controller.Qualified(tvAdded, cancellationToken);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Qualified_ShouldReturnBadRequest_WhenMediaIsInvalid()
        {
            // Arrange
            var media = new MediaAdded { EventType = "invalid", ApplicationUrl = "http://localhost" };
            var cancellationToken = new CancellationToken();

            // Act
            var result = await _controller.Qualified(media, cancellationToken);

            // Assert
            result.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task Qualified_ShouldReturnOk_WhenMediaIsValid()
        {
            // Arrange
            var tvAdded = new TvAdded { EventType = "add", ApplicationUrl = "http://localhost", Series = new Series { Id = 33, Title = "Test Series" } };
            var cancellationToken = new CancellationToken();

            _sonarrServiceMock.Setup(x => x.Qualify(tvAdded, cancellationToken)).ReturnsAsync(string.Empty);

            // Act
            var result = await _controller.Qualified(tvAdded, cancellationToken);

            // Assert
            result.Should().BeOfType<OkResult>();
        }
    }
}