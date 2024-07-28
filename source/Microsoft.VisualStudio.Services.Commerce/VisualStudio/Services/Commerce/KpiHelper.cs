// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.KpiHelper
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal static class KpiHelper
  {
    private const string Area = "Commerce";
    private const string Layer = "KpiHelper";

    private static string GetKpiArea(CommerceKpi kpi)
    {
      switch (kpi)
      {
        case CommerceKpi.AzureBillableEvents:
        case CommerceKpi.ReportUsageEventStore:
          return "Microsoft.VisualStudio.MeteringMetrics";
        case CommerceKpi.CommerceCacheHit:
        case CommerceKpi.CommerceCacheMiss:
        case CommerceKpi.PlatformSubscriptionCacheHit:
        case CommerceKpi.PlatformSubscriptionCacheMiss:
        case CommerceKpi.PlatformMeterPriceCacheHit:
        case CommerceKpi.PlatformMeterPriceCacheMiss:
        case CommerceKpi.PlatformMeteringQueueResetJobHit:
          return "Microsoft.VisualStudio.Commerce";
        default:
          return "Microsoft.VisualStudio.Commerce";
      }
    }

    public static void IncrementByOne(this CommerceKpi kpi, IVssRequestContext requestContext) => kpi.Increment(requestContext, 1.0);

    public static void Increment(
      this CommerceKpi kpi,
      IVssRequestContext requestContext,
      double value)
    {
      string kpiArea = KpiHelper.GetKpiArea(kpi);
      KpiHelper.LogKpi(requestContext, kpi.ToString(), kpiArea, value);
    }

    private static void LogKpi(
      IVssRequestContext requestContext,
      string kpiName,
      string kpiArea,
      double value)
    {
      try
      {
        KpiMetric metric = new KpiMetric()
        {
          Name = kpiName,
          Value = value,
          TimeStamp = DateTime.UtcNow
        };
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
        vssRequestContext.GetService<KpiService>().Publish(vssRequestContext, kpiArea, (string) null, metric);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5106158, "Commerce", nameof (KpiHelper), ex);
      }
    }
  }
}
