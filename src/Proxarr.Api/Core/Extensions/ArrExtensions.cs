namespace Proxarr.Api.Core.Extensions
{
    public static class ArrExtensions
    {

        public static Radarr.Http.Client.TagResource? Slugify(this Radarr.Http.Client.TagResource tagResource)
        {
            ArgumentNullException.ThrowIfNull(tagResource);

            tagResource.Label = tagResource.Label.Slugify();
            return tagResource;
        }

        public static Sonarr.Http.Client.TagResource Slugify(this Sonarr.Http.Client.TagResource tagResource) {

            ArgumentNullException.ThrowIfNull(tagResource);

            tagResource.Label = tagResource.Label.Slugify();
            return tagResource;
        }

        public static string Slugify(this string s) => s.Replace(" ", "_");
    }
}
