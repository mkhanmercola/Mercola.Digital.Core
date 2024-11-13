using Polly;
using Polly.Timeout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercola.Digital.Core.Api;
/// <summary>
/// Basic Polly policies that can be used within <see cref="ApiRetryAdapterBase"/>.
/// </summary>
public static class RetryPolicies
{
    private const int DefaultTimeoutSeconds = 30;
    private const int DefaultRetryCount = 1;
    private static readonly Func<HttpResponseMessage, bool> DefaultRetryResult = r => (int)r.StatusCode >= 500;

    /// <summary>
    /// Creates a Polly Timout Policy.
    /// </summary>
    /// <param name="timeoutSeconds">The timeout period in seconds</param>
    /// <returns>A Polly Timeout policy</returns>
    public static IAsyncPolicy<HttpResponseMessage> TimeoutPolicy(int timeoutSeconds = DefaultTimeoutSeconds) =>
        Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(timeoutSeconds), TimeoutStrategy.Pessimistic);

    /// <summary>
    /// Creates a Polly wait and retry policy on any <see cref="ServiceException"/>'s.
    /// </summary>
    /// /// <param name="retryResult"></param>
    /// <param name="retryCount">The number of retries to attempt</param>
    /// <returns>A Polly WaitAndRetry policy</returns>
    public static IAsyncPolicy<HttpResponseMessage> WaitAndRetryPolicy(int retryCount = DefaultRetryCount,
        Func<HttpResponseMessage, bool> retryResult = null) => Policy.Handle<HttpRequestException>()
            .OrResult(retryResult ?? DefaultRetryResult)
            .Or<TimeoutRejectedException>()
            .Or<IOException>()
            .WaitAndRetryAsync(retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

    /// <summary>
    /// Creates the default <see cref="ApiRetryAdapterBase"/> policy if none is set or overriden.
    /// </summary>
    /// <param name="timeoutSeconds">The timeout period in seconds</param>
    /// <param name="retryCount">The number of retries to attempt</param>
    /// <param name="retryResult"></param>
    /// <returns></returns>
    public static IAsyncPolicy<HttpResponseMessage> DefaultRetryPolicy(int timeoutSeconds = DefaultTimeoutSeconds,
        int retryCount = DefaultRetryCount, Func<HttpResponseMessage, bool> retryResult = null) =>
        WaitAndRetryPolicy(retryCount, retryResult).WrapAsync(TimeoutPolicy(timeoutSeconds));
}
