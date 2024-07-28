// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tokens.FrameworkTokenCacheService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.DelegatedAuthorization;
using Microsoft.VisualStudio.Services.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Tokens
{
  internal class FrameworkTokenCacheService : 
    VssCacheService,
    IFrameworkTokenCacheService,
    IFrameworkDelegatedAuthorizationCacheService,
    IVssFrameworkService
  {
    private IRedisCacheService m_redisCacheService;
    private readonly VssMemoryCacheList<string, AccessToken> m_localCache;
    private readonly VssCacheExpiryProvider<string, AccessToken> m_allowAccessExpiryProvider;
    private readonly VssCacheExpiryProvider<string, AccessToken> m_denyAccessExpiryProvider;
    private readonly VssCacheExpiryProvider<string, AccessToken> m_denyAccessInactivityExpiryProvider;
    private readonly Capture<TimeSpan> m_allowAccessInactivityInterval;
    private readonly Capture<TimeSpan> m_denyAccessInactivityInterval;
    private readonly Capture<TimeSpan> m_denyAccessExpiryInterval;
    private readonly TimeSpan DefaultCacheSlidingExpiration = TimeSpan.FromHours(1.0);
    private readonly TimeSpan DefaultNoAccessTokenSlidingExpiration = TimeSpan.FromMinutes(10.0);
    private readonly TimeSpan DefaultNoAccessTokenExpiration = TimeSpan.FromSeconds(5.0);
    private readonly Capture<TimeSpan> NoExpiryTimeInterval = Capture.Create<TimeSpan>(VssCacheExpiryProvider.NoExpiry);
    private static readonly TimeSpan s_cacheCleanupInterval = TimeSpan.FromMinutes(10.0);
    internal static readonly Guid CacheNamespace = new Guid("948DF448-B65A-4E62-BD04-10C195250C00");
    internal static readonly TimeSpan CacheExpirationPolicy = TimeSpan.FromHours(12.0);
    internal static readonly ContainerSettings CacheContainerSettings = new ContainerSettings()
    {
      KeyExpiry = new TimeSpan?(FrameworkTokenCacheService.CacheExpirationPolicy),
      CiAreaName = typeof (FrameworkTokenCacheService).Name
    };
    private INotificationRegistration m_delegatedAuthRegistration;
    private const string s_Area = "Token";
    private const string s_Layer = "FrameworkTokenCacheService";
    private const string SessionTokenCacheSlidingExpiration = "/Service/Token/SessionTokenCacheSlidingExpiration";
    private const string NoAccessTokenCacheSlidingExpiration = "/Service/Token/NoAccessTokenCacheSlidingExpiration";
    private const string NoAccessTokenCacheExpiration = "/Service/Token/NoAccessTokenCacheExpiration";
    private const string DisableRemoteCache = "VisualStudio.Services.Token.DisableRemoteCache";

    public AccessToken NoAccessTokenEntry => new AccessToken();

    public FrameworkTokenCacheService()
    {
      this.m_localCache = new VssMemoryCacheList<string, AccessToken>((IVssCachePerformanceProvider) this);
      this.m_allowAccessInactivityInterval = Capture.Create<TimeSpan>(this.DefaultCacheSlidingExpiration);
      this.m_allowAccessExpiryProvider = new VssCacheExpiryProvider<string, AccessToken>(this.NoExpiryTimeInterval, this.m_allowAccessInactivityInterval);
      this.m_denyAccessInactivityInterval = Capture.Create<TimeSpan>(this.DefaultNoAccessTokenSlidingExpiration);
      this.m_denyAccessExpiryInterval = Capture.Create<TimeSpan>(this.DefaultNoAccessTokenExpiration);
      this.m_denyAccessExpiryProvider = new VssCacheExpiryProvider<string, AccessToken>(this.m_denyAccessExpiryInterval, this.NoExpiryTimeInterval);
      this.m_denyAccessInactivityExpiryProvider = new VssCacheExpiryProvider<string, AccessToken>(this.NoExpiryTimeInterval, this.m_denyAccessInactivityInterval);
    }

    protected override void ServiceStart(IVssRequestContext systemRequestContext)
    {
      if (!systemRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        throw new UnexpectedHostTypeException(systemRequestContext.ServiceHost.HostType);
      base.ServiceStart(systemRequestContext);
      this.m_redisCacheService = systemRequestContext.GetService<IRedisCacheService>();
      this.m_delegatedAuthRegistration = systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().CreateRegistration(systemRequestContext, "Default", SqlNotificationEventClasses.TokenChanged, new SqlNotificationCallback(this.OnTokenChangedNotification), false, true);
      this.RegisterCacheServicing<string, AccessToken>(systemRequestContext, (IMemoryCacheList<string, AccessToken>) this.m_localCache, FrameworkTokenCacheService.s_cacheCleanupInterval);
    }

    protected override void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      this.m_delegatedAuthRegistration.Unregister(systemRequestContext);
      base.ServiceEnd(systemRequestContext);
    }

    public void SetAccessToken(
      IVssRequestContext requestContext,
      string accessTokenKey,
      AccessToken accessToken)
    {
      requestContext.TraceEnter(1048250, "Token", nameof (FrameworkTokenCacheService), nameof (SetAccessToken));
      TimeSpan timeSpan = requestContext.GetService<IVssRegistryService>().GetValue<TimeSpan>(requestContext, (RegistryQuery) "/Service/Token/SessionTokenCacheSlidingExpiration", this.DefaultCacheSlidingExpiration);
      this.m_allowAccessInactivityInterval.Value = timeSpan;
      if (timeSpan != new TimeSpan())
      {
        this.m_localCache.Add(accessTokenKey, accessToken, true, this.m_allowAccessExpiryProvider);
        requestContext.Trace(1048251, TraceLevel.Verbose, "Token", nameof (FrameworkTokenCacheService), "SetAccessToken: caching access token {0} in local cache.", (object) accessToken.AccessId);
      }
      else
        requestContext.Trace(1048252, TraceLevel.Verbose, "Token", nameof (FrameworkTokenCacheService), "SetAccessToken: skipping caching access token {0} in local cache.", (object) accessToken.AccessId);
      if (!requestContext.IsFeatureEnabled("VisualStudio.Services.Token.DisableRemoteCache"))
      {
        TeamFoundationTaskService service = requestContext.GetService<TeamFoundationTaskService>();
        TeamFoundationTask teamFoundationTask = new TeamFoundationTask(new TeamFoundationTaskCallback(this.SetTokenInRedisCache), (object) new KeyValuePair<string, AccessToken>(accessTokenKey, accessToken), 0);
        Guid instanceId = requestContext.ServiceHost.InstanceId;
        TeamFoundationTask task = teamFoundationTask;
        service.AddTask(instanceId, task);
      }
      else
        requestContext.Trace(1048258, TraceLevel.Verbose, "Token", nameof (FrameworkTokenCacheService), "SetAccessToken: skipping caching access token {0} in remove cache.", (object) accessToken.AccessId);
      requestContext.TraceLeave(1048259, "Token", nameof (FrameworkTokenCacheService), nameof (SetAccessToken));
    }

    public bool TryGetAccessToken(
      IVssRequestContext requestContext,
      string accessTokenKey,
      out AccessToken accessToken)
    {
      requestContext.TraceEnter(1048260, "Token", nameof (FrameworkTokenCacheService), nameof (TryGetAccessToken));
      try
      {
        if (this.m_localCache.TryGetValue(accessTokenKey, out accessToken))
        {
          requestContext.Trace(1048261, TraceLevel.Verbose, "Token", nameof (FrameworkTokenCacheService), "TryGetAccessToken: found access token {0} in local cache.", (object) accessToken.AccessId);
          return true;
        }
        if (!requestContext.IsFeatureEnabled("VisualStudio.Services.Token.DisableRemoteCache"))
        {
          if (this.m_redisCacheService.IsEnabled(requestContext))
          {
            bool accessToken1 = this.GetRedisContainer(requestContext).TryGet<string, AccessToken>(requestContext, accessTokenKey, out accessToken);
            if (accessToken1)
            {
              requestContext.Trace(1048262, TraceLevel.Verbose, "Token", nameof (FrameworkTokenCacheService), "TryGetAccessToken: found access token {0} in remote cache.", (object) accessToken.AccessId);
              this.SetAccessToken(requestContext, accessTokenKey, accessToken);
              return accessToken1;
            }
          }
          else
            requestContext.Trace(1048265, TraceLevel.Warning, "Token", nameof (FrameworkTokenCacheService), "TryGetAccessToken: Redis cache is not enabled.");
        }
        else
          requestContext.Trace(1048266, TraceLevel.Verbose, "Token", nameof (FrameworkTokenCacheService), "TryGetAccessToken: skipping remote cache lookup.");
      }
      finally
      {
        requestContext.TraceLeave(1048269, "Token", nameof (FrameworkTokenCacheService), nameof (TryGetAccessToken));
      }
      return false;
    }

    public void AddNoAccessTokenEntryToLocalCache(
      IVssRequestContext requestContext,
      string accessTokenKey,
      bool isPublic = false)
    {
      requestContext.TraceEnter(1048290, "Token", nameof (FrameworkTokenCacheService), nameof (AddNoAccessTokenEntryToLocalCache));
      try
      {
        IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
        if (isPublic)
        {
          TimeSpan timeSpan = service.GetValue<TimeSpan>(requestContext, (RegistryQuery) "/Service/Token/NoAccessTokenCacheExpiration", this.DefaultNoAccessTokenExpiration);
          this.m_denyAccessExpiryInterval.Value = timeSpan;
          if (!(timeSpan != new TimeSpan()))
            return;
          requestContext.Trace(1048266, TraceLevel.Verbose, "Token", nameof (FrameworkTokenCacheService), string.Format("Caching dummy token instance to avoid DoS on SPS for public access token key with expiry interval {0}.", (object) timeSpan));
          this.m_localCache.Add(accessTokenKey, this.NoAccessTokenEntry, true, this.m_denyAccessExpiryProvider);
        }
        else
        {
          TimeSpan timeSpan = service.GetValue<TimeSpan>(requestContext, (RegistryQuery) "/Service/Token/NoAccessTokenCacheSlidingExpiration", this.DefaultNoAccessTokenSlidingExpiration);
          this.m_denyAccessInactivityInterval.Value = timeSpan;
          if (!(timeSpan != new TimeSpan()))
            return;
          requestContext.Trace(1048266, TraceLevel.Verbose, "Token", nameof (FrameworkTokenCacheService), string.Format("Caching dummy token instance to avoid DoS on SPS for access token key with inactivity interval {0}.", (object) timeSpan));
          this.m_localCache.Add(accessTokenKey, this.NoAccessTokenEntry, true, this.m_denyAccessInactivityExpiryProvider);
        }
      }
      finally
      {
        requestContext.TraceLeave(1048299, "Token", nameof (FrameworkTokenCacheService), nameof (AddNoAccessTokenEntryToLocalCache));
      }
    }

    public void InvalidateAccessToken(IVssRequestContext requestContext, string accessTokenKey)
    {
      requestContext.TraceEnter(1048270, "Token", nameof (FrameworkTokenCacheService), nameof (InvalidateAccessToken));
      try
      {
        if (this.m_localCache.Remove(accessTokenKey))
          requestContext.Trace(1048071, TraceLevel.Verbose, "Token", nameof (FrameworkTokenCacheService), "InvalidateAccessToken: invalidating cached access token from local cache.");
        if (!requestContext.IsFeatureEnabled("VisualStudio.Services.Token.DisableRemoteCache"))
        {
          TeamFoundationTaskService service = requestContext.GetService<TeamFoundationTaskService>();
          TeamFoundationTask teamFoundationTask = new TeamFoundationTask(new TeamFoundationTaskCallback(this.InvalidateTokenInRedisCache), (object) accessTokenKey, 0);
          Guid instanceId = requestContext.ServiceHost.InstanceId;
          TeamFoundationTask task = teamFoundationTask;
          service.AddTask(instanceId, task);
        }
        else
          requestContext.Trace(1048277, TraceLevel.Verbose, "Token", nameof (FrameworkTokenCacheService), "InvalidateAccessToken: skipping access token remote cache invalidation.");
      }
      finally
      {
        requestContext.TraceLeave(1048279, "Token", nameof (FrameworkTokenCacheService), nameof (InvalidateAccessToken));
      }
    }

    public void Clear(IVssRequestContext requestContext) => this.m_localCache.Clear();

    public void InvalidateAccessTokens(
      IVssRequestContext requestContext,
      IEnumerable<string> accessTokenKeys)
    {
      requestContext.TraceEnter(1048280, "Token", nameof (FrameworkTokenCacheService), nameof (InvalidateAccessTokens));
      try
      {
        foreach (string accessTokenKey in accessTokenKeys)
        {
          if (!string.IsNullOrWhiteSpace(accessTokenKey))
            this.InvalidateAccessToken(requestContext, accessTokenKey);
        }
      }
      finally
      {
        requestContext.TraceLeave(1048289, "Token", nameof (FrameworkTokenCacheService), nameof (InvalidateAccessTokens));
      }
    }

    private void InvalidateTokenInRedisCache(IVssRequestContext requestContext, object taskArgs)
    {
      string key = taskArgs as string;
      if (string.IsNullOrWhiteSpace(key))
        return;
      if (this.m_redisCacheService.IsEnabled(requestContext))
      {
        requestContext.Trace(1048289, TraceLevel.Verbose, "Token", nameof (FrameworkTokenCacheService), "InvalidateAccessToken: invalidating cached access token from remote cache.");
        this.GetRedisContainer(requestContext).Invalidate<string, AccessToken>(requestContext, key);
      }
      else
        requestContext.Trace(1048278, TraceLevel.Warning, "Token", nameof (FrameworkTokenCacheService), "InvalidateAccessToken: Redis cache is not enabled.");
    }

    private void SetTokenInRedisCache(IVssRequestContext requestContext, object taskArgs)
    {
      KeyValuePair<string, AccessToken>? nullable = taskArgs as KeyValuePair<string, AccessToken>?;
      if (!nullable.HasValue)
        return;
      if (this.m_redisCacheService.IsEnabled(requestContext))
      {
        requestContext.Trace(1048253, TraceLevel.Verbose, "Token", nameof (FrameworkTokenCacheService), "SetTokenInRedisCache: caching access token {0} in remote cache.", (object) nullable.Value.Value.AccessId);
        IMutableDictionaryCacheContainer<string, AccessToken> redisContainer = this.GetRedisContainer(requestContext);
        IVssRequestContext requestContext1 = requestContext;
        Dictionary<string, AccessToken> items = new Dictionary<string, AccessToken>();
        string key = nullable.Value.Key;
        KeyValuePair<string, AccessToken> keyValuePair = nullable.Value;
        AccessToken accessToken = keyValuePair.Value;
        items.Add(key, accessToken);
        if (redisContainer.Set(requestContext1, (IDictionary<string, AccessToken>) items))
          return;
        IVssRequestContext requestContext2 = requestContext;
        keyValuePair = nullable.Value;
        // ISSUE: variable of a boxed type
        __Boxed<Guid> accessId = (ValueType) keyValuePair.Value.AccessId;
        requestContext2.Trace(1048254, TraceLevel.Error, "Token", nameof (FrameworkTokenCacheService), "SetTokenInRedisCache: Redis cache container set did not set the value for access token {0}.", (object) accessId);
      }
      else
        requestContext.Trace(1048257, TraceLevel.Warning, "Token", nameof (FrameworkTokenCacheService), "Redis cache is not enabled.");
    }

    private void OnTokenChangedNotification(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      DelegatedAuthorizationMessage authorizationMessage = TeamFoundationSerializationUtility.Deserialize<DelegatedAuthorizationMessage>(eventData);
      requestContext.Trace(1048275, TraceLevel.Info, "Token", nameof (FrameworkTokenCacheService), "Sql notification received for invalidating cached access token.");
      if (authorizationMessage == null)
        return;
      requestContext.Trace(1048276, TraceLevel.Info, "Token", nameof (FrameworkTokenCacheService), "Sql notification resulted in calling InvalidateAccessTokens API.");
      this.InvalidateAccessTokens(requestContext, (IEnumerable<string>) authorizationMessage.CompactSessionTokenChanges.Where<CompactSessionTokenChange>((Func<CompactSessionTokenChange, bool>) (c => c != null)).Select<CompactSessionTokenChange, string>((Func<CompactSessionTokenChange, string>) (c => c.TokenKey)).ToList<string>());
    }

    private IMutableDictionaryCacheContainer<string, AccessToken> GetRedisContainer(
      IVssRequestContext requestContext)
    {
      return this.m_redisCacheService.GetVolatileDictionaryContainer<string, AccessToken, FrameworkTokenCacheService.RedisCacheSecurityToken>(requestContext, FrameworkTokenCacheService.CacheNamespace, FrameworkTokenCacheService.CacheContainerSettings);
    }

    internal sealed class RedisCacheSecurityToken
    {
    }
  }
}
