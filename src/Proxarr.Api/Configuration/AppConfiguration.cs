namespace Proxarr.Api.Configuration
{
    public sealed class AppConfiguration
    {
        public const string SECTION_NAME = nameof(AppConfiguration);

        public string? LOG_FOLDER { get; set; }

        public string? TAG_NAME { get; set; }

        public required string TMDB_API_KEY { get; set; }

        public string FULL_SCAN_CRON { get; set; } = "0 6 * * 1";

        public List<ClientInstance> Clients { get; set; }

        private Dictionary<string, string[]> _watchProviders;

        /// <summary>
        /// Transform Watch providers (ex: US:Netflix,US:Amazon Prime Video) to a dictionary
        /// grouped by region
        /// </summary>
        public Dictionary<string, string[]> WatchProviders
        {
            get
            {
                _watchProviders ??= InitProviders();
                return _watchProviders;
            }
        }

        public string WATCH_PROVIDERS { get; set; }

        private Dictionary<string, string[]> InitProviders()
        {
            ArgumentNullException.ThrowIfNull(WATCH_PROVIDERS);

            var dict = new Dictionary<string, string[]>();

            foreach (var item in WATCH_PROVIDERS!.Split(','))
            {
                var parts = item.Split(':');
                if (parts.Length != 2)
                {
                    throw new FormatException("Malformed WATCH_PROVIDERS! Must follow this format: REGION:WatchProvider, REGION:WatchProvider, ... ex (US:Netflix,FR:Youtube)");
                }
                if (!dict.TryGetValue(parts[0], out string[]? value))
                {
                    dict.Add(parts[0], [parts[1]]);
                }
                else
                {
                    dict[parts[0]] = [.. value, parts[1]];
                }
            }

            return dict;
        }
    }

    public sealed class ClientInstance
    {
        /// <summary>
        /// The name of the application (Sonarr or Radarr)
        /// </summary>
        public required string Application { get; set; }
        public required string BaseUrl { get; set; }
        public required string ApiKey { get; set; }

        public bool IsSonarr => Application.Equals("Sonarr", StringComparison.OrdinalIgnoreCase);
        public bool IsRadarr => Application.Equals("Radarr", StringComparison.OrdinalIgnoreCase);
    }
}
