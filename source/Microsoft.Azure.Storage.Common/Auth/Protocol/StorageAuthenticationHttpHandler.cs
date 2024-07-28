// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Auth.Protocol.StorageAuthenticationHttpHandler
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using Microsoft.Azure.Storage.Core;
using Microsoft.Azure.Storage.Core.Auth;
using Microsoft.Azure.Storage.Core.Util;
using System;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Storage.Auth.Protocol
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
      StorageCredentials credentials1 = request.Credentials;
      Func<StorageRequestMessage, CancellationToken, Task<HttpResponseMessage>> func;
      if ((credentials1 != null ? (credentials1.IsSharedKey ? 1 : 0) : 0) != 0)
      {
        func = new Func<StorageRequestMessage, CancellationToken, Task<HttpResponseMessage>>(this.GetSharedKeyAuthenticationTask);
      }
      else
      {
        StorageCredentials credentials2 = request.Credentials;
        func = (credentials2 != null ? (credentials2.IsToken ? 1 : 0) : 0) == 0 ? new Func<StorageRequestMessage, CancellationToken, Task<HttpResponseMessage>>(this.GetNoOpAuthenticationTask) : new Func<StorageRequestMessage, CancellationToken, Task<HttpResponseMessage>>(this.GetTokenAuthenticationTask);
      }
      return func;
    }

    private Task<HttpResponseMessage> GetSharedKeyAuthenticationTask(
      StorageRequestMessage request,
      CancellationToken cancellationToken)
    {
      StorageAuthenticationHttpHandler.AddDateHeader(request);
      StorageAuthenticationHttpHandler.AddSharedKeyAuth(request);
      return base.SendAsync((HttpRequestMessage) request, cancellationToken);
    }

    internal static void AddDateHeader(StorageRequestMessage request)
    {
      if (request.Headers.Contains("x-ms-date"))
        return;
      string httpString = HttpWebUtility.ConvertDateTimeToHttpString(DateTimeOffset.UtcNow);
      request.Headers.Add("x-ms-date", httpString);
    }

    internal static void AddSharedKeyAuth(StorageRequestMessage request)
    {
      string accountName = request.AccountName;
      StorageCredentials credentials = request.Credentials;
      ICanonicalizer canonicalizer = request.Canonicalizer;
      if (!credentials.IsSharedKey)
        return;
      StorageAccountKey key = credentials.Key;
      if (!string.IsNullOrEmpty(key.KeyName))
        request.Headers.Add("x-ms-key-name", key.KeyName);
      string message = canonicalizer.CanonicalizeHttpRequest((HttpRequestMessage) request, accountName);
      string hmac256 = CryptoUtility.ComputeHmac256(key.KeyValue, message);
      request.Headers.Authorization = new AuthenticationHeaderValue(canonicalizer.AuthorizationScheme, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1}", (object) credentials.AccountName, (object) hmac256));
    }

    private Task<HttpResponseMessage> GetTokenAuthenticationTask(
      StorageRequestMessage request,
      CancellationToken cancellationToken)
    {
      StorageAuthenticationHttpHandler.AddTokenAuth(request);
      return base.SendAsync((HttpRequestMessage) request, cancellationToken);
    }

    internal static void AddTokenAuth(StorageRequestMessage request)
    {
      StorageCredentials credentials = request.Credentials;
      if (!request.Headers.Contains("x-ms-date"))
      {
        string httpString = HttpWebUtility.ConvertDateTimeToHttpString(DateTimeOffset.UtcNow);
        request.Headers.Add("x-ms-date", httpString);
      }
      if (!"https".Equals(request.RequestUri.Scheme, StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException("Only HTTPS is allowed for token credential.");
      if (!credentials.IsToken)
        return;
      request.Headers.Add("Authorization", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Bearer {0}", (object) credentials.TokenCredential.Token));
    }
  }
}
