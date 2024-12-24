namespace Proxarr.Api
{
    public sealed class AppConfiguration
    {
        public AppConfiguration()
        {
            var providers = Environment.GetEnvironmentVariable("WATCH_PROVIDERS");

            ArgumentNullException.ThrowIfNull(providers, "WATCH_PROVIDERS");

            foreach (var region in providers!.Split(','))
            {
                var parts = region.Split(':');
                if (parts.Length != 2)
                {
                    throw new ArgumentException("WATCH_PROVIDERS");
                }
                if (!WatchProviders.ContainsKey(parts[0]))
                {
                    WatchProviders.Add(parts[0], [parts[1]]);
                }
                else
                {
                    WatchProviders[parts[0]] = [.. WatchProviders[parts[0]], parts[1]];
                }
            }
        }

        public string QualifiedTagName { get; set; } = Environment.GetEnvironmentVariable("TAG_NAME") ?? "Q";

        /// <summary>
        /// Watch providers (ex: US:Netflix,US:Amazon Prime Video)
        /// </summary>
        public Dictionary<string, string[]> WatchProviders { get; set; } = [];
    }
}
