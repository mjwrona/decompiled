// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ResponseMessage
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Diagnostics;
using Microsoft.Azure.Cosmos.Query.Core.Metrics;
using Microsoft.Azure.Cosmos.Resource.CosmosExceptions;
using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Documents;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace Microsoft.Azure.Cosmos
{
  public class ResponseMessage : IDisposable
  {
    private CosmosDiagnostics diagnostics;
    private bool disposed;
    private Stream content;

    public ResponseMessage()
    {
      this.Headers = new Headers();
      this.CosmosException = (CosmosException) null;
      this.Trace = (ITrace) NoOpTrace.Singleton;
    }

    public ResponseMessage(
      HttpStatusCode statusCode,
      RequestMessage requestMessage = null,
      string errorMessage = null)
    {
      this.StatusCode = statusCode >= (HttpStatusCode) 0 && statusCode <= (HttpStatusCode) 999 ? statusCode : throw new ArgumentOutOfRangeException(nameof (statusCode));
      this.RequestMessage = requestMessage;
      this.Headers = new Headers();
      this.Trace = requestMessage?.Trace ?? (ITrace) NoOpTrace.Singleton;
      if (string.IsNullOrEmpty(errorMessage))
        return;
      this.CosmosException = CosmosExceptionFactory.Create(statusCode, requestMessage, errorMessage);
    }

    internal ResponseMessage(
      HttpStatusCode statusCode,
      RequestMessage requestMessage,
      Headers headers,
      CosmosException cosmosException,
      ITrace trace)
    {
      this.StatusCode = statusCode;
      this.RequestMessage = requestMessage;
      this.CosmosException = cosmosException;
      this.Headers = headers ?? new Headers();
      this.IndexUtilizationText = ResponseMessage.DecodeIndexMetrics(this.Headers, true);
      if (requestMessage != null && requestMessage.Trace != null)
        this.Trace = requestMessage.Trace;
      else
        this.Trace = trace ?? throw new ArgumentNullException(nameof (trace));
    }

    public virtual HttpStatusCode StatusCode { get; private set; }

    public virtual Stream Content
    {
      get => this.content;
      set
      {
        this.CheckDisposed();
        this.content = value;
      }
    }

    public virtual string ErrorMessage => this.CosmosException?.Message;

    public virtual Headers Headers { get; }

    public virtual string ContinuationToken => this.Headers?.ContinuationToken;

    private Lazy<string> IndexUtilizationText { get; }

    public string IndexMetrics => this.IndexUtilizationText?.Value;

    public virtual RequestMessage RequestMessage { get; internal set; }

    public virtual CosmosDiagnostics Diagnostics
    {
      get => this.diagnostics ?? (CosmosDiagnostics) new CosmosTraceDiagnostics(this.Trace ?? (ITrace) NoOpTrace.Singleton);
      set => this.diagnostics = value ?? throw new ArgumentNullException(nameof (Diagnostics));
    }

    internal ITrace Trace { get; set; }

    internal CosmosException CosmosException { get; }

    public virtual bool IsSuccessStatusCode => this.StatusCode.IsSuccess();

    public virtual ResponseMessage EnsureSuccessStatusCode()
    {
      if (!this.IsSuccessStatusCode)
        throw CosmosExceptionFactory.Create(this);
      return this;
    }

    public void Dispose() => this.Dispose(true);

    internal string GetResourceAddress()
    {
      string resourceIdOrFullName;
      return PathsHelper.TryParsePathSegments(this.RequestMessage?.RequestUriString, out bool _, out string _, out resourceIdOrFullName, out bool _) ? resourceIdOrFullName : (string) null;
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!disposing || this.disposed)
        return;
      this.disposed = true;
      if (this.content != null)
      {
        this.content.Dispose();
        this.content = (Stream) null;
      }
      if (this.RequestMessage != null)
      {
        this.RequestMessage.Dispose();
        this.RequestMessage = (RequestMessage) null;
      }
      if (this.Trace == null)
        return;
      this.Trace.Dispose();
      this.Trace = (ITrace) null;
    }

    private void CheckDisposed()
    {
      if (this.disposed)
        throw new ObjectDisposedException(this.GetType().ToString());
    }

    internal static Lazy<string> DecodeIndexMetrics(
      Headers responseMessageHeaders,
      bool isBse64Encoded)
    {
      return responseMessageHeaders?.IndexUtilizationText != null ? new Lazy<string>((Func<string>) (() =>
      {
        IndexUtilizationInfo fromString = IndexUtilizationInfo.CreateFromString(responseMessageHeaders.IndexUtilizationText, isBse64Encoded);
        StringBuilder stringBuilder = new StringBuilder();
        new IndexMetricWriter(stringBuilder).WriteIndexMetrics(fromString);
        return stringBuilder.ToString();
      })) : (Lazy<string>) null;
    }
  }
}
