// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NameResolution.Server.PrimaryNameResolutionEntryCacheService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.NameResolution.Server.Internal;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.NameResolution.Server
{
  internal class PrimaryNameResolutionEntryCacheService : 
    VssMemoryCacheService<Guid, NameResolutionEntry>
  {
    private bool m_isMps;
    private INotificationRegistration m_nameResolutionRegistration;

    public PrimaryNameResolutionEntryCacheService()
      : base((IEqualityComparer<Guid>) EqualityComparer<Guid>.Default, PrimaryNameResolutionEntryCacheService.GetCacheConfiguration())
    {
      this.ExpiryInterval.Value = TimeSpan.FromHours(4.0);
      this.MaxCacheLength.Value = 150000;
    }

    protected override void ServiceStart(IVssRequestContext systemRequestContext)
    {
      this.m_isMps = systemRequestContext.ServiceInstanceType() == ServiceInstanceTypes.MPS;
      if (this.m_isMps)
        systemRequestContext.GetService<IDataspacePartitionService>().RegisterNotificationAllPartitions(systemRequestContext, NameResolutionConstants.MpsCategory, SqlNotificationEventClasses.NameResolutionEntriesChanged, new SqlNotificationHandler(this.OnEntriesChanged), false);
      else
        this.m_nameResolutionRegistration = systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().CreateRegistration(systemRequestContext, "Default", SqlNotificationEventClasses.NameResolutionEntriesChanged, new SqlNotificationHandler(this.OnEntriesChanged), false, false);
    }

    protected override void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      if (this.m_isMps)
        systemRequestContext.GetService<IDataspacePartitionService>().UnregisterNotificationAllPartitions(systemRequestContext, NameResolutionConstants.MpsCategory, SqlNotificationEventClasses.NameResolutionEntriesChanged, new SqlNotificationHandler(this.OnEntriesChanged), false);
      else
        this.m_nameResolutionRegistration.Unregister(systemRequestContext);
    }

    private void OnEntriesChanged(IVssRequestContext requestContext, NotificationEventArgs args)
    {
      NameResolutionEntryChange[] resolutionEntryChangeArray = args.Deserialize<NameResolutionEntryChange[]>();
      if (resolutionEntryChangeArray == null)
        return;
      foreach (NameResolutionEntryChange resolutionEntryChange in resolutionEntryChangeArray)
      {
        if (resolutionEntryChange.OldValue != Guid.Empty)
          this.Remove(requestContext, resolutionEntryChange.OldValue);
        if (resolutionEntryChange.NewValue != Guid.Empty && resolutionEntryChange.NewValue != resolutionEntryChange.OldValue)
          this.Remove(requestContext, resolutionEntryChange.NewValue);
      }
    }

    private static MemoryCacheConfiguration<Guid, NameResolutionEntry> GetCacheConfiguration() => new MemoryCacheConfiguration<Guid, NameResolutionEntry>().WithExpiryProvider((ExpiryProviderFactory<Guid, NameResolutionEntry>) ((expiryInterval, inactivityInterval, expiryDelay, timeProvider) => (VssCacheExpiryProvider<Guid, NameResolutionEntry>) new PrimaryNameResolutionEntryCacheService.CacheExpiryProvider(expiryInterval, inactivityInterval, expiryDelay)));

    private class CacheExpiryProvider : VssCacheExpiryProvider<Guid, NameResolutionEntry>
    {
      public CacheExpiryProvider(
        Capture<TimeSpan> expiryInterval,
        Capture<TimeSpan> inactivityInterval,
        Capture<DateTime> expiryDelay)
        : base(expiryInterval, inactivityInterval, expiryDelay)
      {
      }

      public override bool IsExpired(
        Guid key,
        NameResolutionEntry value,
        DateTime modifiedTimestamp,
        DateTime accessedTimestamp)
      {
        return value != null && value.ExpiresOn.HasValue ? value.ExpiresOn.Value <= DateTime.UtcNow : base.IsExpired(key, value, modifiedTimestamp, accessedTimestamp);
      }
    }
  }
}
