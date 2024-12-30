using System.Diagnostics.CodeAnalysis;

namespace Proxarr.Api.Configuration
{
    [ExcludeFromCodeCoverage]
    public class BasicAuthenticationConfiguration
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }
}