using System.Diagnostics.CodeAnalysis;

namespace Proxarr.Api.Configuration
{
    [ExcludeFromCodeCoverage]
    public sealed class ClientConfiguration
    {
        public const string SECTION_NAME = $"{AppConfiguration.SECTION_NAME}:Clients";

        /// <summary>
        /// Must be Sonarr or Radarr
        /// </summary>
        public required string Application { get; set; }
        public required string ApiKey { get; set; }
        public required string BaseUrl { get; set; }


        public bool IsSonarr => Application.Equals("Sonarr", StringComparison.OrdinalIgnoreCase);
        public bool IsRadarr => Application.Equals("Radarr", StringComparison.OrdinalIgnoreCase);
    }
}
