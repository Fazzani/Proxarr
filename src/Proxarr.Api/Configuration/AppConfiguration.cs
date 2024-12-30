using System.Diagnostics.CodeAnalysis;

namespace Proxarr.Api.Configuration
{
    [ExcludeFromCodeCoverage]
    public sealed class AppConfiguration
    {
        public const string SECTION_NAME = nameof(AppConfiguration);

        [ConfigurationKeyName("LOG_FOLDER")]
        public string? LogFolder { get; set; }

        [ConfigurationKeyName("TAG_NAME")]
        public string? TagName { get; set; } = "q";

        [ConfigurationKeyName("TMDB_API_KEY")]
        public string? TmdbApiKey { get; set; }

        [ConfigurationKeyName("FULL_SCAN_CRON")]
        public string FullScanCron { get; set; } = "0 6 * * 1";

        public BasicAuthenticationConfiguration Authentication { get; set; }

        public List<ClientConfiguration> Clients { get; set; }

        private Dictionary<string, string[]> _watchProviders;

        /// <summary>
        /// Transform Watch providers (ex: US:Netflix,US:Amazon Prime Video) to a dictionary
        /// grouped by region
        /// </summary>
        public Dictionary<string, string[]> WatchProvidersDict
        {
            get
            {
                _watchProviders ??= InitProviders();
                return _watchProviders;
            }
        }

        [ConfigurationKeyName("WATCH_PROVIDERS")]
        public string WatchProviders { get; set; }

        private Dictionary<string, string[]> InitProviders()
        {
            ArgumentNullException.ThrowIfNull(WatchProviders);

            var dict = new Dictionary<string, string[]>();

            foreach (var item in WatchProviders!.Split(','))
            {
                var parts = item.Split(':');
                if (parts.Length != 2)
                {
                    throw new FormatException("Malformed WATCH_PROVIDERS! Must follow this format: REGION:WatchProvider, REGION:WatchProvider, ... ex (US:Netflix,FR:Youtube)");
                }
                if (!dict.TryGetValue(parts[0], out string[]? value))
                {
                    dict.Add(parts[0].Trim(), [parts[1].Trim()]);
                }
                else
                {
                    dict[parts[0]] = [.. value, parts[1].Trim()];
                }
            }

            return dict;
        }
    }
}
