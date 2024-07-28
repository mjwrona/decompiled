// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.LicenseClaimCacheService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Licensing
{
  internal class LicenseClaimCacheService : ILicenseClaimCacheService, IVssFrameworkService
  {
    private Guid m_serviceHostId;
    private INotificationRegistration m_entitlementRegistration;
    private const string TraceArea = "Licensing";
    private const string TraceLayer = "Cache";
    private static readonly PerformanceTracer Tracer = new PerformanceTracer(LicenseClaimPerfCounters.StandardSet, "Licensing", "Cache");

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      this.m_serviceHostId = systemRequestContext.ServiceHost.InstanceId;
      IVssRequestContext vssRequestContext = systemRequestContext.To(TeamFoundationHostType.Deployment);
      this.m_entitlementRegistration = vssRequestContext.GetService<ITeamFoundationSqlNotificationService>().CreateRegistration(vssRequestContext, "Default", SqlNotificationEventClasses.UserEntitlementChanged, new SqlNotificationHandler(this.OnUserEntitlementChanged), true, true);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext) => this.m_entitlementRegistration.Unregister(systemRequestContext.To(TeamFoundationHostType.Deployment));

    private void OnUserEntitlementChanged(
      IVssRequestContext requestContext,
      NotificationEventArgs args)
    {
      try
      {
        EntitlementChangeMessage entitlementChangeMessage = TeamFoundationSerializationUtility.Deserialize<EntitlementChangeMessage>(args.Data);
        if (entitlementChangeMessage == null)
          return;
        string[] strArray = LicenseClaimCacheKeysGenerator.Generate(requestContext, entitlementChangeMessage);
        if (strArray == null)
          return;
        foreach (string str in strArray)
        {
          string cacheKey = str;
          requestContext.TraceConditionally(1045011, TraceLevel.Info, "Licensing", "Cache", (Func<string>) (() => string.Format("Removing cache entry for key: {0}", (object) cacheKey)));
          this.Remove(requestContext, cacheKey);
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1045019, "Licensing", "Cache", ex);
      }
    }

    public bool Remove(IVssRequestContext requestContext, string key)
    {
      this.ValidateRequestContext(requestContext);
      ArgumentUtility.CheckStringForNullOrEmpty(key, nameof (key));
      using (LicenseClaimCacheService.Tracer.TraceTimedAction(requestContext, LicenseClaimCacheService.TracePoints.Remove.Slow, 200, nameof (Remove)))
        return LicenseClaimCacheService.Tracer.TraceAction<bool>(requestContext, (ActionTracePoints) LicenseClaimCacheService.TracePoints.Remove, (Func<bool>) (() => requestContext.GetService<LicenseClaimRemoteCache>().Remove(requestContext, key) & requestContext.GetService<LicenseClaimLocalCache>().Remove(requestContext, key)), nameof (Remove));
    }

    public void Set(IVssRequestContext requestContext, string key, ILicenseClaim value)
    {
      this.ValidateRequestContext(requestContext);
      ArgumentUtility.CheckStringForNullOrEmpty(key, nameof (key));
      using (LicenseClaimCacheService.Tracer.TraceTimedAction(requestContext, LicenseClaimCacheService.TracePoints.Set.Slow, 200, nameof (Set)))
        LicenseClaimCacheService.Tracer.TraceAction(requestContext, (ActionTracePoints) LicenseClaimCacheService.TracePoints.Set, (Action) (() =>
        {
          ILicenseClaim licenseClaim = value;
          requestContext.GetService<LicenseClaimRemoteCache>().Set(requestContext, key, licenseClaim);
          requestContext.GetService<LicenseClaimLocalCache>().Set(requestContext, key, licenseClaim);
        }), nameof (Set));
    }

    public bool TryGetValue(IVssRequestContext requestContext, string key, out ILicenseClaim value)
    {
      this.ValidateRequestContext(requestContext);
      ArgumentUtility.CheckStringForNullOrEmpty(key, nameof (key));
      using (LicenseClaimCacheService.Tracer.TraceTimedAction(requestContext, LicenseClaimCacheService.TracePoints.Get.Slow, 200, nameof (TryGetValue)))
      {
        try
        {
          requestContext.TraceEnter(LicenseClaimCacheService.TracePoints.Get.Enter, "Licensing", "Cache", nameof (TryGetValue));
          LicenseClaimLocalCache service = requestContext.GetService<LicenseClaimLocalCache>();
          ILicenseClaim licenseClaim1;
          if (service.TryPeekValue(requestContext, key, out licenseClaim1))
          {
            value = licenseClaim1;
            return true;
          }
          try
          {
            ILicenseClaim licenseClaim2;
            if (requestContext.GetService<LicenseClaimRemoteCache>().TryGetValue(requestContext, key, out licenseClaim2))
            {
              service.Set(requestContext, key, licenseClaim2);
              value = licenseClaim2;
              return true;
            }
          }
          catch (Exception ex)
          {
            requestContext.TraceException(LicenseClaimCacheService.TracePoints.Get.Exception, "Licensing", "Cache", ex);
          }
          value = (ILicenseClaim) null;
          return false;
        }
        finally
        {
          requestContext.TraceLeave(LicenseClaimCacheService.TracePoints.Get.Exit, "Licensing", "Cache", nameof (TryGetValue));
        }
      }
    }

    private void ValidateRequestContext(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (!this.m_serviceHostId.Equals(requestContext.ServiceHost.InstanceId))
        throw new InvalidRequestContextHostException(FrameworkResources.CacheServiceRequestContextHostMessage((object) this.m_serviceHostId, (object) requestContext.ServiceHost.InstanceId));
    }

    private static class TracePoints
    {
      internal static readonly TimedActionTracePoints Remove = new TimedActionTracePoints(1045801, 1045807, 1045808, 1045809);
      internal static readonly TimedActionTracePoints Set = new TimedActionTracePoints(1045811, 1045817, 1045818, 1045819);
      internal static readonly TimedActionTracePoints Get = new TimedActionTracePoints(1045821, 1045827, 1045828, 1045829);
    }
  }
}
