using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace Mercola.Digital.Core.Api;
public abstract class ApiAdapterCore
{
    private readonly IHttpClientFactory _httpClientFactory;
    private const string AuthorizationBearer = "Bearer";
    private const string OcpApimSubKeyHeader = "Ocp-Apim-Subscription-Key";
    private const string PathDelimiter = "/";

    /// <summary>
    /// Retrieves the base address for the web API.
    /// </summary>
    protected abstract string BaseAddress { get; }

    /// <summary>
    /// Gets the value for the "Accept" header for the API calls. Default to <see cref="MediaTypeNames.Application.Json"/>.
    /// </summary>
    protected virtual string Accept => MediaTypeNames.Application.Json;

    /// <summary>
    /// Gets the value for the "ContentType" header for the API calls. Default to <see cref="MediaTypeNames.Application.Json"/>.
    /// </summary>
    protected virtual string ContentType => MediaTypeNames.Application.Json;

    /// <summary>
    /// Gets the encoding type for the "ContentType" header for the API calls.
    /// </summary>
    protected virtual Encoding Encoding { get; }

    /// <summary>
    /// Gets the security Bearer token used to access the API.
    /// </summary>
    protected virtual string BearerToken { get; }

    /// <summary>
    /// Gets the security Azure API Manager token used to access the API.
    /// </summary>
    protected virtual string ApiManagerToken { get; }

    /// <summary>
    /// The <see cref="Newtonsoft.Json.JsonSerializerSettings"/> used to deserialize the response object.
    /// If this is null, default serialization settings will be used.
    /// </summary>
    protected virtual JsonSerializerSettings JsonDeserializerSettings { get; }

