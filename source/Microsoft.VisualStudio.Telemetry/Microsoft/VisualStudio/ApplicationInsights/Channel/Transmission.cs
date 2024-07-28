// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.Channel.Transmission
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.ApplicationInsights.Channel
{
  public class Transmission
  {
    internal const string ContentTypeHeader = "Content-Type";
    internal const string ContentEncodingHeader = "Content-Encoding";
    internal const string EndPointAPIKeyPropertyName = "EndPointAPIKey";
    private static readonly string Key = "9418E9E3-1969-413A-8617-A85739D315A1";
    private static readonly HashAlgorithm Encrypter = (HashAlgorithm) new HMACSHA256(Encoding.UTF8.GetBytes(Transmission.Key));
    private static readonly object hashLock = new object();
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(100.0);
    private int isSending;
    private string contentHash;
    internal string ApiKey = string.Empty;

    public Transmission(
      Uri address,
      byte[] content,
      string contentType,
      string contentEncoding,
      TimeSpan timeout = default (TimeSpan))
    {
      if (address == (Uri) null)
        throw new ArgumentNullException(nameof (address));
      if (content == null)
        throw new ArgumentNullException(nameof (content));
      if (contentType == null)
        throw new ArgumentNullException(nameof (contentType));
      this.EndpointAddress = address;
      this.Content = content;
      this.ContentType = contentType;
      this.ContentEncoding = contentEncoding;
      this.Timeout = timeout == new TimeSpan() ? Transmission.DefaultTimeout : timeout;
    }

    public Transmission(
      Uri address,
      byte[] content,
      string contentType,
      string contentEncoding,
      string apiKey,
      TimeSpan timeout = default (TimeSpan))
      : this(address, content, contentType, contentEncoding, timeout)
    {
      this.ApiKey = apiKey;
    }

    protected internal Transmission()
    {
    }

    public Uri EndpointAddress { get; private set; }

    public byte[] Content { get; private set; }

    public string ContentHash
    {
      get
      {
        if (this.contentHash == null)
          this.contentHash = Transmission.HashContent(this.Content);
        return this.contentHash;
      }
    }

    public string ContentType { get; private set; }

    public string ContentEncoding { get; private set; }

    public TimeSpan Timeout { get; private set; }

    public virtual async Task SendAsync(CancellationToken token = default (CancellationToken))
    {
      if (Interlocked.CompareExchange(ref this.isSending, 1, 0) != 0)
        throw new InvalidOperationException("SendAsync is already in progress.");
      try
      {
        WebRequest request = this.CreateRequest(this.EndpointAddress);
        Task timeoutTask = Task.Delay(this.Timeout);
        Task sendTask = this.SendRequestAsync(request);
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
        token.Register((Action) (() => tcs.TrySetCanceled()), false);
        Task task1 = await Task.WhenAny(timeoutTask, sendTask, (Task) tcs.Task).ConfigureAwait(false);
        token.ThrowIfCancellationRequested();
        Task task2 = timeoutTask;
        if (task1 == task2)
          request.Abort();
        await sendTask.ConfigureAwait(false);
        request = (WebRequest) null;
        timeoutTask = (Task) null;
        sendTask = (Task) null;
      }
      finally
      {
        Interlocked.Exchange(ref this.isSending, 0);
      }
    }

    private string GetNoResponseBodyFlag()
    {
      bool result;
      return !bool.TryParse(Environment.GetEnvironmentVariable("VSTelemetryAPI_NoResponseBody"), out result) ? "false" : result.ToString().ToLowerInvariant();
    }

    public override string ToString() => "ContentHash: " + this.ContentHash;

    protected virtual WebRequest CreateRequest(Uri address)
    {
      WebRequest request = WebRequest.Create(address);
      request.Method = "POST";
      if (!string.IsNullOrEmpty(this.ContentType))
        request.ContentType = this.ContentType;
      if (!string.IsNullOrEmpty(this.ContentEncoding))
        request.Headers["Content-Encoding"] = this.ContentEncoding;
      if (this.EndpointAddress.OriginalString.Equals("https://events.data.microsoft.com/OneCollector/1.0", StringComparison.OrdinalIgnoreCase) || this.EndpointAddress.OriginalString.Equals("https://mobile.events.data.microsoft.com/OneCollector/1.0", StringComparison.OrdinalIgnoreCase))
      {
        request.Headers["x-apikey"] = this.ApiKey;
        request.Headers["sdk-version"] = "VSTelemetryAPI";
        request.Headers["NoResponseBody"] = this.GetNoResponseBodyFlag();
      }
      request.ContentLength = (long) this.Content.Length;
      return request;
    }

    private async Task SendRequestAsync(WebRequest request)
    {
      using (Stream requestStream = await request.GetRequestStreamAsync().ConfigureAwait(false))
        await requestStream.WriteAsync(this.Content, 0, this.Content.Length).ConfigureAwait(false);
      using (await request.GetResponseAsync().ConfigureAwait(false))
        ;
    }

    private static string HashContent(byte[] content)
    {
      lock (Transmission.hashLock)
        return BitConverter.ToString(Transmission.Encrypter.ComputeHash(content)).Replace("-", string.Empty);
    }
  }
}
