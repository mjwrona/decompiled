// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.LicenseClaimLocalCache
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Licensing
{
  internal class LicenseClaimLocalCache : VssMemoryCacheService<string, ILicenseClaim>
  {
    private static readonly TimeSpan s_cleanUpInterval = TimeSpan.FromMinutes(15.0);
    private const string TraceArea = "Licensing";
    private const string TraceLayer = "LocalCache";
    private static readonly PerformanceTracer Tracer = new PerformanceTracer(LicenseClaimPerfCounters.StandardSet, "Licensing", "LocalCache");

    public LicenseClaimLocalCache()
      : base((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase, LicenseClaimLocalCache.s_cleanUpInterval)
    {
    }

    protected override void ServiceStart(IVssRequestContext requestContext)
    {
      base.ServiceStart(requestContext);
      requestContext.CheckHostedDeployment();
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      vssRequestContext.GetService<IVssRegistryService>().RegisterNotification(vssRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged), "/BusinessPolicy/...");
      this.PopulateSettings(requestContext);
    }

    protected override void ServiceEnd(IVssRequestContext requestContext)
    {
      requestContext.TraceConditionally(1045998, TraceLevel.Verbose, "Licensing", "LocalCache", (Func<string>) (() => string.Format("ServiceEnd, before cleaning up configuration cache. Service Host: {0}", (object) requestContext.ServiceHost.InstanceId)));
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      vssRequestContext.GetService<IVssRegistryService>().UnregisterNotification(vssRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged));
      base.ServiceEnd(requestContext);
    }

    private void OnRegistryChanged(
      IVssRequestContext context,
      RegistryEntryCollection changedEntries)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      ArgumentUtility.CheckForNull<RegistryEntryCollection>(changedEntries, nameof (changedEntries));
      if (!changedEntries.Any<RegistryEntry>())
        return;
      this.PopulateSettings(context);
    }

    private void PopulateSettings(IVssRequestContext context)
    {
      IVssRequestContext vssRequestContext = context.To(TeamFoundationHostType.Deployment);
      IVssRegistryService service = vssRequestContext.GetService<IVssRegistryService>();
      Constants.RegistrySettings registrySettings = new Constants.RegistrySettings();
      registrySettings.CacheConfigurationRefreshInterval = service.GetValue<TimeSpan>(vssRequestContext, (RegistryQuery) "/BusinessPolicy/CacheConfigurationRefreshInterval", Constants.RegistrySettings.DefaultCacheConfigurationRefreshInterval);
      registrySettings.CacheEntryStateIdleLifetime = service.GetValue<TimeSpan>(vssRequestContext, (RegistryQuery) "/BusinessPolicy/CacheEntryStateIdleLifetime", Constants.RegistrySettings.DefaultCacheEntryStateIdleLifetime);
      this.Settings = registrySettings;
      this.InactivityInterval.Value = registrySettings.CacheEntryStateIdleLifetime;
    }

    public new bool Remove(IVssRequestContext requestContext, string key)
    {
      using (LicenseClaimLocalCache.Tracer.TraceTimedAction(requestContext, LicenseClaimLocalCache.TracePoints.SlowRemove, 50, nameof (Remove)))
      {
        bool removed = base.Remove(requestContext, key);
        LicenseClaimLocalCache.Tracer.Trace(requestContext, 1045803, TraceLevel.Verbose, (Func<string>) (() => string.Format("Removed {0} from L1 cache for host {1}: {2}", (object) key, (object) requestContext.ServiceHost.InstanceId, (object) removed)), nameof (Remove));
        return removed;
      }
    }

    public new void Set(IVssRequestContext requestContext, string key, ILicenseClaim value)
    {
      using (LicenseClaimLocalCache.Tracer.TraceTimedAction(requestContext, LicenseClaimLocalCache.TracePoints.SlowSet, 50, nameof (Set)))
      {
        base.Set(requestContext, key, value);
        LicenseClaimLocalCache.Tracer.Trace(requestContext, 1045813, TraceLevel.Verbose, (Func<string>) (() => string.Format("Set({0}) for host {1} in L1 cache", (object) key, (object) requestContext.ServiceHost.InstanceId)), nameof (Set));
      }
    }

    public new bool TryPeekValue(
      IVssRequestContext requestContext,
      string key,
      out ILicenseClaim value)
    {
      using (LicenseClaimLocalCache.Tracer.TraceTimedAction(requestContext, LicenseClaimLocalCache.TracePoints.SlowGet, 50, nameof (TryPeekValue)))
      {
        int num = base.TryPeekValue(requestContext, key, out value) ? 1 : 0;
        if (num != 0)
          LicenseClaimLocalCache.Tracer.TraceCacheHit(requestContext, 1045824, key, LicenseClaimLocalCache.TraceConstants.LocalCacheLookupOperationName);
        else
          LicenseClaimLocalCache.Tracer.TraceCacheMiss(requestContext, 1045836, key, LicenseClaimLocalCache.TraceConstants.LocalCacheLookupOperationName);
        return num != 0;
      }
    }

    private Constants.RegistrySettings Settings { get; set; }

    private static class TraceConstants
    {
      internal static readonly string LocalCacheLookupOperationName = "Get";
    }

    private static class TracePoints
    {
      internal static readonly int SlowRemove = 1045607;
      internal static readonly int SlowSet = 1045617;
      internal static readonly int SlowGet = 1045627;
    }
  }
}
