using Proxarr.Api.Configuration;

namespace Proxarr.Api.Core
{
    public class ApiKeyDelegatingHandler : DelegatingHandler
    {
        private readonly IConfiguration _configuration;
        private const string apiKeyHeaderName = "X-API-KEY";

        public ApiKeyDelegatingHandler(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!request.Headers.Contains(apiKeyHeaderName) && request.RequestUri != null)
            {
                var key = $"{request.RequestUri.Scheme}://{request.RequestUri.Host}";
                var clients = _configuration.GetSection("Clients").Get<List<ClientConfiguration>>();
                var clientConfig = clients?.FirstOrDefault(x => x.BaseUrl.Equals(key, StringComparison.OrdinalIgnoreCase));

                if (clientConfig != null)
                {
                    request.Headers.Add(apiKeyHeaderName, clientConfig.ApiKey);
                }
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
