// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.Cache.AadLocalCache
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Microsoft.VisualStudio.Services.Aad.Cache
{
  internal class AadLocalCache : IAadLocalCache, IVssFrameworkService, IAadCache
  {
    private readonly Type[] types;
    private readonly ConcurrentDictionary<Type, object> containers;

    internal AadLocalCache()
      : this(typeof (AadCacheAncestorIds), typeof (AadCacheTenant), typeof (AadCacheDescendantIds), typeof (AadCacheDirectoryRoles), typeof (AadCacheDirectoryRoleMembers), typeof (AadCacheUserRolesAndGroups))
    {
    }

    internal AadLocalCache(params Type[] types)
    {
      AadCacheUtils.ValidateTypes((IEnumerable<Type>) types);
      this.types = types;
      this.containers = new ConcurrentDictionary<Type, object>();
    }

    public void ServiceStart(IVssRequestContext context)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      if (!context.ExecutionEnvironment.IsHostedDeployment)
        throw new InvalidOperationException(FrameworkResources.ServiceAvailableInHostedTfsOnly());
      context.CheckServiceHostType(TeamFoundationHostType.Deployment);
      context.GetService<IVssRegistryService>().RegisterNotification(context, new RegistrySettingsChangedCallback(this.OnRegistryChanged), "/Service/Aad/Cache/LocalCache/...");
      this.CreateOrUpdateContainers(context);
    }

    public void ServiceEnd(IVssRequestContext context)
    {
      context.CheckServiceHostType(TeamFoundationHostType.Deployment);
      context.GetService<IVssRegistryService>().UnregisterNotification(context, new RegistrySettingsChangedCallback(this.OnRegistryChanged));
    }

    private void OnRegistryChanged(
      IVssRequestContext context,
      RegistryEntryCollection changedEntries)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      ArgumentUtility.CheckForNull<RegistryEntryCollection>(changedEntries, nameof (changedEntries));
      if (!changedEntries.Any<RegistryEntry>())
        return;
      this.CreateOrUpdateContainers(context);
    }

    private void CreateOrUpdateContainers(IVssRequestContext context)
    {
      foreach (Type type in this.types)
      {
        int registryValue1 = AadCacheUtils.GetRegistryValue<int>(context, type, 1024, AadCacheConstants.DefaultSettings.LocalCache.ContainerSize, "/Service/Aad/Cache/LocalCache/ContainerSize", AadCacheConstants.RegistryKeys.LocalCache.ContainerSize);
        int registryValue2 = AadCacheUtils.GetRegistryValue<int>(context, type, 5, AadCacheConstants.DefaultSettings.LocalCache.EvictionSampleSize, "/Service/Aad/Cache/LocalCache/EvictionSampleSize", AadCacheConstants.RegistryKeys.LocalCache.EvictionSampleSize);
        this.containers[type] = Activator.CreateInstance(typeof (AadLocalCacheContainer<>).MakeGenericType(type), BindingFlags.Instance | BindingFlags.NonPublic, (Binder) null, new object[2]
        {
          (object) registryValue1,
          (object) registryValue2
        }, (CultureInfo) null);
      }
    }

    public IEnumerable<AadCacheLookup<T>> GetObjects<T>(
      IVssRequestContext context,
      IEnumerable<AadCacheKey> keys)
      where T : AadCacheObject
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      ArgumentUtility.CheckForNull<IEnumerable<AadCacheKey>>(keys, nameof (keys));
      return !context.IsFeatureEnabled("Microsoft.VisualStudio.Services.Aad.Cache.LocalCache") ? keys.Select<AadCacheKey, AadCacheLookup<T>>((Func<AadCacheKey, AadCacheLookup<T>>) (key => new AadCacheLookup<T>(key, AadCacheLookupStatus.Miss, (Exception) null))) : this.GetContainer<T>().GetObjects(context, keys);
    }

    public void AddObjects<T>(IVssRequestContext context, IEnumerable<T> objects) where T : AadCacheObject
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      ArgumentUtility.CheckForNull<IEnumerable<T>>(objects, nameof (objects));
      if (!context.IsFeatureEnabled("Microsoft.VisualStudio.Services.Aad.Cache.LocalCache"))
        return;
      this.GetContainer<T>().AddObjects(context, objects);
    }

    public void RemoveObjects<T>(IVssRequestContext context, IEnumerable<AadCacheKey> keys) where T : AadCacheObject
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      ArgumentUtility.CheckForNull<IEnumerable<AadCacheKey>>(keys, nameof (keys));
      if (!context.IsFeatureEnabled("Microsoft.VisualStudio.Services.Aad.Cache.LocalCache"))
        return;
      this.GetContainer<T>().RemoveObjects(context, keys);
    }

    private AadLocalCacheContainer<T> GetContainer<T>() where T : AadCacheObject => (AadLocalCacheContainer<T>) this.containers[typeof (T)];
  }
}
