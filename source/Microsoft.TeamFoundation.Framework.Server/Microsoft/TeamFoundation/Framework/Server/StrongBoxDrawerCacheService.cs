// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.StrongBoxDrawerCacheService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class StrongBoxDrawerCacheService : 
    VssVersionedMemoryCacheService<string, StrongBoxDrawerInfo>
  {
    private INotificationRegistration m_drawerRegistration;
    private IVssMemoryCacheGrouping<string, StrongBoxDrawerInfo, Guid> m_drawerByGuidGrouping;
    private static readonly Lazy<XmlSerializer> s_drawerSerializer = new Lazy<XmlSerializer>((Func<XmlSerializer>) (() => new XmlSerializer(typeof (List<StrongBoxDrawerName>))));

    public StrongBoxDrawerCacheService()
      : base((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
    }

    protected override void ServiceStart(IVssRequestContext requestContext)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      bool flag = requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment);
      this.MaxCacheLength.Value = service.GetValue<int>(requestContext, (RegistryQuery) FrameworkServerConstants.StrongBoxCacheDrawerCount, flag ? 5000 : 0);
      this.m_drawerByGuidGrouping = VssMemoryCacheGroupingFactory.Create<string, StrongBoxDrawerInfo, Guid>(requestContext, this.MemoryCache, (Func<string, StrongBoxDrawerInfo, IEnumerable<Guid>>) ((x, y) => (IEnumerable<Guid>) new Guid[1]
      {
        y.DrawerId
      }), groupingBehavior: VssMemoryCacheGroupingBehavior.Replace);
      base.ServiceStart(requestContext);
      this.m_drawerRegistration = requestContext.GetService<ITeamFoundationSqlNotificationService>().CreateRegistration(requestContext, "Default", SqlNotificationEventClasses.StrongBoxDrawerChanged, new SqlNotificationHandler(this.OnDrawerChanged), false, false);
    }

    protected override void ServiceEnd(IVssRequestContext requestContext)
    {
      this.m_drawerRegistration.Unregister(requestContext);
      base.ServiceEnd(requestContext);
    }

    public StrongBoxDrawerInfo GetDrawerInfo(
      IVssRequestContext requestContext,
      string drawerName,
      Func<string, StrongBoxDrawerInfo> missDelegate)
    {
      StrongBoxDrawerInfo drawerInfo1;
      if (this.TryGetValue(requestContext, drawerName, out drawerInfo1))
        return drawerInfo1;
      using (IVssVersionedCacheContext<string, StrongBoxDrawerInfo> versionedContext = this.CreateVersionedContext(requestContext))
      {
        StrongBoxDrawerInfo drawerInfo2 = missDelegate(drawerName);
        if (drawerInfo2 != null)
        {
          CacheUpdateResult cacheUpdateResult = versionedContext.TryUpdate(requestContext, drawerName, drawerInfo2);
          if (cacheUpdateResult != CacheUpdateResult.Success)
            requestContext.Trace(109116, TraceLevel.Verbose, "StrongBox", "Service", "Unable to add StrongBoxDrawerInfo {0} to cache. Reason:{1}", (object) drawerName, (object) cacheUpdateResult);
        }
        return drawerInfo2;
      }
    }

    public virtual StrongBoxDrawerInfo GetDrawerInfo(
      IVssRequestContext requestContext,
      Guid drawerId,
      Func<Guid, StrongBoxDrawerInfo> missDelegate)
    {
      IEnumerable<string> keys;
      if (this.m_drawerByGuidGrouping.TryGetKeys(drawerId, out keys))
      {
        string key = keys.FirstOrDefault<string>();
        StrongBoxDrawerInfo drawerInfo;
        if (!string.IsNullOrEmpty(key) && this.TryGetValue(requestContext, key, out drawerInfo))
          return drawerInfo;
      }
      using (IVssVersionedCacheContext<string, StrongBoxDrawerInfo> versionedContext = this.CreateVersionedContext(requestContext))
      {
        StrongBoxDrawerInfo drawerInfo = missDelegate(drawerId);
        if (drawerInfo != null)
        {
          CacheUpdateResult cacheUpdateResult = versionedContext.TryUpdate(requestContext, drawerInfo.Name, drawerInfo);
          if (cacheUpdateResult != CacheUpdateResult.Success)
            requestContext.Trace(109117, TraceLevel.Verbose, "StrongBox", "Service", "Unable to add StrongBoxDrawerInfo {0} to cache. Reason:{1}", (object) drawerId, (object) cacheUpdateResult);
        }
        return drawerInfo;
      }
    }

    private void OnDrawerChanged(IVssRequestContext requestContext, NotificationEventArgs args)
    {
      if (string.IsNullOrWhiteSpace(args.Data))
        return;
      foreach (StrongBoxDrawerName strongBoxDrawerName in (List<StrongBoxDrawerName>) StrongBoxDrawerCacheService.s_drawerSerializer.Value.Deserialize((TextReader) new StringReader(args.Data)))
      {
        using (IVssVersionedCacheContext<string, StrongBoxDrawerInfo> versionedContext = this.CreateVersionedContext(requestContext))
          versionedContext.Invalidate(requestContext, strongBoxDrawerName.DrawerName);
      }
    }
  }
}