    /// <summary>
    /// The <see cref="Newtonsoft.Json.JsonSerializerSettings"/> used to serialize the request object.
    /// If this is null, default serialization settings will be used.
    /// </summary>
    protected virtual JsonSerializerSettings JsonSerializerSettings { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="clientFactory"></param>
    protected ApiAdapterCore(IHttpClientFactory clientFactory)
    {
        _httpClientFactory = clientFactory;
    }

    /// <summary>
    /// Send a DELETE request to the specified Uri as an asynchronous operation, with a response object of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">Type of the response object.</typeparam>
    /// <param name="path">(Optional) The path of the request after <seealso cref="BaseAddress"/>.</param>
    /// <param name="cancellationToken">The cancellation token. Default is <see cref="CancellationToken.None"/>.</param>
    /// <param name="headers"><see cref="HeaderEntry"/> objects representing the request headers (optional).</param>
    /// <returns>The operation response as an object of type <typeparamref name="T"/>.</returns>
    /// <remarks>Type {T} cannot be of type <see cref="HttpResponseMessage"/>.</remarks>
    protected virtual async Task<T> DeleteAsync<T>(string path, CancellationToken cancellationToken = default, params HeaderEntry[] headers)
    {
        if (typeof(T) == typeof(HttpResponseMessage))
            throw new ArgumentException($"Invalid type '{typeof(HttpResponseMessage)}'. Please use the non-generic method 'Task<HttpResponseMessage> DeleteAsync()' instead.");

        using HttpResponseMessage response = await DeleteAsync(path, cancellationToken, headers);

        return await ProcessResponseAsync<T>(response, cancellationToken);
    }

    /// <summary>
    /// Send a DELETE request to the specified Uri as an asynchronous operation, which returns a generic <see cref="HttpResponseMessage"/> response object.
    /// </summary>
    /// <param name="path">(Optional) The path of the request after <seealso cref="BaseAddress"/>.</param>
    /// <param name="cancellationToken">The cancellation token. Default is <see cref="CancellationToken.None"/>.</param>
    /// <param name="headers"><see cref="HeaderEntry"/> objects representing the request headers (optional).</param>
    /// <returns>The operation response as an <see cref="HttpResponseMessage"/> object.</returns>
    protected virtual Task<HttpResponseMessage> DeleteAsync(string path, CancellationToken cancellationToken = default, params HeaderEntry[] headers)
        => SendAsync(HttpMethod.Delete, path, null, cancellationToken, headers);

    /// <summary>
    /// Sends a PATCH request to the specified Uri as an asynchronous operation, with a response object of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">Type of the response object.</typeparam>
    /// <param name="path">(Optional) The path of the request after <seealso cref="BaseAddress"/>.</param>
    /// <param name="content">The HTTP request content sent to the server.</param>
    /// <param name="cancellationToken">The cancellation token. Default is <see cref="CancellationToken.None"/>.</param>
    /// <param name="headers"><see cref="HeaderEntry"/> objects representing the request headers (optional).</param>
    /// <returns>The operation response as an object of type <typeparamref name="T"/>.</returns>
    /// <remarks>Type {T} cannot be of type <see cref="HttpResponseMessage"/>.</remarks>
    protected virtual async Task<T> PatchAsync<T>(string path, object content, CancellationToken cancellationToken = default, params HeaderEntry[] headers)
    {
        if (typeof(T) == typeof(HttpResponseMessage))
            throw new ArgumentException($"Invalid type '{typeof(HttpResponseMessage)}'. Please use the non-generic method 'Task<HttpResponseMessage> PatchAsync()' instead.");

        using HttpResponseMessage response = await PatchAsync(path, content, cancellationToken, headers);

        return await ProcessResponseAsync<T>(response, cancellationToken);
    }

    /// <summary>
    /// Sends a PATCH request to the specified Uri as an asynchronous operation, which returns a generic <see cref="HttpResponseMessage"/> response object.
    /// </summary>
    /// <param name="path">(Optional) The path of the request after <seealso cref="BaseAddress"/>.</param>
    /// <param name="content">The HTTP request content sent to the server.</param>
    /// <param name="cancellationToken">The cancellation token. Default is <see cref="CancellationToken.None"/>.</param>
    /// <param name="headers"><see cref="HeaderEntry"/> objects representing the request headers (optional).</param>
    /// <returns>The operation response as an <see cref="HttpResponseMessage"/> object.</returns>
    protected virtual Task<HttpResponseMessage> PatchAsync(string path, object content, CancellationToken cancellationToken = default, params HeaderEntry[] headers)
        => SendAsync(HttpMethod.Patch, path, content, cancellationToken, headers);

    /// <summary>
    /// Send a GET request to the specified Uri as an asynchronous operation, with a response object of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">Type of the response object.</typeparam>
    /// <param name="path">(Optional) The path of the request after <seealso cref="BaseAddress"/>.</param>
    /// <param name="cancellationToken">The cancellation token. Default is <see cref="CancellationToken.None"/>.</param>
    /// <param name="headers"><see cref="HeaderEntry"/> objects representing the request headers (optional).</param>
    /// <returns>The operation response as an object of type <typeparamref name="T"/>.</returns>
    /// <remarks>Type {T} cannot be of type <see cref="HttpResponseMessage"/>.</remarks>
    protected virtual async Task<T> GetAsync<T>(string path, CancellationToken cancellationToken = default, params HeaderEntry[] headers)
    {
        if (typeof(T) == typeof(HttpResponseMessage))
            throw new ArgumentException($"Invalid type '{typeof(HttpResponseMessage)}'. Please use the non-generic method 'Task<HttpResponseMessage> GetAsync()' instead.");

        using HttpResponseMessage response = await GetAsync(path, cancellationToken, headers);

        return await ProcessResponseAsync<T>(response, cancellationToken);
    }

    /// <summary>
    /// Send a GET request to the specified Uri as an asynchronous operation, which returns a generic <see cref="HttpResponseMessage"/> response object.
    /// </summary>
    /// <param name="path">(Optional) The path of the request after <seealso cref="BaseAddress"/>.</param>
    /// <param name="cancellationToken">The cancellation token. Default is <see cref="CancellationToken.None"/>.</param>
    /// <param name="headers"><see cref="HeaderEntry"/> objects representing the request headers (optional).</param>
    /// <returns>The operation response as an <see cref="HttpResponseMessage"/> object.</returns>
    protected virtual Task<HttpResponseMessage> GetAsync(string path, CancellationToken cancellationToken = default, params HeaderEntry[] headers)
        => SendAsync(HttpMethod.Get, path, null, cancellationToken, headers);

    /// <summary>
    /// Send a POST request to the specified Uri as an asynchronous operation, with a response object of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">Type of the response object.</typeparam>
    /// <param name="path">(Optional) The path of the request after <seealso cref="BaseAddress"/>.</param>
    /// <param name="content">The HTTP request content sent to the server.</param>
    /// <param name="cancellationToken">The cancellation token. Default is <see cref="CancellationToken.None"/>.</param>
    /// <param name="headers"><see cref="HeaderEntry"/> objects representing the request headers (optional).</param>
    /// <returns>The operation response as an object of type <typeparamref name="T"/>.</returns>
    /// <remarks>Type {T} cannot be of type <see cref="HttpResponseMessage"/>.</remarks>
    protected virtual async Task<T> PostAsync<T>(string path, object content, CancellationToken cancellationToken = default, params HeaderEntry[] headers)
    {
        if (typeof(T) == typeof(HttpResponseMessage))
            throw new ArgumentException($"Invalid type '{typeof(HttpResponseMessage)}'. Please use the non-generic method 'Task<HttpResponseMessage> PostAsync()' instead.");

        using HttpResponseMessage response = await PostAsync(path, content, cancellationToken, headers);

        return await ProcessResponseAsync<T>(response, cancellationToken);
    }

    /// <summary>
    /// Send a POST request to the specified Uri as an asynchronous operation, which returns a generic <see cref="HttpResponseMessage"/> response object.
    /// </summary>
    /// <param name="path">(Optional) The path of the request after <seealso cref="BaseAddress"/>.</param>
    /// <param name="content">The HTTP request content sent to the server.</param>
    /// <param name="cancellationToken">The cancellation token. Default is <see cref="CancellationToken.None"/>.</param>
    /// <param name="headers"><see cref="HeaderEntry"/> objects representing the request headers (optional).</param>
    /// <returns>The operation response as an <see cref="HttpResponseMessage"/> object.</returns>
    protected virtual Task<HttpResponseMessage> PostAsync(string path, object content, CancellationToken cancellationToken = default, params HeaderEntry[] headers)
        => SendAsync(HttpMethod.Post, path, content, cancellationToken, headers);

    /// <summary>
    /// Send a PUT request to the specified Uri as an asynchronous operation, with a response object of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">Type of the response object.</typeparam>
    /// <param name="path">(Optional) The path of the request after <seealso cref="BaseAddress"/>.</param>
    /// <param name="content">The HTTP request content sent to the server.</param>
    /// <param name="cancellationToken">The cancellation token. Default is <see cref="CancellationToken.None"/>.</param>
    /// <param name="headers"><see cref="HeaderEntry"/> objects representing the request headers (optional).</param>
    /// <returns>The operation response as an object of type <typeparamref name="T"/>.</returns>
    /// <remarks>Type {T} cannot be of type <see cref="HttpResponseMessage"/>.</remarks>
    protected virtual async Task<T> PutAsync<T>(string path, object content, CancellationToken cancellationToken = default, params HeaderEntry[] headers)
    {
        if (typeof(T) == typeof(HttpResponseMessage))
            throw new ArgumentException($"Invalid type '{typeof(HttpResponseMessage)}'. Please use the non-generic method 'Task<HttpResponseMessage> PutAsync()' instead.");

        using HttpResponseMessage response = await PutAsync(path, content, cancellationToken, headers);

        return await ProcessResponseAsync<T>(response, cancellationToken);
    }

    /// <summary>
    /// Send a PUT request to the specified Uri as an asynchronous operation, which returns a generic <see cref="HttpResponseMessage"/> response object.
    /// </summary>
    /// <param name="path">(Optional) The path of the request after <seealso cref="BaseAddress"/>.</param>
    /// <param name="content">The HTTP request content sent to the server.</param>
    /// <param name="cancellationToken">The cancellation token. Default is <see cref="CancellationToken.None"/>.</param>
    /// <param name="headers"><see cref="HeaderEntry"/> objects representing the request headers (optional).</param>
    /// <returns>The operation response as an <see cref="HttpResponseMessage"/> object.</returns>
    protected virtual Task<HttpResponseMessage> PutAsync(string path, object content, CancellationToken cancellationToken = default, params HeaderEntry[] headers)
        => SendAsync(HttpMethod.Put, path, content, cancellationToken, headers);

    /// <summary>
    /// Send an HTTP request as an asynchronous operation, with a response object of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">Type of the response object.</typeparam>
    /// <param name="method"><see cref="HttpMethod"/> object representing the HTTP method.</param>
    /// <param name="path">(Optional) The path of the request after <seealso cref="BaseAddress"/>.</param>
    /// <param name="content">The HTTP request content sent to the server.</param>
    /// <param name="cancellationToken">The cancellation token. Default is <see cref="CancellationToken.None"/>.</param>
    /// <param name="headers"><see cref="HeaderEntry"/> objects representing the request headers (optional).</param>
    /// <returns>The operation response as an object of type <typeparamref name="T"/>.</returns>
    /// <remarks>Type {T} cannot be of type <see cref="HttpResponseMessage"/>.</remarks>
    protected virtual async Task<T> SendAsync<T>(HttpMethod method, string path, object content, CancellationToken cancellationToken = default, params HeaderEntry[] headers)
    {
        if (typeof(T) == typeof(HttpResponseMessage))
            throw new ArgumentException($"Invalid type '{typeof(HttpResponseMessage)}'. Please use the non-generic method 'Task<HttpResponseMessage> SendAsync()' instead.");

        using HttpResponseMessage response = await SendAsync(method, path, content, cancellationToken, headers);

        return await ProcessResponseAsync<T>(response, cancellationToken);
    }

    /// <summary>
    /// Send an HTTP request as an asynchronous operation, which returns a generic <see cref="HttpResponseMessage"/> response object.
    /// </summary>
    /// <param name="method"><see cref="HttpMethod"/> object representing the HTTP method.</param>
    /// <param name="path">(Optional) The path of the request after <seealso cref="BaseAddress"/>.</param>
    /// <param name="content">The HTTP request content sent to the server.</param>
    /// <param name="cancellationToken">The cancellation token. Default is <see cref="CancellationToken.None"/>.</param>
    /// <param name="headers"><see cref="HeaderEntry"/> objects representing the request headers (optional).</param>
    /// <returns>The operation response as an <see cref="HttpResponseMessage"/> object.</returns>
    protected virtual async Task<HttpResponseMessage> SendAsync(HttpMethod method, string path, object content, CancellationToken cancellationToken = default, params HeaderEntry[] headers)
    {
        string requestUri = ParseUri(path);
        using HttpClient client = GetHttpClient();
        using HttpRequestMessage request = new HttpRequestMessage(method, requestUri);

        // Add headers from parameters
        foreach (HeaderEntry header in headers)
            request.Headers.Add(header.Key, header.Value);

        if (content != null)
            request.Content = CreateStringContent(content);

        return await client.SendAsync(request, cancellationToken);
    }

    /// <summary>
    /// Creates a new <see cref="HttpClient"/> using the <see cref="IHttpClientFactory"/> implementation.
    /// </summary>
    /// <returns>An <see cref="HttpClient"/> configured using the <see cref="IHttpClientFactory"/> implementation.</returns>
    protected virtual HttpClient GetHttpClient()
    {
        HttpClient client = _httpClientFactory.CreateClient();

        client.BaseAddress = new Uri(BaseAddress.EndsWith(PathDelimiter) ? BaseAddress : BaseAddress + PathDelimiter);

        // Clear request headers (these are sent with each request)
        client.DefaultRequestHeaders.Accept.Clear();

        if (!string.IsNullOrWhiteSpace(Accept))
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Accept));

