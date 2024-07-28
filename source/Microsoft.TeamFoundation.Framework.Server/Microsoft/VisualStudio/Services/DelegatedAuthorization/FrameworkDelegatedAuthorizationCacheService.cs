// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.FrameworkDelegatedAuthorizationCacheService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization
{
  internal class FrameworkDelegatedAuthorizationCacheService : 
    VssCacheService,
    IFrameworkDelegatedAuthorizationCacheService,
    IVssFrameworkService
  {
    private IRedisCacheService m_redisCacheService;
    private readonly VssMemoryCacheList<string, AccessToken> m_localCache;
    private readonly VssCacheExpiryProvider<string, AccessToken> m_allowAccessExpiryProvider1H;
    private readonly VssCacheExpiryProvider<string, AccessToken> m_denyAccessExpiryProvider;
    private readonly VssCacheExpiryProvider<string, AccessToken> m_denyAccessInactivityExpiryProvider;
    private readonly Capture<TimeSpan> m_allowAccessInactivityInterval;
    private readonly Capture<TimeSpan> m_denyAccessInactivityInterval;
    private readonly Capture<TimeSpan> m_denyAccessExpiryInterval;
    private readonly TimeSpan DefaultNoAccessTokenSlidingExpiration = TimeSpan.FromMinutes(10.0);
    private readonly TimeSpan DefaultNoAccessTokenExpiration = TimeSpan.FromSeconds(5.0);
    private readonly Capture<TimeSpan> NoExpiryTimeInterval = Capture.Create<TimeSpan>(VssCacheExpiryProvider.NoExpiry);
    private static readonly TimeSpan s_cacheCleanupInterval = TimeSpan.FromMinutes(10.0);
    internal static readonly Guid CacheNamespace = new Guid("948DF448-B65A-4E62-BD04-10C195250C00");
    internal static readonly TimeSpan DefaultPatTokenCacheExpirationPolicy = TimeSpan.FromHours(12.0);
    private INotificationRegistration m_delegatedAuthRegistration;
    private const string s_Area = "DelegatedAuthorizationService";
    private const string s_Layer = "FrameworkDelegatedAuthorizationCacheService";
    private const string SessionTokenCacheSlidingExpiration = "/Service/DelegatedAuthorization/SessionTokenCacheSlidingExpiration";
    private const string SessionTokenCacheExpiration = "/Service/DelegatedAuthorization/SessionTokenCacheExpiration";
    private const string NoAccessTokenCacheSlidingExpiration = "/Service/DelegatedAuthorization/NoAccessTokenCacheSlidingExpiration";
    private const string NoAccessTokenCacheExpiration = "/Service/DelegatedAuthorization/NoAccessTokenCacheExpiration";
    private const string DisableRemoteCache = "VisualStudio.Services.DelegatedAuthorization.DisableRemoteCache";
    private const string ExchangeLongLastingTokens = "AzureDevOps.Services.TokenService.ExchangeLongLastingTokens.M167";

    internal virtual TimeSpan DefaultCacheSlidingExpiration { get; } = TimeSpan.FromHours(1.0);

    internal virtual TimeSpan DefaultCacheExpirationOffset { get; } = TimeSpan.FromMinutes(10.0);

    public AccessToken NoAccessTokenEntry => new AccessToken();

    public FrameworkDelegatedAuthorizationCacheService()
    {
      this.m_localCache = new VssMemoryCacheList<string, AccessToken>((IVssCachePerformanceProvider) this);
      this.m_allowAccessInactivityInterval = Capture.Create<TimeSpan>(this.DefaultCacheSlidingExpiration);
      this.m_allowAccessExpiryProvider1H = new VssCacheExpiryProvider<string, AccessToken>(this.m_allowAccessInactivityInterval, this.NoExpiryTimeInterval);
      this.m_denyAccessInactivityInterval = Capture.Create<TimeSpan>(this.DefaultNoAccessTokenSlidingExpiration);
      this.m_denyAccessExpiryInterval = Capture.Create<TimeSpan>(this.DefaultNoAccessTokenExpiration);
      this.m_denyAccessExpiryProvider = new VssCacheExpiryProvider<string, AccessToken>(this.m_denyAccessExpiryInterval, this.NoExpiryTimeInterval);
      this.m_denyAccessInactivityExpiryProvider = new VssCacheExpiryProvider<string, AccessToken>(this.NoExpiryTimeInterval, this.m_denyAccessInactivityInterval);
    }

    internal FrameworkDelegatedAuthorizationCacheService(IRedisCacheService redisCacheService)
      : this()
    {
      this.m_redisCacheService = redisCacheService;
    }

    protected override void ServiceStart(IVssRequestContext systemRequestContext)
    {
      if (!systemRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        throw new UnexpectedHostTypeException(systemRequestContext.ServiceHost.HostType);
      base.ServiceStart(systemRequestContext);
      this.m_redisCacheService = systemRequestContext.GetService<IRedisCacheService>();
      this.m_delegatedAuthRegistration = systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().CreateRegistration(systemRequestContext, "Default", SqlNotificationEventClasses.DelegatedAuthorizationChanged, new SqlNotificationCallback(this.OnDelegatedAuthorizationChangedNotification), false, true);
      this.RegisterCacheServicing<string, AccessToken>(systemRequestContext, (IMemoryCacheList<string, AccessToken>) this.m_localCache, FrameworkDelegatedAuthorizationCacheService.s_cacheCleanupInterval);
    }

    protected override void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      this.m_delegatedAuthRegistration.Unregister(systemRequestContext);
      base.ServiceEnd(systemRequestContext);
    }

    internal TimeSpan GetAccessTokenExpirationInterval(AccessToken accessToken) => accessToken.ValidTo - (DateTimeOffset) DateTime.UtcNow - this.DefaultCacheExpirationOffset;

    public void SetAccessToken(
      IVssRequestContext requestContext,
      string accessTokenKey,
      AccessToken accessToken)
    {
      requestContext.TraceEnter(1048050, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationCacheService), nameof (SetAccessToken));
      this.SetTokenInLocalCache(requestContext, accessTokenKey, accessToken);
      if (!requestContext.IsFeatureEnabled("VisualStudio.Services.DelegatedAuthorization.DisableRemoteCache"))
      {
        ITeamFoundationTaskService service = requestContext.GetService<ITeamFoundationTaskService>();
        TeamFoundationTask teamFoundationTask = new TeamFoundationTask(new TeamFoundationTaskCallback(this.SetTokenInRedisCache), (object) new KeyValuePair<string, AccessToken>(accessTokenKey, accessToken), 0);
        Guid instanceId = requestContext.ServiceHost.InstanceId;
        TeamFoundationTask task = teamFoundationTask;
        service.AddTask(instanceId, task);
      }
      else
        requestContext.Trace(1048058, TraceLevel.Verbose, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationCacheService), "SetAccessToken: skipping caching access token {0} in remove cache.", (object) accessToken.AccessId);
      requestContext.TraceLeave(1048059, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationCacheService), nameof (SetAccessToken));
    }

    public bool TryGetAccessToken(
      IVssRequestContext requestContext,
      string accessTokenKey,
      out AccessToken accessToken)
    {
      requestContext.TraceEnter(1048060, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationCacheService), nameof (TryGetAccessToken));
      accessToken = (AccessToken) null;
      if (this.m_localCache.TryGetValue(accessTokenKey, out accessToken))
      {
        requestContext.Trace(1048061, TraceLevel.Verbose, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationCacheService), "TryGetAccessToken: found access token {0} in local cache.", (object) accessToken.AccessId);
        return true;
      }
      if (!requestContext.IsFeatureEnabled("VisualStudio.Services.DelegatedAuthorization.DisableRemoteCache"))
      {
        if (this.m_redisCacheService.IsEnabled(requestContext))
        {
          bool accessToken1 = this.GetRedisContainer(requestContext).TryGet<string, AccessToken>(requestContext, accessTokenKey, out accessToken);
          if (accessToken1)
          {
            requestContext.Trace(1048062, TraceLevel.Verbose, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationCacheService), "TryGetAccessToken: found access token {0} in remote cache.", (object) accessToken.AccessId);
            this.SetTokenInLocalCache(requestContext, accessTokenKey, accessToken);
            return accessToken1;
          }
        }
        else
          requestContext.Trace(1048065, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationCacheService), "TryGetAccessToken: Redis cache is not enabled.");
      }
      else
        requestContext.Trace(1048066, TraceLevel.Verbose, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationCacheService), "TryGetAccessToken: skipping remote cache lookup.");
      requestContext.TraceLeave(1048069, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationCacheService), nameof (TryGetAccessToken));
      return false;
    }

    private void SetTokenInLocalCache(
      IVssRequestContext requestContext,
      string accessTokenKey,
      AccessToken accessToken)
    {
      TimeSpan timeSpan;
      VssCacheExpiryProvider<string, AccessToken> expiryProvider;
      if (requestContext.IsFeatureEnabled("AzureDevOps.Services.TokenService.ExchangeLongLastingTokens.M167"))
      {
        timeSpan = this.GetAccessTokenExpirationInterval(accessToken);
        if (timeSpan.TotalHours <= 0.0)
          timeSpan = new TimeSpan();
        expiryProvider = new VssCacheExpiryProvider<string, AccessToken>(Capture.Create<TimeSpan>(timeSpan), this.NoExpiryTimeInterval);
      }
      else
      {
        timeSpan = requestContext.GetService<IVssRegistryService>().GetValue<TimeSpan>(requestContext, (RegistryQuery) "/Service/DelegatedAuthorization/SessionTokenCacheSlidingExpiration", this.DefaultCacheSlidingExpiration);
        this.m_allowAccessInactivityInterval.Value = timeSpan;
        expiryProvider = this.m_allowAccessExpiryProvider1H;
      }
      if (timeSpan != new TimeSpan())
      {
        this.m_localCache.Add(accessTokenKey, accessToken, true, expiryProvider);
        requestContext.Trace(1048051, TraceLevel.Verbose, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationCacheService), "SetAccessToken: caching access token {0} in local cache.", (object) accessToken.AccessId);
      }
      else
        requestContext.Trace(1048052, TraceLevel.Verbose, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationCacheService), "SetAccessToken: skipping caching access token {0} in local cache.", (object) accessToken.AccessId);
    }

    public void AddNoAccessTokenEntryToLocalCache(
      IVssRequestContext requestContext,
      string accessTokenKey,
      bool isPublic = false)
    {
      requestContext.TraceEnter(1048090, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationCacheService), nameof (AddNoAccessTokenEntryToLocalCache));
      try
      {
        IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
        if (isPublic)
        {
          TimeSpan timeSpan = service.GetValue<TimeSpan>(requestContext, (RegistryQuery) "/Service/DelegatedAuthorization/NoAccessTokenCacheExpiration", this.DefaultNoAccessTokenExpiration);
          this.m_denyAccessExpiryInterval.Value = timeSpan;
          if (!(timeSpan != new TimeSpan()))
            return;
          requestContext.Trace(1048066, TraceLevel.Verbose, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationCacheService), string.Format("Caching dummy token instance to avoid DoS on SPS for public access token key with expiry interval {0}.", (object) timeSpan));
          this.m_localCache.Add(accessTokenKey, this.NoAccessTokenEntry, true, this.m_denyAccessExpiryProvider);
        }
        else
        {
          TimeSpan timeSpan = service.GetValue<TimeSpan>(requestContext, (RegistryQuery) "/Service/DelegatedAuthorization/NoAccessTokenCacheSlidingExpiration", this.DefaultNoAccessTokenSlidingExpiration);
          this.m_denyAccessInactivityInterval.Value = timeSpan;
          if (!(timeSpan != new TimeSpan()))
            return;
          requestContext.Trace(1048066, TraceLevel.Verbose, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationCacheService), string.Format("Caching dummy token instance to avoid DoS on SPS for access token key with inactivity interval {0}.", (object) timeSpan));
          this.m_localCache.Add(accessTokenKey, this.NoAccessTokenEntry, true, this.m_denyAccessInactivityExpiryProvider);
        }
      }
      finally
      {
        requestContext.TraceLeave(1048099, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationCacheService), nameof (AddNoAccessTokenEntryToLocalCache));
      }
    }

    public void InvalidateAccessToken(IVssRequestContext requestContext, string accessTokenKey)
    {
      requestContext.TraceEnter(1048070, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationCacheService), nameof (InvalidateAccessToken));
      if (this.m_localCache.Remove(accessTokenKey))
        requestContext.Trace(1048071, TraceLevel.Verbose, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationCacheService), "InvalidateAccessToken: invalidating cached access token from local cache.");
      if (!requestContext.IsFeatureEnabled("VisualStudio.Services.DelegatedAuthorization.DisableRemoteCache"))
      {
        TeamFoundationTaskService service = requestContext.GetService<TeamFoundationTaskService>();
        TeamFoundationTask teamFoundationTask = new TeamFoundationTask(new TeamFoundationTaskCallback(this.InvalidateTokenInRedisCache), (object) accessTokenKey, 0);
        Guid instanceId = requestContext.ServiceHost.InstanceId;
        TeamFoundationTask task = teamFoundationTask;
        service.AddTask(instanceId, task);
      }
      else
        requestContext.Trace(1048077, TraceLevel.Verbose, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationCacheService), "InvalidateAccessToken: skipping access token remote cache invalidation.");
      requestContext.TraceLeave(1048079, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationCacheService), nameof (InvalidateAccessToken));
    }

    public void InvalidateAccessTokens(
      IVssRequestContext requestContext,
      IEnumerable<string> accessTokenKeys)
    {
      requestContext.TraceEnter(1048080, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationCacheService), nameof (InvalidateAccessTokens));
      foreach (string accessTokenKey in accessTokenKeys)
      {
        if (!string.IsNullOrWhiteSpace(accessTokenKey))
          this.InvalidateAccessToken(requestContext, accessTokenKey);
      }
      requestContext.TraceLeave(1048089, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationCacheService), nameof (InvalidateAccessTokens));
    }

    public void Clear(IVssRequestContext requestContext)
    {
      requestContext.Trace(1048080, TraceLevel.Verbose, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationCacheService), "Clear: clearing all entries out of a local cache.");
      this.m_localCache.Clear();
    }

    private void InvalidateTokenInRedisCache(IVssRequestContext requestContext, object taskArgs)
    {
      string key = taskArgs as string;
      if (string.IsNullOrWhiteSpace(key))
        return;
      if (this.m_redisCacheService.IsEnabled(requestContext))
      {
        requestContext.Trace(1048089, TraceLevel.Verbose, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationCacheService), "InvalidateAccessToken: invalidating cached access token from remote cache.");
        this.GetRedisContainer(requestContext).Invalidate<string, AccessToken>(requestContext, key);
      }
      else
        requestContext.Trace(1048078, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationCacheService), "InvalidateAccessToken: Redis cache is not enabled.");
    }

    private void SetTokenInRedisCache(IVssRequestContext requestContext, object taskArgs)
    {
      KeyValuePair<string, AccessToken>? nullable = taskArgs as KeyValuePair<string, AccessToken>?;
      if (!nullable.HasValue)
        return;
      if (this.m_redisCacheService.IsEnabled(requestContext))
      {
        requestContext.Trace(1048053, TraceLevel.Verbose, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationCacheService), "SetTokenInRedisCache: caching access token {0} in remote cache.", (object) nullable.Value.Value.AccessId);
        if (requestContext.IsFeatureEnabled("AzureDevOps.Services.TokenService.ExchangeLongLastingTokens.M167"))
        {
          ContainerSettings containerSettings1 = new ContainerSettings();
          KeyValuePair<string, AccessToken> keyValuePair = nullable.Value;
          containerSettings1.KeyExpiry = new TimeSpan?(this.GetAccessTokenExpirationInterval(keyValuePair.Value));
          containerSettings1.CiAreaName = typeof (FrameworkDelegatedAuthorizationCacheService).Name;
          ContainerSettings containerSettings2 = containerSettings1;
          IMutableDictionaryCacheContainer<string, AccessToken> redisContainer = this.GetRedisContainer(requestContext, containerSettings2);
          IVssRequestContext requestContext1 = requestContext;
          Dictionary<string, AccessToken> items = new Dictionary<string, AccessToken>();
          keyValuePair = nullable.Value;
          string key = keyValuePair.Key;
          keyValuePair = nullable.Value;
          AccessToken accessToken = keyValuePair.Value;
          items.Add(key, accessToken);
          if (redisContainer.Set(requestContext1, (IDictionary<string, AccessToken>) items))
            return;
          IVssRequestContext requestContext2 = requestContext;
          keyValuePair = nullable.Value;
          // ISSUE: variable of a boxed type
          __Boxed<Guid> accessId = (ValueType) keyValuePair.Value.AccessId;
          requestContext2.Trace(1048054, TraceLevel.Error, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationCacheService), "SetTokenInRedisCache: Redis cache container set did not set the value for access token {0}.", (object) accessId);
        }
        else
        {
          IMutableDictionaryCacheContainer<string, AccessToken> redisContainer = this.GetRedisContainer(requestContext);
          IVssRequestContext requestContext3 = requestContext;
          Dictionary<string, AccessToken> items = new Dictionary<string, AccessToken>();
          string key = nullable.Value.Key;
          KeyValuePair<string, AccessToken> keyValuePair = nullable.Value;
          AccessToken accessToken = keyValuePair.Value;
          items.Add(key, accessToken);
          if (redisContainer.Set(requestContext3, (IDictionary<string, AccessToken>) items))
            return;
          IVssRequestContext requestContext4 = requestContext;
          keyValuePair = nullable.Value;
          // ISSUE: variable of a boxed type
          __Boxed<Guid> accessId = (ValueType) keyValuePair.Value.AccessId;
          requestContext4.Trace(1048054, TraceLevel.Error, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationCacheService), "SetTokenInRedisCache: Redis cache container set did not set the value for access token {0}.", (object) accessId);
        }
      }
      else
        requestContext.Trace(1048057, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationCacheService), "Redis cache is not enabled.");
    }

    private void OnDelegatedAuthorizationChangedNotification(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      DelegatedAuthorizationMessage authorizationMessage = TeamFoundationSerializationUtility.Deserialize<DelegatedAuthorizationMessage>(eventData);
      requestContext.Trace(1048075, TraceLevel.Info, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationCacheService), "Sql notification received for invalidating cached access token.");
      if (authorizationMessage == null)
        return;
      requestContext.Trace(1048076, TraceLevel.Info, "DelegatedAuthorizationService", nameof (FrameworkDelegatedAuthorizationCacheService), "Sql notification resulted in calling InvalidateAccessTokens API.");
      this.InvalidateAccessTokens(requestContext, (IEnumerable<string>) authorizationMessage.CompactSessionTokenChanges.Where<CompactSessionTokenChange>((Func<CompactSessionTokenChange, bool>) (c => c != null)).Select<CompactSessionTokenChange, string>((Func<CompactSessionTokenChange, string>) (c => c.TokenKey)).ToList<string>());
    }

    private IMutableDictionaryCacheContainer<string, AccessToken> GetRedisContainer(
      IVssRequestContext requestContext,
      ContainerSettings containerSettings = null)
    {
      if (containerSettings == null)
      {
        TimeSpan timeSpan = requestContext.GetService<IVssRegistryService>().GetValue<TimeSpan>(requestContext, (RegistryQuery) "/Service/DelegatedAuthorization/SessionTokenCacheExpiration", FrameworkDelegatedAuthorizationCacheService.DefaultPatTokenCacheExpirationPolicy);
        containerSettings = new ContainerSettings()
        {
          KeyExpiry = new TimeSpan?(timeSpan),
          CiAreaName = typeof (FrameworkDelegatedAuthorizationCacheService).Name
        };
      }
      return this.m_redisCacheService.GetVolatileDictionaryContainer<string, AccessToken, FrameworkDelegatedAuthorizationCacheService.RedisCacheSecurityToken>(requestContext, FrameworkDelegatedAuthorizationCacheService.CacheNamespace, containerSettings);
    }

    internal sealed class RedisCacheSecurityToken
    {
    }
  }
}
