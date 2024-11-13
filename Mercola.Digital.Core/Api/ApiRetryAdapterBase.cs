using Polly;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercola.Digital.Core.Api;
public abstract class ApiRetryAdapterBase : ApiAdapterCore
{
    /// <summary>
    /// Instantiates a new object of the <see cref="ApiRetryAdapterBase"/> class.
    /// </summary>
    /// <param name="clientFactory">Injected <see cref="IHttpClientFactory"/> implementation to instantiate and pool <see cref="HttpClient"/> objects.</param>
    protected ApiRetryAdapterBase(IHttpClientFactory clientFactory) : base(clientFactory)
    {
    }

    /// <summary>
    /// Get the <see cref="IAsyncPolicy"/> set when executing the client request
    /// </summary>
    protected virtual IAsyncPolicy<HttpResponseMessage> Policy { get; }

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    protected override Task<T> GetAsync<T>(string path, CancellationToken cancellationToken = default, params HeaderEntry[] headers)
    {
        return SendAsync<T>(HttpMethod.Get, path, null, cancellationToken, headers);
    }

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    protected override Task<T> PostAsync<T>(string path, object content, CancellationToken cancellationToken = default, params HeaderEntry[] headers)
    {
        return SendAsync<T>(HttpMethod.Post, path, content, cancellationToken, headers);
    }

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    protected override Task<T> PutAsync<T>(string path, object content, CancellationToken cancellationToken = default, params HeaderEntry[] headers)
    {
        return SendAsync<T>(HttpMethod.Put, path, content, cancellationToken, headers);
    }

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    protected override Task<T> PatchAsync<T>(string path, object content, CancellationToken cancellationToken = default, params HeaderEntry[] headers)
    {
        return SendAsync<T>(HttpMethod.Patch, path, content, cancellationToken, headers);
    }

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    protected override Task<T> DeleteAsync<T>(string path, CancellationToken cancellationToken = default, params HeaderEntry[] headers)
    {
        return SendAsync<T>(HttpMethod.Delete, path, null, cancellationToken, headers);
    }

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    protected override Task<HttpResponseMessage> GetAsync(string path, CancellationToken cancellationToken = default, params HeaderEntry[] headers)
    {
        return SendAsync(HttpMethod.Get, path, null, cancellationToken, headers);
    }

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    protected override Task<HttpResponseMessage> PostAsync(string path, object content, CancellationToken cancellationToken = default, params HeaderEntry[] headers)
    {
        return SendAsync(HttpMethod.Post, path, content, cancellationToken, headers);
    }

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    protected override Task<HttpResponseMessage> PutAsync(string path, object content, CancellationToken cancellationToken = default, params HeaderEntry[] headers)
    {
        return SendAsync(HttpMethod.Put, path, content, cancellationToken, headers);
    }

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    protected override Task<HttpResponseMessage> PatchAsync(string path, object content, CancellationToken cancellationToken = default, params HeaderEntry[] headers)
    {
        return SendAsync(HttpMethod.Patch, path, content, cancellationToken, headers);
    }

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    protected override Task<HttpResponseMessage> DeleteAsync(string path, CancellationToken cancellationToken = default, params HeaderEntry[] headers)
    {
        return SendAsync(HttpMethod.Delete, path, null, cancellationToken, headers);
    }

    /// <summary>
    /// Executes the defined <see cref="IAsyncPolicy"/> and sends an HTTP request as an asynchronous operation,
    /// and sends an HTTP request as an asynchronous operation, with a response object of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">Type of the response object.</typeparam>
    /// <param name="method"><see cref="HttpMethod"/> object representing the HTTP method.</param>
    /// <param name="path">(Optional) The path of the request after <seealso cref="ApiAdapterCore.BaseAddress"/>.</param>
    /// <param name="content">The HTTP request content sent to the server.</param>
    /// <param name="cancellationToken">The cancellation token. Default is <see cref="CancellationToken.None"/>.</param>
    /// <param name="headers"><see cref="HeaderEntry"/> objects representing the request headers (optional).</param>
    /// <returns>The operation response as an object of type <typeparamref name="T"/>.</returns>
    /// <remarks>Type {T} cannot be of type <see cref="HttpResponseMessage"/>.</remarks>
    protected override async Task<T> SendAsync<T>(HttpMethod method, string path, object content, CancellationToken cancellationToken = default, params HeaderEntry[] headers)
    {
        if (typeof(T) == typeof(HttpResponseMessage))
            throw new ArgumentException($"Invalid type '{typeof(HttpResponseMessage)}'. Please use the non-generic method 'Task<HttpResponseMessage> SendAsync()' instead.");

        using HttpResponseMessage response = await SendAsync(method, path, content, cancellationToken, headers);

        return await ProcessResponseAsync<T>(response, cancellationToken);
    }

    /// <summary>
    /// Executes the defined <see cref="IAsyncPolicy"/> and sends an HTTP request as an asynchronous operation,
    /// which returns a generic <see cref="HttpResponseMessage"/> response object.
    /// </summary>
    /// <param name="method"><see cref="HttpMethod"/> object representing the HTTP method.</param>
    /// <param name="path">(Optional) The path of the request after <seealso cref="ApiAdapterCore.BaseAddress"/>.</param>
    /// <param name="content">The HTTP request content sent to the server.</param>
    /// <param name="cancellationToken">The cancellation token. Default is <see cref="CancellationToken.None"/>.</param>
    /// <param name="headers"><see cref="HeaderEntry"/> objects representing the request headers (optional).</param>
    /// <returns>The operation response as an <see cref="HttpResponseMessage"/> object.</returns>
    protected override async Task<HttpResponseMessage> SendAsync(HttpMethod method, string path, object content, CancellationToken cancellationToken = default, params HeaderEntry[] headers)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using HttpClient client = GetHttpClient();

        var policy = Policy ?? RetryPolicies.DefaultRetryPolicy();

        try
        {
            return await policy.ExecuteAsync(async policyCancellationToken =>
            {
                string requestUri = ParseUri(path);
                using HttpRequestMessage request = new HttpRequestMessage(method, requestUri);

                // Add headers from parameters
                foreach (HeaderEntry header in headers)
                    request.Headers.Add(header.Key, header.Value);

                if (content != null)
                    request.Content = CreateStringContent(content);

                return await client.SendAsync(request, policyCancellationToken);
            }, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception e)
        {
            throw new ServiceException(e);
        }
    }
}
