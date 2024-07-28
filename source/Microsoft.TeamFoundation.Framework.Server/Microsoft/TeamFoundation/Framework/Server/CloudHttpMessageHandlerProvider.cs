// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.CloudHttpMessageHandlerProvider
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server.OAuth2;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class CloudHttpMessageHandlerProvider : IVssHttpMessageHandlerProvider, IDisposable
  {
    private ConcurrentDictionary<CloudHttpMessageHandlerProvider.HandlerCacheKey, HttpMessageHandler> m_handlers;
    private const string c_netHttpHandler = "HttpClientHandler";
    private int m_settingsVersion = 1;
    private CloudHttpMessageHandlerProvider.HandlerSettings m_handlerSettings;
    private const string c_settingsPath = "/Service/HttpMessageHandlerProvider/Settings/";
    private static readonly RegistryQuery s_settingsQuery = (RegistryQuery) "/Service/HttpMessageHandlerProvider/Settings/...";
    private static readonly RegistryQuery s_sendDataTimeoutPath = (RegistryQuery) "/Service/HttpMessageHandlerProvider/Settings/SendTimeout";
    private static readonly RegistryQuery s_receiveDataTimeoutPath = (RegistryQuery) "/Service/HttpMessageHandlerProvider/Settings/ReceiveDataTimeout";
    private static readonly RegistryQuery s_receiveHeadersTimeoutPath = (RegistryQuery) "/Service/HttpMessageHandlerProvider/Settings/ReceiveHeadersTimeout";
    private static readonly RegistryQuery s_hostedServiceNameQuery = (RegistryQuery) "/Configuration/Settings/HostedServiceName";
    private static readonly TimeSpan s_defaultSendDataTimeout = TimeSpan.FromSeconds(100.0);
    private static readonly TimeSpan s_defaultReceiveDataTimeout = TimeSpan.FromSeconds(100.0);
    private static readonly TimeSpan s_defaultReceiveHeadersTimeout = TimeSpan.FromSeconds(100.0);

    public CloudHttpMessageHandlerProvider() => this.m_handlers = new ConcurrentDictionary<CloudHttpMessageHandlerProvider.HandlerCacheKey, HttpMessageHandler>(CloudHttpMessageHandlerProvider.HandlerCacheKey.Comparer);

    public void Dispose()
    {
      foreach (KeyValuePair<CloudHttpMessageHandlerProvider.HandlerCacheKey, HttpMessageHandler> handler in this.m_handlers)
        handler.Value.Dispose();
      this.m_handlers.Clear();
    }

    public HttpMessageHandler GetHandler(
      IVssRequestContext requestContext,
      Uri baseUri,
      Guid targetServicePrincipal = default (Guid))
    {
      requestContext = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
      Uri uri = new UriBuilder(baseUri)
      {
        Path = "/",
        Query = ((string) null),
        Fragment = ((string) null)
      }.Uri;
      long version = requestContext.GetService<IOAuth2SettingsService>().GetVersion(requestContext);
      IS2SCredentialsService s2sCredentialsService = requestContext.GetService<IS2SCredentialsService>();
      CloudHttpMessageHandlerProvider.HandlerSettings handlerSettings = this.m_handlerSettings;
      return this.m_handlers.GetOrAdd(new CloudHttpMessageHandlerProvider.HandlerCacheKey(uri, targetServicePrincipal, version, "HttpClientHandler", handlerSettings.Version), (Func<CloudHttpMessageHandlerProvider.HandlerCacheKey, HttpMessageHandler>) (key => this.CreateHttpMessageHandler(s2sCredentialsService.GetS2SCredentials(requestContext, targetServicePrincipal), CloudHttpMessageHandlerProvider.GetVssHttpRequestSettings(requestContext))));
    }

    public void Initialize(IVssRequestContext requestContext)
    {
      requestContext.GetService<IVssRegistryService>().RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnHandlerSettingsChanged), false, (IEnumerable<RegistryQuery>) new RegistryQuery[2]
      {
        CloudHttpMessageHandlerProvider.s_settingsQuery,
        CloudHttpMessageHandlerProvider.s_hostedServiceNameQuery
      });
      Interlocked.CompareExchange<CloudHttpMessageHandlerProvider.HandlerSettings>(ref this.m_handlerSettings, new CloudHttpMessageHandlerProvider.HandlerSettings(requestContext, 1), (CloudHttpMessageHandlerProvider.HandlerSettings) null);
    }

    private void OnHandlerSettingsChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      Volatile.Write<CloudHttpMessageHandlerProvider.HandlerSettings>(ref this.m_handlerSettings, new CloudHttpMessageHandlerProvider.HandlerSettings(requestContext, ++this.m_settingsVersion));
    }

    protected virtual HttpMessageHandler CreateHttpMessageHandler(
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      HttpMessageHandler handler = null)
    {
      return handler == null ? (HttpMessageHandler) new VssHttpMessageHandler(credentials, settings) : (HttpMessageHandler) new VssHttpMessageHandler(credentials, settings, handler);
    }

    private static VssHttpRequestSettings GetVssHttpRequestSettings(
      IVssRequestContext requestContext)
    {
      VssHttpRequestSettings httpRequestSettings = new VssHttpRequestSettings();
      if (httpRequestSettings.UserAgent == null)
        httpRequestSettings.UserAgent = new List<ProductInfoHeaderValue>();
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      string str = service.GetValue(requestContext, in CloudHttpMessageHandlerProvider.s_hostedServiceNameQuery, string.Empty);
      if (!string.IsNullOrEmpty(str))
        httpRequestSettings.UserAgent.Add(new ProductInfoHeaderValue(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "(Service={0})", (object) str)));
      TimeSpan? nullable = service.GetValue<TimeSpan?>(requestContext, in CloudHttpMessageHandlerProvider.s_sendDataTimeoutPath);
      if (nullable.HasValue)
        httpRequestSettings.SendTimeout = nullable.Value;
      return httpRequestSettings;
    }

    private struct HandlerCacheKey
    {
      public readonly Uri BaseUri;
      public readonly Guid TargetServicePrincipal;
      public readonly long CredentialsVersion;
      public readonly string HandlerType;
      public readonly int SettingsVersion;
      public static readonly IEqualityComparer<CloudHttpMessageHandlerProvider.HandlerCacheKey> Comparer = (IEqualityComparer<CloudHttpMessageHandlerProvider.HandlerCacheKey>) new CloudHttpMessageHandlerProvider.HandlerCacheKey.HandlerCacheKeyComparer();

      public HandlerCacheKey(
        Uri baseUri,
        Guid targetServicePrincipal,
        long credentialsVersion,
        string handlerType,
        int settingsVersion)
      {
        this.BaseUri = baseUri;
        this.TargetServicePrincipal = targetServicePrincipal;
        this.CredentialsVersion = credentialsVersion;
        this.HandlerType = handlerType;
        this.SettingsVersion = settingsVersion;
      }

      private class HandlerCacheKeyComparer : 
        IEqualityComparer<CloudHttpMessageHandlerProvider.HandlerCacheKey>
      {
        public bool Equals(
          CloudHttpMessageHandlerProvider.HandlerCacheKey x,
          CloudHttpMessageHandlerProvider.HandlerCacheKey y)
        {
          return x.BaseUri.Equals((object) y.BaseUri) && x.TargetServicePrincipal == y.TargetServicePrincipal && x.CredentialsVersion == y.CredentialsVersion && x.SettingsVersion == y.SettingsVersion && x.HandlerType.Equals(y.HandlerType, StringComparison.Ordinal);
        }

        public int GetHashCode(
          CloudHttpMessageHandlerProvider.HandlerCacheKey obj)
        {
          return (obj.BaseUri.GetHashCode() ^ StringComparer.Ordinal.GetHashCode(obj.HandlerType) ^ obj.TargetServicePrincipal.GetHashCode()) + 4447 * obj.CredentialsVersion.GetHashCode() + obj.SettingsVersion;
        }
      }
    }

    private class HandlerSettings
    {
      public readonly int Version;
      public readonly TimeSpan SendDataTimeout;
      public readonly TimeSpan ReceiveDataTimeout;
      public readonly TimeSpan ReceiveHeadersTimeout;

      public HandlerSettings(IVssRequestContext requestContext, int version)
      {
        List<IEnumerable<RegistryItem>> list = requestContext.GetService<IVssRegistryService>().Read(requestContext, (IEnumerable<RegistryQuery>) new RegistryQuery[3]
        {
          CloudHttpMessageHandlerProvider.s_sendDataTimeoutPath,
          CloudHttpMessageHandlerProvider.s_receiveDataTimeoutPath,
          CloudHttpMessageHandlerProvider.s_receiveHeadersTimeoutPath
        }).ToList<IEnumerable<RegistryItem>>();
        this.SendDataTimeout = CloudHttpMessageHandlerProvider.HandlerSettings.GetSingleValueOrDefault<TimeSpan>(list[0], CloudHttpMessageHandlerProvider.s_defaultSendDataTimeout);
        this.ReceiveDataTimeout = CloudHttpMessageHandlerProvider.HandlerSettings.GetSingleValueOrDefault<TimeSpan>(list[1], CloudHttpMessageHandlerProvider.s_defaultReceiveDataTimeout);
        this.ReceiveHeadersTimeout = CloudHttpMessageHandlerProvider.HandlerSettings.GetSingleValueOrDefault<TimeSpan>(list[2], CloudHttpMessageHandlerProvider.s_defaultReceiveHeadersTimeout);
        this.Version = version;
      }

      private static T GetSingleValueOrDefault<T>(
        IEnumerable<RegistryItem> enumerable,
        T defaultValue)
      {
        return RegistryUtility.FromString<T>(enumerable.Select<RegistryItem, string>((Func<RegistryItem, string>) (x => x.Value)).FirstOrDefault<string>(), defaultValue);
      }
    }
  }
}
