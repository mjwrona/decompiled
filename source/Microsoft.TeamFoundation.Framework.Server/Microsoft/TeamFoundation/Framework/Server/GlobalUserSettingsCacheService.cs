// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.GlobalUserSettingsCacheService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Redis;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class GlobalUserSettingsCacheService : 
    IGlobalUserSettingsCacheService,
    IVssFrameworkService
  {
    private static readonly string Area = "UserSettingsService";
    private static readonly string Layer = "Cache";
    private static readonly Guid CacheContainerId = new Guid("{DD1C9F4F-59E6-4144-9D8A-5D51A4B0C5A9}");
    private static readonly TimeSpan CacheExpirationPolicy = TimeSpan.FromDays(7.0);
    private static readonly ContainerSettings CacheContainerSettings = new ContainerSettings()
    {
      KeyExpiry = new TimeSpan?(GlobalUserSettingsCacheService.CacheExpirationPolicy),
      CiAreaName = typeof (GlobalUserSettingsCacheService).Name
    };

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(0, GlobalUserSettingsCacheService.Area, GlobalUserSettingsCacheService.Layer, nameof (ServiceStart));
      systemRequestContext.CheckDeploymentRequestContext();
      ITeamFoundationSqlNotificationService service = systemRequestContext.GetService<ITeamFoundationSqlNotificationService>();
      service.RegisterNotification(systemRequestContext, "Default", SqlNotificationEventClasses.UserSettingsChanged, new SqlNotificationCallback(this.OnUserSettingsChanged), true);
      service.RegisterNotification(systemRequestContext, "Default", SqlNotificationEventClasses.UserSettingsCacheUpdated, new SqlNotificationCallback(this.OnUserSettingsCacheUpdated), true);
      systemRequestContext.TraceLeave(0, GlobalUserSettingsCacheService.Area, GlobalUserSettingsCacheService.Layer, nameof (ServiceStart));
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(0, GlobalUserSettingsCacheService.Area, GlobalUserSettingsCacheService.Layer, nameof (ServiceEnd));
      ITeamFoundationSqlNotificationService service = systemRequestContext.GetService<ITeamFoundationSqlNotificationService>();
      service.UnregisterNotification(systemRequestContext, "Default", SqlNotificationEventClasses.UserSettingsChanged, new SqlNotificationCallback(this.OnUserSettingsChanged), false);
      service.UnregisterNotification(systemRequestContext, "Default", SqlNotificationEventClasses.UserSettingsCacheUpdated, new SqlNotificationCallback(this.OnUserSettingsCacheUpdated), false);
      systemRequestContext.TraceLeave(0, GlobalUserSettingsCacheService.Area, GlobalUserSettingsCacheService.Layer, nameof (ServiceEnd));
    }

    public (bool, GlobalUserSettings) TryGetValue(IVssRequestContext requestContext, Guid key)
    {
      GlobalUserSettingsCacheService.UserSettingsMemoryCacheService service = requestContext.GetService<GlobalUserSettingsCacheService.UserSettingsMemoryCacheService>();
      GlobalUserSettings globalUserSettings1;
      if (service.TryGetValue(requestContext, key, out globalUserSettings1))
      {
        if (globalUserSettings1 == null)
          requestContext.Trace(10013550, TraceLevel.Info, GlobalUserSettingsCacheService.Area, GlobalUserSettingsCacheService.Layer, "GlobalUserSettingsCacheService.TryGetValue: Null settings in memory cache set for key: {0}", (object) key);
        if (globalUserSettings1.UserAttributes == null)
          requestContext.Trace(10013551, TraceLevel.Info, GlobalUserSettingsCacheService.Area, GlobalUserSettingsCacheService.Layer, "GlobalUserSettingsCacheService.TryGetValue: Null UserAttributes in memory cache  set for key: {0}", (object) key);
        requestContext.Trace(10013554, TraceLevel.Info, GlobalUserSettingsCacheService.Area, GlobalUserSettingsCacheService.Layer, "GlobalUserSettingsCacheService.TryGetValue: Returning UserAttributes from memory cache for key: {0}", (object) key);
        return (true, globalUserSettings1);
      }
      IMutableDictionaryCacheContainer<Guid, GlobalUserSettings> cacheContainer;
      GlobalUserSettings globalUserSettings2;
      if (!this.TryGetRedisCache(requestContext, out cacheContainer) || !cacheContainer.TryGet<Guid, GlobalUserSettings>(requestContext, key, out globalUserSettings2))
        return (false, (GlobalUserSettings) null);
      if (globalUserSettings2 == null)
        requestContext.Trace(10013552, TraceLevel.Info, GlobalUserSettingsCacheService.Area, GlobalUserSettingsCacheService.Layer, "GlobalUserSettingsCacheService.TryGetValue: Null settings in redis cache set for key: {0}", (object) key);
      if (globalUserSettings2.UserAttributes == null)
        requestContext.Trace(10013553, TraceLevel.Info, GlobalUserSettingsCacheService.Area, GlobalUserSettingsCacheService.Layer, "GlobalUserSettingsCacheService.TryGetValue: Null UserAttributes in redis cache  set for key: {0}", (object) key);
      service.Set(requestContext, key, globalUserSettings2);
      requestContext.Trace(10013555, TraceLevel.Info, GlobalUserSettingsCacheService.Area, GlobalUserSettingsCacheService.Layer, "GlobalUserSettingsCacheService.TryGetValue: Returning UserAttributes from redis cache for key: {0}", (object) key);
      return (true, globalUserSettings2);
    }

    public void Set(IVssRequestContext requestContext, Guid key, GlobalUserSettings value)
    {
      if (value == null || value.UserAttributes == null)
        requestContext.Trace(10013546, TraceLevel.Info, GlobalUserSettingsCacheService.Area, GlobalUserSettingsCacheService.Layer, "GlobalUserSettingsCacheService: Attempting to set null settings for key: {0}", (object) key);
      requestContext.GetService<GlobalUserSettingsCacheService.UserSettingsMemoryCacheService>().Set(requestContext, key, value);
      IMutableDictionaryCacheContainer<Guid, GlobalUserSettings> cacheContainer;
      if (value.IsInError || !this.TryGetRedisCache(requestContext, out cacheContainer))
        return;
      cacheContainer.Set(requestContext, (IDictionary<Guid, GlobalUserSettings>) new Dictionary<Guid, GlobalUserSettings>()
      {
        {
          key,
          value
        }
      });
      requestContext.GetService<ITeamFoundationSqlNotificationService>().SendNotification(requestContext, SqlNotificationEventClasses.UserSettingsCacheUpdated, key.ToString("D"));
    }

    public bool Remove(IVssRequestContext requestContext, Guid key) => this.Remove(requestContext, key, false);

    private bool Remove(IVssRequestContext requestContext, Guid key, bool memoryCacheOnly)
    {
      IMutableDictionaryCacheContainer<Guid, GlobalUserSettings> cacheContainer;
      if (!memoryCacheOnly && this.TryGetRedisCache(requestContext, out cacheContainer))
        cacheContainer.Invalidate<Guid, GlobalUserSettings>(requestContext, key);
      return requestContext.GetService<GlobalUserSettingsCacheService.UserSettingsMemoryCacheService>().Remove(requestContext, key);
    }

    private bool TryGetRedisCache(
      IVssRequestContext requestContext,
      out IMutableDictionaryCacheContainer<Guid, GlobalUserSettings> cacheContainer)
    {
      IRedisCacheService service = requestContext.GetService<IRedisCacheService>();
      if (!service.IsEnabled(requestContext))
      {
        cacheContainer = (IMutableDictionaryCacheContainer<Guid, GlobalUserSettings>) null;
        return false;
      }
      cacheContainer = service.GetVolatileDictionaryContainer<Guid, GlobalUserSettings, GlobalUserSettingsCacheService.UserSettingsCacheSecurityToken>(requestContext, GlobalUserSettingsCacheService.CacheContainerId, GlobalUserSettingsCacheService.CacheContainerSettings);
      return true;
    }

    private void OnUserSettingsChanged(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      List<Guid> guidList = JsonConvert.DeserializeObject<List<Guid>>(eventData);
      if (guidList == null)
        return;
      foreach (Guid key in guidList)
        this.Remove(requestContext, key);
    }

    private void OnUserSettingsCacheUpdated(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      Guid result;
      if (!Guid.TryParse(eventData, out result))
        return;
      this.Remove(requestContext, result, true);
    }

    private class UserSettingsCacheSecurityToken
    {
    }

    internal class UserSettingsMemoryCacheService : VssMemoryCacheService<Guid, GlobalUserSettings>
    {
      private static readonly string name = nameof (UserSettingsMemoryCacheService);
      private VssCacheExpiryProvider<Guid, GlobalUserSettings> m_expirationForDefaultValues;
      private static readonly TimeSpan maxCacheExpiryInterval = TimeSpan.FromHours(24.0);
      private static readonly TimeSpan maxCacheInactivityAge = TimeSpan.FromHours(2.0);

      public UserSettingsMemoryCacheService()
      {
        this.ExpiryInterval.Value = GlobalUserSettingsCacheService.UserSettingsMemoryCacheService.maxCacheExpiryInterval;
        this.InactivityInterval.Value = GlobalUserSettingsCacheService.UserSettingsMemoryCacheService.maxCacheInactivityAge;
        this.m_expirationForDefaultValues = new VssCacheExpiryProvider<Guid, GlobalUserSettings>(this.InactivityInterval, this.InactivityInterval);
      }

      public override void Set(
        IVssRequestContext requestContext,
        Guid key,
        GlobalUserSettings value)
      {
        if (value.IsInError)
          this.MemoryCache.Add(key, value, true, this.m_expirationForDefaultValues);
        else
          base.Set(requestContext, key, value);
      }

      public override string Name => GlobalUserSettingsCacheService.UserSettingsMemoryCacheService.name;
    }
  }
}
