using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;
using System.Text;

namespace Stack.Web
{
    public class RestClient : IDisposable
    {
        public RestClient(RestClientOptions options)
        {
            Options = options;
        }

        public virtual async Task<T> GetAsync<T>(object resource = null)
            where T : class
        {
            using (HttpResponseMessage response = await Client.GetAsync(GetResourceUri(resource)))
            {
                await CheckResponseStatusAsync(response);
                return await GetResult<T>(response);
            }
        }
        public virtual async Task<T> GetAsync<T>(PageOptions page = null, SortOptions[] sort = null)
            where T : class
        {
            return await GetAsync<T, Filter>(page: page, sort: sort);
        }
        public virtual async Task<T> GetAsync<T, TFilter>(TFilter filter = null, PageOptions page = null, SortOptions[] sort = null)
            where T : class
            where TFilter : Filter
        {
            using (HttpResponseMessage response = await Client.GetAsync(GetResourceUriWithQueryString(filter, page, sort)))
            {
                await CheckResponseStatusAsync(response);
                return await GetResult<T>(response);
            }
        }

        public virtual async Task<TResult> PostAsync<TRequest, TResult>(TRequest data)
            where TRequest : class
            where TResult : class
        {
            using (HttpContent content = GetContent(data))
            {
                using (HttpResponseMessage response = await Client.PostAsync(string.Empty, content))
                {
                    await CheckResponseStatusAsync(response);
                    return await GetResult<TResult>(response);
                }
            }
        }
        public virtual async Task<TResult> PutAsync<TRequest, TResult>(object resource, TRequest data)
            where TRequest : class
            where TResult : class
        {
            using (HttpContent content = GetContent(data))
            {
                using (HttpResponseMessage response = await Client.PutAsync(GetResourceUri(resource), content))
                {
                    await CheckResponseStatusAsync(response);
                    return await GetResult<TResult>(response);
                }
            }
        }
        public virtual async Task DeleteAsync(object resource)
        {
            using (HttpResponseMessage response = await Client.DeleteAsync(GetResourceUri(resource)))
            {
                await CheckResponseStatusAsync(response);
            }
        }

        public virtual void Dispose()
        {
            if (client != null)
            {
                client.Dispose();
            }
        }

        #region Protected members
        protected RestClientOptions Options { get; private set; }
        #endregion

        #region Private members
        private string GetResourceUri(object resource = null)
        {
            return resource != null ? new Uri(new Uri(Options.BaseUrl), Convert.ToString(resource)).ToString() : string.Empty;
        }
        private string GetResourceUriWithQueryString<T>(T filter = null, PageOptions page = null, SortOptions[] sort = null)
            where T : Filter
        {
            QueryStringSerializer serializer = new QueryStringSerializer();
            string filterQuery = filter != null ? serializer.Serialize(filter) : null;
            string pageQuery = page != null ? serializer.Serialize(page) : null;
            string sortQuery = sort != null ? serializer.Serialize(sort) : null;
            string query = string.Join("&", filterQuery, pageQuery, sortQuery);

            return !string.IsNullOrEmpty(query) ? $"{Options.BaseUrl}?{query}" : string.Empty;
        }

        private async Task<T> GetResult<T>(HttpResponseMessage response)
            where T : class
        {
            string content = await response.Content.ReadAsStringAsync();

            var contractResolver = new DefaultContractResolver();
            if (Options.JsonNaming == JsonPropertyNaming.Underscore)
            {
                contractResolver.NamingStrategy = new SnakeCaseNamingStrategy();
            }

            return JsonConvert.DeserializeObject<T>(content, new JsonSerializerSettings()
            {
                ContractResolver = contractResolver
            });
        }
        private StringContent GetContent(object data)
        {
            return new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, Options.MediaType);
        }
        private async Task CheckResponseStatusAsync(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                HttpStatusCode status = response.StatusCode;
                string content = null;
                try
                {
                    content = await response.Content.ReadAsStringAsync();
                    response.EnsureSuccessStatusCode();
                }
                catch (Exception e)
                {
                    throw new RestRequestException(content, e, status);
                }
            }
        }

        private HttpClient Client
        {
            get
            {
                if (client == null)
                {
                    client = new HttpClient();
                    client.BaseAddress = new Uri(Options.BaseUrl);
                    client.Timeout = new TimeSpan(0, 0, Options.Timeout);

                    if (Options.RequiresAuth())
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                            Options.AuthScheme,
                            Options.AuthToken);
                    }
                }
                return client;
            }
        }

        private HttpClient client;
        #endregion
    }
}
