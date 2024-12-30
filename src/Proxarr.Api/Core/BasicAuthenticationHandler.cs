using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text;
using System.Security.Principal;
using Microsoft.AspNetCore.Authorization;
using Proxarr.Api.Configuration;

namespace Proxarr.Api.Core
{
    public class BasicAuthenticationDefaults
    {
        public const string AuthenticationScheme = "Basic";
    }

    public class BasicAuthorizationAttribute : AuthorizeAttribute
    {
        public BasicAuthorizationAttribute()
        {
            AuthenticationSchemes = BasicAuthenticationDefaults.AuthenticationScheme;
        }
    }

    public class BasicAuthenticationClient : IIdentity
    {
        public string? AuthenticationType { get; set; }

        public bool IsAuthenticated { get; set; }

        public string? Name { get; set; }
    }

    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly AppConfiguration? _appConfig;

        public BasicAuthenticationHandler(IOptions<AppConfiguration> appConfig, IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder) 
            : base(options, logger, encoder)
        {
            _appConfig = appConfig?.Value;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // No authorization header, so throw no result.
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return Task.FromResult(AuthenticateResult.Fail("Missing Authorization header"));
            }

            var authorizationHeader = Request.Headers.Authorization.ToString();

            // If authorization header doesn't start with basic, throw no result.
            if (!authorizationHeader.StartsWith(BasicAuthenticationDefaults.AuthenticationScheme, StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(AuthenticateResult.Fail("Authorization header does not start with 'Basic'"));
            }

            // Decrypt the authorization header and split out the client id/secret which is separated by the first ':'
            var authBase64Decoded = Encoding.UTF8.GetString(Convert.FromBase64String(authorizationHeader.Replace("Basic ", "", StringComparison.OrdinalIgnoreCase)));
            var authSplit = authBase64Decoded.Split([':'], 2);

            // No username and password, so throw no result.
            if (authSplit.Length != 2)
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization header format"));
            }

            // Store the client ID and secret
            var clientId = authSplit[0];
            var clientSecret = authSplit[1];

            // Client ID and secret are incorrect
            if (clientId != _appConfig?.Authentication?.Username || clientSecret != _appConfig?.Authentication?.Password)
            {
                return Task.FromResult(AuthenticateResult.Fail($"The secret is incorrect for the client '{clientId}'"));
            }

            // Authenticate the client using basic authentication
            var client = new BasicAuthenticationClient
            {
                AuthenticationType = BasicAuthenticationDefaults.AuthenticationScheme,
                IsAuthenticated = true,
                Name = clientId
            };

            // Set the client ID as the name claim type.
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(client,
            [
                new Claim(ClaimTypes.Name, clientId)
            ]));

            // Return a success result.
            return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, Scheme.Name)));
        }
    }
}
