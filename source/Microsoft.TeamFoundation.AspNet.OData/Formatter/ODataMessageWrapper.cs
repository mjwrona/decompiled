// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Formatter.ODataMessageWrapper
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.OData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Microsoft.AspNet.OData.Formatter
{
  internal class ODataMessageWrapper : 
    IODataRequestMessageAsync,
    IODataRequestMessage,
    IODataResponseMessageAsync,
    IODataResponseMessage,
    IODataPayloadUriConverter,
    IContainerProvider,
    IDisposable
  {
    private Stream _stream;
    private Dictionary<string, string> _headers;
    private IDictionary<string, string> _contentIdMapping;
    private static readonly Regex ContentIdReferencePattern = new Regex("\\$\\d", RegexOptions.Compiled);

    public ODataMessageWrapper()
      : this((Stream) null, (Dictionary<string, string>) null)
    {
    }

    public ODataMessageWrapper(Stream stream)
      : this(stream, (Dictionary<string, string>) null)
    {
    }

    public ODataMessageWrapper(Stream stream, Dictionary<string, string> headers)
      : this(stream, headers, (IDictionary<string, string>) null)
    {
    }

    public ODataMessageWrapper(
      Stream stream,
      Dictionary<string, string> headers,
      IDictionary<string, string> contentIdMapping)
    {
      this._stream = stream;
      this._headers = headers == null ? new Dictionary<string, string>() : headers;
      this._contentIdMapping = contentIdMapping ?? (IDictionary<string, string>) new Dictionary<string, string>();
    }

    public IEnumerable<KeyValuePair<string, string>> Headers => (IEnumerable<KeyValuePair<string, string>>) this._headers;

    public string Method
    {
      get => throw new NotImplementedException();
      set => throw new NotImplementedException();
    }

    public Uri Url
    {
      get => throw new NotImplementedException();
      set => throw new NotImplementedException();
    }

    public int StatusCode
    {
      get => throw new NotImplementedException();
      set => throw new NotImplementedException();
    }

    public IServiceProvider Container { get; set; }

    public string GetHeader(string headerName)
    {
      string str;
      return this._headers.TryGetValue(headerName, out str) ? str : (string) null;
    }

    public Stream GetStream() => this._stream;

    public Task<Stream> GetStreamAsync()
    {
      TaskCompletionSource<Stream> completionSource = new TaskCompletionSource<Stream>();
      completionSource.SetResult(this._stream);
      return completionSource.Task;
    }

    public void SetHeader(string headerName, string headerValue) => this._headers[headerName] = headerValue;

    public Uri ConvertPayloadUri(Uri baseUri, Uri payloadUri)
    {
      string str = !(payloadUri == (Uri) null) ? payloadUri.OriginalString : throw new ArgumentNullException(nameof (payloadUri));
      return ODataMessageWrapper.ContentIdReferencePattern.IsMatch(str) ? new Uri(ContentIdHelpers.ResolveContentId(str, this._contentIdMapping), UriKind.RelativeOrAbsolute) : (Uri) null;
    }

    public void Dispose() => this.Dispose(true);

    protected void Dispose(bool disposing)
    {
      if (!disposing || this._stream == null)
        return;
      this._stream.Dispose();
    }
  }
}
