using System;

namespace Stack.Web
{
    public class RestClientOptions
    {
        public RestClientOptions(
            string baseUrl,
            string authToken = null,
            string mediaType = JsonMediaType,
            JsonPropertyNaming jsonNaming = JsonPropertyNaming.Default,
            int? timeout = null)
        {
            Assure.NotEmpty(baseUrl, nameof(baseUrl));
            Assure.NotEmpty(mediaType, nameof(mediaType));

            if (!baseUrl.EndsWith("/"))
            {
                baseUrl += "/";
            }

            if (!Uri.IsWellFormedUriString(baseUrl, UriKind.Absolute))
            {
                throw new ArgumentException($"{nameof(baseUrl)} is not a valid URL.");
            }

            if (timeout.HasValue && (timeout.Value > 60 || timeout.Value < 1))
            {
                timeout = null;
            }

            BaseUrl = baseUrl;
            AuthScheme = !string.IsNullOrEmpty(authToken) ? BearerAuthScheme : null;
            AuthToken = authToken;
            MediaType = mediaType;
            JsonNaming = jsonNaming;
            Timeout = timeout ?? DefaultTimeout;
        }

        public const string BasicAuthScheme = "Basic";
        public const string BearerAuthScheme = "Bearer";
        public const string JsonMediaType = "application/json";
        public const int DefaultTimeout = 8;

        public string BaseUrl { get; set; }
        public string AuthScheme { get; set; }
        public string AuthToken { get; set; }
        public string MediaType { get; set; }
        public JsonPropertyNaming JsonNaming { get; set; }
        public int Timeout { get; set; }

        public bool RequiresAuth()
        {
            return !string.IsNullOrEmpty(AuthScheme) && !string.IsNullOrEmpty(AuthToken);
        }
    }
}
