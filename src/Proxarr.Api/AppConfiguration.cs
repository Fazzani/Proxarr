namespace Proxarr.Api
{
    public sealed class AppConfiguration
    {
        public const string SECTION_NAME = nameof(AppConfiguration);

        public string LOG_FOLDER { get; set; }

        public string TAG_NAME { get; set; }

        public string TMDB_API_KEY { get; set; }

        Dictionary<string, string[]> _watchProviders;

        /// <summary>
        /// Watch providers (ex: US:Netflix,US:Amazon Prime Video)
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

            foreach (var region in WATCH_PROVIDERS!.Split(','))
            {
                var parts = region.Split(':');
                if (parts.Length != 2)
                {
                    throw new FormatException("WATCH_PROVIDERS must ");
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
}
