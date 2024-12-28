using Microsoft.Extensions.Options;
using Proxarr.Api.Configuration;

namespace Proxarr.Api.Core
{
    public class ApiKeyDelegatingHandler : DelegatingHandler
    {
        private const string API_KEY_HEADER_NAME = "X-API-KEY";

        private readonly AppConfiguration _configuration;
        private readonly ILogger<ApiKeyDelegatingHandler> _logger;

        public ApiKeyDelegatingHandler(IOptions<AppConfiguration> configuration, ILogger<ApiKeyDelegatingHandler> logger)
        {
            _configuration = configuration?.Value ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!request.Headers.Contains(API_KEY_HEADER_NAME) && request.RequestUri != null)
            {
                var key = $"{request.RequestUri.Scheme}://{request.RequestUri.Host}";
                var clientConfig = _configuration.Clients?.FirstOrDefault(x => x.BaseUrl.Equals(key, StringComparison.OrdinalIgnoreCase));

                if (clientConfig != null)
                {
                    _logger.LogInformation("Adding API Key to request for {BaseUrl}", clientConfig.BaseUrl);
                    request.Headers.Add(API_KEY_HEADER_NAME, clientConfig.ApiKey);
                }
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
