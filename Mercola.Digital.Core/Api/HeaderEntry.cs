using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercola.Digital.Core.Api;
public class HeaderEntry
{
    /// <summary>
    /// Header key.
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// Header value.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="key">The header key.</param>
    /// <param name="value">The header value.</param>
    public HeaderEntry(string key, string value)
    {
        Key = key;
        Value = value;
    }
}
