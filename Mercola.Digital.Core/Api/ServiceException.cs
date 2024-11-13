using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Mercola.Digital.Core.Api;
/// <summary>
/// Represents errors that are returned by an HTTP call (status codes 4xx or 5xx).
/// </summary>
[Serializable]
public class ServiceException : Exception
{
    /// <summary>
    /// Initializes a new instance of the ServiceException class with a specified status code and error message.
    /// </summary>
    /// <param name="statusCode">The status code of the HTTP response.</param>
    /// <param name="responseHeaders">The HTTP response headers.</param>
    /// <param name="responseMessage">The message of the HTTP response.</param>
    public ServiceException(HttpStatusCode statusCode, HttpHeaders responseHeaders, string responseMessage)
    {
        StatusCode = statusCode;
        ResponseHeaders = responseHeaders;
        ResponseMessage = responseMessage;
    }

    /// <summary>
    /// Initializes a new instance of the ServiceException class from an exception.
    /// </summary>
    /// <param name="exception">The exception that was thrown.</param>
    public ServiceException(Exception exception)
    {
        StatusCode = HttpStatusCode.InternalServerError;
        ResponseMessage = exception.Message;
    }

    /// <summary>
    /// Serialization constructor for the <see cref="ServiceException"/> class.
    /// </summary>
    /// <param name="serializationInfo">The serialization info.</param>
    /// <param name="streamingContext">The streaming context.</param>
    protected ServiceException(SerializationInfo serializationInfo, StreamingContext streamingContext)
        : base(serializationInfo, streamingContext)
    {
        StatusCode = (HttpStatusCode)serializationInfo.GetValue(nameof(StatusCode), typeof(HttpStatusCode))!;
        ResponseHeaders = (HttpHeaders)serializationInfo.GetValue(nameof(ResponseHeaders), typeof(HttpHeaders));
        ResponseMessage = serializationInfo.GetString(nameof(ResponseMessage));
    }

    /// <inheritdoc cref="ISerializable.GetObjectData" />
    public override void GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
    {
        base.GetObjectData(serializationInfo, streamingContext);

        serializationInfo.AddValue(nameof(StatusCode), StatusCode, typeof(HttpStatusCode));
        serializationInfo.AddValue(nameof(ResponseHeaders), ResponseHeaders, typeof(HttpHeaders));
        serializationInfo.AddValue(nameof(ResponseMessage), ResponseMessage, typeof(string));
    }

    /// <summary>
    /// Gets the status code of the HTTP response.
    /// </summary>
    public HttpStatusCode StatusCode { get; }

    /// <summary>
    /// Gets the collection of HTTP response headers.
    /// </summary>
    public HttpHeaders ResponseHeaders { get; }

    /// <summary>
    /// Gets the message of the HTTP response.
    /// </summary>
    public string ResponseMessage { get; }
}
