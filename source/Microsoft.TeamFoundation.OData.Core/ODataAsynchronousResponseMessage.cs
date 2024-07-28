// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataAsynchronousResponseMessage
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.OData
{
  public sealed class ODataAsynchronousResponseMessage : 
    IContainerProvider,
    IODataResponseMessageAsync,
    IODataResponseMessage
  {
    private readonly bool writing;
    private readonly Stream stream;
    private readonly Action<ODataAsynchronousResponseMessage> writeEnvelope;
    private readonly IServiceProvider container;
    private bool envelopeWritten;
    private int statusCode;
    private IDictionary<string, string> headers;

    private ODataAsynchronousResponseMessage(
      Stream stream,
      int statusCode,
      IDictionary<string, string> headers,
      Action<ODataAsynchronousResponseMessage> writeEnvelope,
      bool writing,
      IServiceProvider container)
    {
      this.stream = stream;
      this.statusCode = statusCode;
      this.headers = headers;
      this.writeEnvelope = writeEnvelope;
      this.writing = writing;
      this.container = container;
    }

    public int StatusCode
    {
      get => this.statusCode;
      set
      {
        this.VerifyCanSetHeaderAndStatusCode();
        this.statusCode = value;
      }
    }

    public IEnumerable<KeyValuePair<string, string>> Headers => (IEnumerable<KeyValuePair<string, string>>) this.headers;

    public IServiceProvider Container => this.container;

    public string GetHeader(string headerName)
    {
      string str;
      return this.headers != null && this.headers.TryGetValue(headerName, out str) ? str : (string) null;
    }

    public void SetHeader(string headerName, string headerValue)
    {
      this.VerifyCanSetHeaderAndStatusCode();
      if (headerValue == null)
      {
        if (this.headers == null)
          return;
        this.headers.Remove(headerName);
      }
      else
      {
        if (this.headers == null)
          this.headers = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.Ordinal);
        this.headers[headerName] = headerValue;
      }
    }

    public Stream GetStream()
    {
      if (this.writing && !this.envelopeWritten)
      {
        if (this.writeEnvelope != null)
          this.writeEnvelope(this);
        this.envelopeWritten = true;
      }
      return this.stream;
    }

    public Task<Stream> GetStreamAsync() => Task<Stream>.Factory.StartNew(new Func<Stream>(this.GetStream));

    internal static ODataAsynchronousResponseMessage CreateMessageForWriting(
      Stream outputStream,
      Action<ODataAsynchronousResponseMessage> writeEnvelope,
      IServiceProvider container)
    {
      return new ODataAsynchronousResponseMessage(outputStream, 0, (IDictionary<string, string>) null, writeEnvelope, true, container);
    }

    internal static ODataAsynchronousResponseMessage CreateMessageForReading(
      Stream stream,
      int statusCode,
      IDictionary<string, string> headers,
      IServiceProvider container)
    {
      return new ODataAsynchronousResponseMessage(stream, statusCode, headers, (Action<ODataAsynchronousResponseMessage>) null, false, container);
    }

    private void VerifyCanSetHeaderAndStatusCode()
    {
      if (!this.writing)
        throw new ODataException(Strings.ODataAsyncResponseMessage_MustNotModifyMessage);
    }
  }
}
