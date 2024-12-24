using Proxarr.Api.Configuration;

namespace Proxarr.Api.Core
{
    public class ApiKeyDelegatingHandler : DelegatingHandler
    {
        private const string API_KEY_HEADER_NAME = "X-API-KEY";

        private readonly IConfiguration _configuration;

        public ApiKeyDelegatingHandler(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!request.Headers.Contains(API_KEY_HEADER_NAME) && request.RequestUri != null)
            {
                var key = $"{request.RequestUri.Scheme}://{request.RequestUri.Host}";
                var clients = _configuration.GetSection(ClientConfiguration.SECTION_NAME).Get<List<ClientConfiguration>>();
                var clientConfig = clients?.FirstOrDefault(x => x.BaseUrl.Equals(key, StringComparison.OrdinalIgnoreCase));

                if (clientConfig != null)
                {
                    request.Headers.Add(API_KEY_HEADER_NAME, clientConfig.ApiKey);
                }
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
