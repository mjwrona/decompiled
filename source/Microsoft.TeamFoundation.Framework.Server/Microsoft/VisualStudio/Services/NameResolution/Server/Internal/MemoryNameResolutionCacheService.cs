// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NameResolution.Server.Internal.MemoryNameResolutionCacheService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.NameResolution.Server.Internal
{
  internal class MemoryNameResolutionCacheService : 
    VssMemoryCacheService<NameResolutionKey, NameResolutionEntry>,
    INameResolutionCacheService,
    INameResolutionCache,
    IVssFrameworkService
  {
    private bool m_isMps;
    private INotificationRegistration m_nameResolutionRegistration;

    public MemoryNameResolutionCacheService()
      : base((IEqualityComparer<NameResolutionKey>) NameResolutionKeyComparer.Instance, MemoryNameResolutionCacheService.GetCacheConfiguration())
    {
      this.InactivityInterval.Value = TimeSpan.FromMinutes(15.0);
    }

    protected override void ServiceStart(IVssRequestContext systemRequestContext)
    {
      this.m_isMps = systemRequestContext.ServiceInstanceType() == ServiceInstanceTypes.MPS;
      if (this.m_isMps)
        systemRequestContext.GetService<IDataspacePartitionService>().RegisterNotificationAllPartitions(systemRequestContext, NameResolutionConstants.MpsCategory, SqlNotificationEventClasses.NameResolutionEntriesChanged, new SqlNotificationHandler(this.OnEntriesChanged), false);
      else
        this.m_nameResolutionRegistration = systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().CreateRegistration(systemRequestContext, "Default", SqlNotificationEventClasses.NameResolutionEntriesChanged, new SqlNotificationHandler(this.OnEntriesChanged), false, false);
      this.InactivityInterval.Value = TimeSpan.FromMinutes((double) systemRequestContext.GetService<IVssRegistryService>().GetValue<int>(systemRequestContext, (RegistryQuery) NameResolutionConstants.InactivityIntervalKey, 15));
    }

    protected override void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      if (this.m_isMps)
        systemRequestContext.GetService<IDataspacePartitionService>().UnregisterNotificationAllPartitions(systemRequestContext, NameResolutionConstants.MpsCategory, SqlNotificationEventClasses.NameResolutionEntriesChanged, new SqlNotificationHandler(this.OnEntriesChanged), false);
      else
        this.m_nameResolutionRegistration.Unregister(systemRequestContext);
    }

    public bool IncrementalUpdatesAllowed => true;

    public NameResolutionEntry Get(
      IVssRequestContext requestContext,
      string @namespace,
      string name)
    {
      NameResolutionEntry nameResolutionEntry;
      return !this.TryGetValue(requestContext, new NameResolutionKey(@namespace, name), out nameResolutionEntry) ? (NameResolutionEntry) null : nameResolutionEntry;
    }

    public void Set(IVssRequestContext requestContext, NameResolutionEntry entry) => this.Set(requestContext, new NameResolutionKey(entry.Namespace, entry.Name), entry);

    public void Remove(IVssRequestContext requestContext, NameResolutionEntry entry) => this.Remove(requestContext, new NameResolutionKey(entry.Namespace, entry.Name));

    private void OnEntriesChanged(IVssRequestContext requestContext, NotificationEventArgs args)
    {
      NameResolutionEntryChange[] resolutionEntryChangeArray = args.Deserialize<NameResolutionEntryChange[]>();
      if (resolutionEntryChangeArray == null)
        return;
      foreach (NameResolutionEntryChange resolutionEntryChange in resolutionEntryChangeArray)
        this.Remove(requestContext, new NameResolutionKey(resolutionEntryChange.Namespace, resolutionEntryChange.Name));
    }

    private static MemoryCacheConfiguration<NameResolutionKey, NameResolutionEntry> GetCacheConfiguration() => new MemoryCacheConfiguration<NameResolutionKey, NameResolutionEntry>().WithCleanupInterval(TimeSpan.FromMinutes(15.0)).WithExpiryProvider((ExpiryProviderFactory<NameResolutionKey, NameResolutionEntry>) ((expiryInterval, inactivityInterval, expiryDelay, timeProvider) => (VssCacheExpiryProvider<NameResolutionKey, NameResolutionEntry>) new MemoryNameResolutionCacheService.CacheExpiryProvider(expiryInterval, inactivityInterval, expiryDelay)));

    private class CacheExpiryProvider : 
      VssCacheExpiryProvider<NameResolutionKey, NameResolutionEntry>
    {
      public CacheExpiryProvider(
        Capture<TimeSpan> expiryInterval,
        Capture<TimeSpan> inactivityInterval,
        Capture<DateTime> expiryDelay)
        : base(expiryInterval, inactivityInterval, expiryDelay)
      {
      }

      public override bool IsExpired(
        NameResolutionKey key,
        NameResolutionEntry value,
        DateTime modifiedTimestamp,
        DateTime accessedTimestamp)
      {
        return base.IsExpired(key, value, modifiedTimestamp, accessedTimestamp) || value.IsExpired;
      }
    }
  }
}
