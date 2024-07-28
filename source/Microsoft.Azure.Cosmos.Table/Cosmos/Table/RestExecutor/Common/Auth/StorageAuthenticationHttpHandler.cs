// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.RestExecutor.Common.Auth.StorageAuthenticationHttpHandler
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Table.RestExecutor.Common.Auth
{
  internal sealed class StorageAuthenticationHttpHandler : HttpClientHandler
  {
    private static Lazy<StorageAuthenticationHttpHandler> instance = new Lazy<StorageAuthenticationHttpHandler>((Func<StorageAuthenticationHttpHandler>) (() => new StorageAuthenticationHttpHandler()));

    internal StorageAuthenticationHttpHandler()
    {
    }

    public static StorageAuthenticationHttpHandler Instance => StorageAuthenticationHttpHandler.instance.Value;

    private Task<HttpResponseMessage> GetNoOpAuthenticationTask(
      StorageRequestMessage request,
      CancellationToken cancellationToken)
    {
      return base.SendAsync((HttpRequestMessage) request, cancellationToken);
    }

    protected override Task<HttpResponseMessage> SendAsync(
      HttpRequestMessage request,
      CancellationToken cancellationToken)
    {
      StorageRequestMessage request1 = request as StorageRequestMessage;
      return this.SelectAuthenticationTaskFactory(request1)(request1, cancellationToken);
    }

    private Func<StorageRequestMessage, CancellationToken, Task<HttpResponseMessage>> SelectAuthenticationTaskFactory(
      StorageRequestMessage request)
    {
      StorageCredentials credentials = request.Credentials;
      return (credentials != null ? (credentials.IsSharedKey ? 1 : 0) : 0) == 0 ? new Func<StorageRequestMessage, CancellationToken, Task<HttpResponseMessage>>(this.GetNoOpAuthenticationTask) : new Func<StorageRequestMessage, CancellationToken, Task<HttpResponseMessage>>(this.GetSharedKeyAuthenticationTask);
    }

    private Task<HttpResponseMessage> GetSharedKeyAuthenticationTask(
      StorageRequestMessage request,
      CancellationToken cancellationToken)
    {
      StorageRequestMessage storageRequestMessage = request;
      ICanonicalizer canonicalizer = storageRequestMessage.Canonicalizer;
      StorageCredentials credentials = storageRequestMessage.Credentials;
      string accountName = storageRequestMessage.AccountName;
      if (!request.Headers.Contains("x-ms-date"))
      {
        string httpString = HttpWebUtility.ConvertDateTimeToHttpString(DateTimeOffset.UtcNow);
        request.Headers.Add("x-ms-date", httpString);
      }
      if (credentials.IsSharedKey)
      {
        string hmac256 = CryptoUtility.ComputeHmac256(credentials.Key, canonicalizer.CanonicalizeHttpRequest((HttpRequestMessage) request, accountName));
        request.Headers.Authorization = new AuthenticationHeaderValue(canonicalizer.AuthorizationScheme, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1}", (object) credentials.AccountName, (object) hmac256));
      }
      return base.SendAsync((HttpRequestMessage) request, cancellationToken);
    }
  }
}
