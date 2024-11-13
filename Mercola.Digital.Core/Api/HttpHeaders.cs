using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Mercola.Digital.Core.Api;
/// <summary>
/// Represents a collection of HTTP request and response headers.
/// </summary>
public class HttpHeaders : IDictionary<string, List<string>>
{
    private readonly Dictionary<string, List<string>> _store;

    #region Interface Properties Implementation

    /// <summary>
    /// Gets an <see cref="ICollection"/> containing the keys of the headers.
    /// </summary>
    public ICollection<string> Keys { get { return _store.Keys; } }

    /// <summary>
    /// Gets an <see cref="ICollection"/> containing the values of the headers.
    /// </summary>
    public ICollection<List<string>> Values { get { return _store.Values; } }

    /// <summary>
    /// Gets the number of headers in the collection.
    /// </summary>
    public int Count { get { return _store.Count; } }

    /// <summary>
    /// Gets a value indicating whether the collection is read-only.
    /// </summary>
    public bool IsReadOnly { get { return ((ICollection<KeyValuePair<string, List<string>>>)_store).IsReadOnly; } }

    /// <summary>
    /// Gets or sets the header with the specified key.
    /// </summary>
    /// <param name="key">The key of the header to get or set.</param>
    /// <returns>The header with the specified key, or an empty list if the key is not present.</returns>
    public List<string> this[string key]
    {
        get
        {
            if (!ContainsKey(key))
            {
                return new List<string>();
            }
            return _store[key];
        }
        set
        {
            _store[key] = value;
        }
    }

    #endregion Interface Properties Implementation

    /// <summary>
    /// Instantiates a new object of the <see cref="HttpHeaders"/> class.
    /// </summary>
    public HttpHeaders()
    {
        _store = new Dictionary<string, List<string>>();
    }

    /// <summary>
    /// Adds the specified header and its values into the collection.
    /// </summary>
    /// <param name="key">The header to add to the collection.</param>
    /// <param name="value">A single header value to add to the collection.</param>
    public void Add(string key, string value)
    {
        Add(key, new List<string>() { value });
    }

    #region Interface Methods Implementation

    /// <summary>
    /// Adds the specified header and its values into the collection.
    /// </summary>
    /// <param name="key">The header to add to the collection.</param>
    /// <param name="value">A single header or a list of header values, represented as <see cref="List{T}"/>, to add to the collection.</param>
    public void Add(string key, List<string> value)
    {
        _store.Add(key, value);
    }

    /// <summary>
    /// Determines whether the collection contains an element with the specified key.
    /// </summary>
    /// <param name="key">The key to locate in the collection.</param>
    /// <returns>true if the System.Collections.Generic.IDictionary`2 contains an element with the key; otherwise, false.</returns>
    public bool ContainsKey(string key)
    {
        return _store.ContainsKey(key);
    }

    /// <summary>
    /// Removes the first occurrence of a specific object from the collection.
    /// </summary>
    /// <param name="key">The object to remove from the collection.</param>
    /// <returns>true if item was successfully removed from the collection; otherwise, false. This method also returns false if item is not found in the original collection.</returns>
    public bool Remove(string key)
    {
        return _store.Remove(key);
    }

    /// <summary>
    /// Gets the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key whose value to get.</param>
    /// <param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the value parameter.
    /// This parameter is passed uninitialized.</param>
    /// <returns>true if the object that implements collection contains an element with the specified key; otherwise, false.</returns>
    public bool TryGetValue(string key, [MaybeNullWhen(false)] out List<string> value)
    {
        return _store.TryGetValue(key, out value);
    }

    /// <summary>
    /// Adds the specified header and its values into the collection.
    /// </summary>
    /// <param name="item">The key and values of the header to add to the collection.</param>
    public void Add(KeyValuePair<string, List<string>> item)
    {
        Add(item.Key, item.Value);
    }

    /// <summary>
    /// Removes all items from the collection.
    /// </summary>
    public void Clear()
    {
        _store.Clear();
    }

    /// <summary>
    /// Determines whether the collection contains a specific value.
    /// </summary>
    /// <param name="item">The object to locate in the collection.</param>
    /// <returns>true if item is found in the collection; otherwise, false.</returns>
    public bool Contains(KeyValuePair<string, List<string>> item)
    {
        return ContainsKey(item.Key);
    }

    /// <summary>
    /// Copies the headers of the collection to an <see cref="Array"/>, starting at a particular <see cref="Array"/> index.
    /// </summary>
    /// <param name="array">The one-dimensional <see cref="Array"/> that is the destination of the headers copied from collection. The <see cref="Array"/> must have zero-based indexing.</param>
    /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
    public void CopyTo(KeyValuePair<string, List<string>>[] array, int arrayIndex)
    {
        ((ICollection<KeyValuePair<string, List<string>>>)_store).CopyTo(array, arrayIndex);
    }

    /// <summary>
    /// Removes the header with the specified key from the collection.
    /// </summary>
    /// <param name="item">The item to remove.</param>
    /// <returns>true if the header is successfully removed; otherwise, false. This method also returns false if key was not found in the original collection.</returns>
    public bool Remove(KeyValuePair<string, List<string>> item)
    {
        return Remove(item.Key);
    }

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the collection.</returns>
    public IEnumerator<KeyValuePair<string, List<string>>> GetEnumerator()
    {
        return ((IEnumerable<KeyValuePair<string, List<string>>>)_store).GetEnumerator();
    }

    /// <summary>
    /// Returns an enumerator that iterates through a collection.
    /// </summary>
    /// <returns>An <see cref="IEnumerator"/> object that can be used to iterate through the collection.</returns>
    [ExcludeFromCodeCoverage]
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_store).GetEnumerator();
    }

    #endregion Interface Methods Implementation

    /// <summary>
    /// Instantiates a new object of the <see cref="HttpHeaders"/> class and copies the headers from <paramref name="httpResponseHeaders"/>.
    /// </summary>
    /// <param name="httpResponseHeaders"><see cref="HttpResponseHeaders"/> object containing the headers.</param>
    /// <returns>An object of the <see cref="HttpHeaders"/> class.</returns>
    internal static HttpHeaders Create(HttpResponseHeaders httpResponseHeaders)
    {
        HttpHeaders responseHeaders = new HttpHeaders();

        foreach (KeyValuePair<string, IEnumerable<string>> header in httpResponseHeaders)
            responseHeaders.Add(header.Key, header.Value.ToList());

        return responseHeaders;
    }

}
