using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercola.Digital.Core.Api;
public abstract class ApiAdapterBase : ApiAdapterCore
{
    /// <summary>
    /// Gets the timeout value (in seconds) for the API calls. Default to 30 seconds.
    /// </summary>
    protected virtual int TimeoutSeconds => 30;

    /// <summary>
    /// Instantiates a new object of the <see cref="ApiAdapterBase"/> class.
    /// </summary>
    /// <param name="clientFactory">Injected <see cref="IHttpClientFactory"/> implementation to instantiate and pool <see cref="HttpClient"/> objects.</param>
    protected ApiAdapterBase(IHttpClientFactory clientFactory) : base(clientFactory)
    {
    }

    /// <inheritdoc />
    protected override HttpClient GetHttpClient()
    {
        var client = base.GetHttpClient();
        client.Timeout = new TimeSpan(0, 0, TimeoutSeconds);
        return client;
    }
}
