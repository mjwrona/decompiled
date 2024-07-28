// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemKpiTracer
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class WorkItemKpiTracer
  {
    private const string c_KpiAreaName = "Microsoft.TeamFoundation.WorkItemTracking.ServerKpi";

    public static void TraceKpi(
      IVssRequestContext requestContext,
      params WorkItemTrackingKpi[] kpis)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<WorkItemTrackingKpi[]>(kpis, nameof (kpis));
      IEnumerable<WorkItemTrackingKpi> source = ((IEnumerable<WorkItemTrackingKpi>) kpis).Where<WorkItemTrackingKpi>((Func<WorkItemTrackingKpi, bool>) (kpi => !kpi.Skip));
      if (!source.Any<WorkItemTrackingKpi>())
        return;
      DateTime utcNow = DateTime.UtcNow;
      List<KpiMetric> metrics = new List<KpiMetric>();
      foreach (WorkItemTrackingKpi workItemTrackingKpi in source)
      {
        int samplingRate = workItemTrackingKpi.GetSamplingRate(requestContext);
        if (samplingRate > 0 && utcNow.Millisecond % samplingRate == 0)
          metrics.Add(new KpiMetric()
          {
            Name = workItemTrackingKpi.Name,
            Value = workItemTrackingKpi.Value,
            TimeStamp = utcNow
          });
      }
      if (metrics.Count <= 0)
        return;
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
      vssRequestContext.GetService<IKpiService>().Publish(vssRequestContext, "Microsoft.TeamFoundation.WorkItemTracking.ServerKpi", WorkItemKpiTracer.GetAccountHostId(requestContext), (string) null, metrics);
    }

    private static Guid GetAccountHostId(IVssRequestContext requestContext) => requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) ? requestContext.ServiceHost.ParentServiceHost.InstanceId : requestContext.ServiceHost.InstanceId;
  }
}