        if (!string.IsNullOrWhiteSpace(BearerToken))
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthorizationBearer, BearerToken);

        if (!string.IsNullOrWhiteSpace(ApiManagerToken))
            client.DefaultRequestHeaders.Add(OcpApimSubKeyHeader, ApiManagerToken);

        return client;
    }

    /// <summary>
    /// Creates a <see cref="StringContent"/> object containing the JSON content from <paramref name="content"/>.
    /// </summary>
    /// <param name="content">The content as a JSON string.</param>
    /// <returns>The <see cref="StringContent"/> object.</returns>
    protected virtual StringContent CreateStringContent(object content)
    {
        // Serialize the content object using the specified JsonSerializerSettings.
        // If JsonSerializerSettings null, the default serialization settings will be used
        // If the content is already a string, don't serialize it again. This will protect us
        // in case the caller has already serialized it.
        string json = content as string ?? JsonConvert.SerializeObject(content, JsonSerializerSettings);

        // Instantiate a StringContent object without specifying Encoding or MediaType (set separately below)
        // See https://github.com/microsoft/referencesource/blob/master/System/net/System/Net/Http/StringContent.cs
        StringContent stringContent = new StringContent(json);

        // Set the ContentType using MediaType: this won't set a value for CharSet
        stringContent.Headers.ContentType = new MediaTypeHeaderValue(ContentType);

        // Set the CharSet if Encoding is specified
        if (Encoding != null)
            stringContent.Headers.ContentType.CharSet = Encoding.WebName;

        return stringContent;
    }

    /// <summary>
    /// Processes the <see cref="HttpResponseMessage"/> and deserializes the content into an object of type <typeparamref name="T"/>.
    /// If the response is not successful (meaning, outside the range 200-299), this method throws a <see cref="ServiceException"/>,
    /// with the <see cref="HttpStatusCode"/> status code and the response content.
    /// </summary>
    /// <typeparam name="T">Type of the response object.</typeparam>
    /// <param name="response">The <see cref="HttpResponseMessage"/> object.</param>
    /// <param name="cancellationToken">The cancellation token. Default is <see cref="CancellationToken.None"/>.</param>
    /// <returns>The operation response as an object of type <typeparamref name="T"/>.</returns>
    protected virtual async Task<T> ProcessResponseAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken = default)
    {
        // Read the response
        string result = await response.Content.ReadAsStringAsync(cancellationToken);

        // 'IsSuccessStatusCode' is true if 'StatusCode' is in the range 200-299
        if (!response.IsSuccessStatusCode)
        {
            // Throw an exception if the response was not successful. This is to avoid deserialization errors,
            // since it's likely that the response is a string, and not an object of type 'T'.
            throw new ServiceException(response.StatusCode, HttpHeaders.Create(response.Headers), result);
        }

        // If 'T' is string, simply return the result
        if (typeof(T) == typeof(String))
            return (T)Convert.ChangeType(result, typeof(T));

        // If response is empty, instantiate an empty object
        if (string.IsNullOrWhiteSpace(result))
            return Activator.CreateInstance<T>();

        // Deserialize the successful response into an object of type 'T' using the specified JsonSerializerSettings.
        // If JsonSerializerSettings null, the default serialization settings will be used
        return JsonConvert.DeserializeObject<T>(result, JsonDeserializerSettings);
    }

    /// <summary>
    /// Parses the <paramref name="uri"/> and removes any leading slashes.
    /// </summary>
    /// <param name="uri">The uri string.</param>
    /// <returns>The parsed uri string.</returns>
    protected static string ParseUri(string uri)
    {
        // Check if null
        if (uri == null)
            return "";

        // Remove the leading slash
        if (uri.StartsWith("/"))
            uri = uri.Substring(1);

        return uri.Trim();
    }
}
