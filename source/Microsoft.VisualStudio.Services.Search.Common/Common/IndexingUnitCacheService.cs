// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.IndexingUnitCacheService
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  internal class IndexingUnitCacheService : VssMemoryCacheService<int, IndexingUnitCacheObject>
  {
    private static readonly TimeSpan s_cacheCleanupInterval = TimeSpan.FromHours(4.0);
    private static readonly TimeSpan s_maxCacheInactivityAge = TimeSpan.FromHours(24.0);
    private const int MaxCacheElements = 100000;

    public IndexingUnitCacheService()
      : base((IEqualityComparer<int>) EqualityComparer<int>.Default, new MemoryCacheConfiguration<int, IndexingUnitCacheObject>().WithCleanupInterval(IndexingUnitCacheService.s_cacheCleanupInterval).WithInactivityInterval(IndexingUnitCacheService.s_maxCacheInactivityAge).WithMaxElements(100000))
    {
    }

    protected override void ServiceStart(IVssRequestContext systemRC)
    {
      if (systemRC == null)
        throw new ArgumentNullException(nameof (systemRC));
      if (!systemRC.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        throw new UnexpectedHostTypeException(systemRC.ServiceHost.HostType);
      base.ServiceStart(systemRC);
      systemRC.GetService<ITeamFoundationSqlNotificationService>().RegisterNotification(systemRC, "Default", IndexingUnitNotifications.IndexingUnitDeletedEventClass, new SqlNotificationCallback(this.OnIndexingUnitIdDeleted), false);
    }

    protected override void ServiceEnd(IVssRequestContext systemRC)
    {
      systemRC.GetService<ITeamFoundationSqlNotificationService>().UnregisterNotification(systemRC, "Default", IndexingUnitNotifications.IndexingUnitDeletedEventClass, new SqlNotificationCallback(this.OnIndexingUnitIdDeleted), false);
      base.ServiceEnd(systemRC);
    }

    private void OnIndexingUnitIdDeleted(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      if (!object.Equals((object) IndexingUnitNotifications.IndexingUnitDeletedEventClass, (object) eventClass) || string.IsNullOrWhiteSpace(eventData))
        return;
      int key = JsonUtilities.Deserialize<int>(eventData);
      this.Remove(requestContext, key);
    }
  }
}
