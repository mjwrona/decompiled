// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.UserPreferencesCacheService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Redis;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class UserPreferencesCacheService : IUserPreferencesCacheService, IVssFrameworkService
  {
    private static readonly string Area = "UserPreferencesService";
    private static readonly string Layer = "Cache";
    private static readonly Guid CacheContainerId = new Guid("{742BA25A-1C7D-4C9D-A5F3-54D758F4508E}");
    private static readonly TimeSpan CacheExpirationPolicy = TimeSpan.FromDays(7.0);
    private static readonly ContainerSettings CacheContainerSettings = new ContainerSettings()
    {
      KeyExpiry = new TimeSpan?(UserPreferencesCacheService.CacheExpirationPolicy),
      CiAreaName = typeof (UserPreferencesCacheService).Name,
      NoThrowMode = new bool?(true)
    };

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(21003500, UserPreferencesCacheService.Area, UserPreferencesCacheService.Layer, nameof (ServiceStart));
      systemRequestContext.CheckDeploymentRequestContext();
      systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().RegisterNotification(systemRequestContext, "Default", SqlNotificationEventClasses.UserPreferenceChanged, new SqlNotificationCallback(this.OnUserPreferenceChanged), true);
      systemRequestContext.TraceLeave(21003501, UserPreferencesCacheService.Area, UserPreferencesCacheService.Layer, nameof (ServiceStart));
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(21003502, UserPreferencesCacheService.Area, UserPreferencesCacheService.Layer, nameof (ServiceEnd));
      systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().UnregisterNotification(systemRequestContext, "Default", SqlNotificationEventClasses.UserPreferenceChanged, new SqlNotificationCallback(this.OnUserPreferenceChanged), false);
      systemRequestContext.TraceLeave(21003503, UserPreferencesCacheService.Area, UserPreferencesCacheService.Layer, nameof (ServiceEnd));
    }

    public bool TryGetValue(IVssRequestContext requestContext, Guid key, out UserPreferences value)
    {
      UserPreferencesCacheService.UserPreferencesMemoryCacheService service = requestContext.GetService<UserPreferencesCacheService.UserPreferencesMemoryCacheService>();
      if (service.TryGetValue(requestContext, key, out value))
        return true;
      IMutableDictionaryCacheContainer<Guid, UserPreferences> cacheContainer;
      if (!this.TryGetRedisCache(requestContext, out cacheContainer) || !cacheContainer.TryGet<Guid, UserPreferences>(requestContext, key, out value))
        return false;
      service.Set(requestContext, key, value);
      return true;
    }

    public void Set(IVssRequestContext requestContext, Guid key, UserPreferences value)
    {
      requestContext.GetService<UserPreferencesCacheService.UserPreferencesMemoryCacheService>().Set(requestContext, key, value);
      IMutableDictionaryCacheContainer<Guid, UserPreferences> cacheContainer;
      if (UserPreferencesCacheService.IsDefaultUserPreferences(value) || !this.TryGetRedisCache(requestContext, out cacheContainer))
        return;
      cacheContainer.Set(requestContext, (IDictionary<Guid, UserPreferences>) new Dictionary<Guid, UserPreferences>()
      {
        {
          key,
          value
        }
      });
    }

    public bool Remove(IVssRequestContext requestContext, Guid key)
    {
      IMutableDictionaryCacheContainer<Guid, UserPreferences> cacheContainer;
      if (this.TryGetRedisCache(requestContext, out cacheContainer))
        cacheContainer.Invalidate<Guid, UserPreferences>(requestContext, key);
      return requestContext.GetService<UserPreferencesCacheService.UserPreferencesMemoryCacheService>().Remove(requestContext, key);
    }

    private bool TryGetRedisCache(
      IVssRequestContext requestContext,
      out IMutableDictionaryCacheContainer<Guid, UserPreferences> cacheContainer)
    {
      IRedisCacheService service = requestContext.GetService<IRedisCacheService>();
      if (!service.IsEnabled(requestContext))
      {
        cacheContainer = (IMutableDictionaryCacheContainer<Guid, UserPreferences>) null;
        return false;
      }
      cacheContainer = service.GetVolatileDictionaryContainer<Guid, UserPreferences, UserPreferencesCacheService.MyCacheSecurityToken>(requestContext, UserPreferencesCacheService.CacheContainerId, UserPreferencesCacheService.CacheContainerSettings);
      return true;
    }

    private void OnUserPreferenceChanged(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      Guid result;
      if (!Guid.TryParse(eventData, out result))
        return;
      this.Remove(requestContext, result);
    }

    internal static bool IsDefaultUserPreferences(UserPreferences value) => value == UserPreferencesService.DefaultUserPreferences;

    private class MyCacheSecurityToken
    {
    }

    internal class UserPreferencesMemoryCacheService : VssMemoryCacheService<Guid, UserPreferences>
    {
      private static readonly string name = "UserPreferencesCache";
      private VssCacheExpiryProvider<Guid, UserPreferences> m_expirationForDefaultValues;
      private static readonly TimeSpan maxCacheExpiryInterval = TimeSpan.FromHours(24.0);
      private static readonly TimeSpan maxCacheInactivityAge = TimeSpan.FromHours(2.0);

      public UserPreferencesMemoryCacheService()
      {
        this.ExpiryInterval.Value = UserPreferencesCacheService.UserPreferencesMemoryCacheService.maxCacheExpiryInterval;
        this.InactivityInterval.Value = UserPreferencesCacheService.UserPreferencesMemoryCacheService.maxCacheInactivityAge;
        this.m_expirationForDefaultValues = new VssCacheExpiryProvider<Guid, UserPreferences>(this.InactivityInterval, this.InactivityInterval);
      }

      public override void Set(IVssRequestContext requestContext, Guid key, UserPreferences value)
      {
        if (UserPreferencesCacheService.IsDefaultUserPreferences(value))
          this.MemoryCache.Add(key, value, true, this.m_expirationForDefaultValues);
        else
          base.Set(requestContext, key, value);
      }

      public override string Name => UserPreferencesCacheService.UserPreferencesMemoryCacheService.name;
    }
  }
}
