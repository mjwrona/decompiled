// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebApiConverters.BuildMetricExtensions
// Assembly: Microsoft.TeamFoundation.Build2.WebApiConverters, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9963E502-0ADF-445A-89CE-AAA11161F2F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebApiConverters.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build2.WebApiConverters
{
  public static class BuildMetricExtensions
  {
    public static Microsoft.TeamFoundation.Build.WebApi.BuildMetric ToWebApiBuildMetric(
      this Microsoft.TeamFoundation.Build2.Server.BuildMetric srvBuildMetric,
      ISecuredObject securedObject)
    {
      ArgumentUtility.CheckForNull<ISecuredObject>(securedObject, nameof (securedObject));
      if (srvBuildMetric == null)
        return (Microsoft.TeamFoundation.Build.WebApi.BuildMetric) null;
      return new Microsoft.TeamFoundation.Build.WebApi.BuildMetric(securedObject)
      {
        Name = srvBuildMetric.Name,
        Scope = srvBuildMetric.Scope,
        Date = srvBuildMetric.Date,
        IntValue = srvBuildMetric.IntValue
      };
    }

    public static Microsoft.TeamFoundation.Build2.Server.BuildMetric ToServerBuildMetric(
      this Microsoft.TeamFoundation.Build.WebApi.BuildMetric webApiBuildMetric)
    {
      if (webApiBuildMetric == null)
        return (Microsoft.TeamFoundation.Build2.Server.BuildMetric) null;
      return new Microsoft.TeamFoundation.Build2.Server.BuildMetric()
      {
        Name = webApiBuildMetric.Name,
        Scope = webApiBuildMetric.Scope,
        Date = webApiBuildMetric.Date,
        IntValue = webApiBuildMetric.IntValue
      };
    }

    public static List<Microsoft.TeamFoundation.Build2.Server.BuildMetric> MergePullRequestMetrics(
      this IEnumerable<Microsoft.TeamFoundation.Build2.Server.BuildMetric> metrics,
      IVssRequestContext requestContext)
    {
      List<Microsoft.TeamFoundation.Build2.Server.BuildMetric> buildMetricList = new List<Microsoft.TeamFoundation.Build2.Server.BuildMetric>();
      if (metrics != null)
      {
        using (PerformanceTimer.StartMeasure(requestContext, "BuildMetricExtensions.MergePullRequestMetrics"))
        {
          Dictionary<string, Dictionary<DateTime, Microsoft.TeamFoundation.Build2.Server.BuildMetric>> dictionary1 = new Dictionary<string, Dictionary<DateTime, Microsoft.TeamFoundation.Build2.Server.BuildMetric>>((IEqualityComparer<string>) StringComparer.Ordinal);
          Dictionary<string, Microsoft.TeamFoundation.Build2.Server.BuildMetric> dictionary2 = new Dictionary<string, Microsoft.TeamFoundation.Build2.Server.BuildMetric>((IEqualityComparer<string>) StringComparer.Ordinal);
          foreach (Microsoft.TeamFoundation.Build2.Server.BuildMetric metric in metrics)
          {
            if (metric.Scope != null && metric.Scope.StartsWith("refs/pull/", StringComparison.Ordinal))
            {
              Microsoft.TeamFoundation.Build2.Server.BuildMetric buildMetric = (Microsoft.TeamFoundation.Build2.Server.BuildMetric) null;
              if (metric.Date.HasValue)
              {
                Dictionary<DateTime, Microsoft.TeamFoundation.Build2.Server.BuildMetric> dictionary3 = (Dictionary<DateTime, Microsoft.TeamFoundation.Build2.Server.BuildMetric>) null;
                if (!dictionary1.TryGetValue(metric.Name, out dictionary3))
                {
                  dictionary3 = new Dictionary<DateTime, Microsoft.TeamFoundation.Build2.Server.BuildMetric>();
                  dictionary1.Add(metric.Name, dictionary3);
                }
                if (!dictionary3.TryGetValue(metric.Date.Value, out buildMetric))
                {
                  buildMetric = BuildMetricExtensions.CreateBuildServerMetric(metric.Name, metric.Date);
                  dictionary3.Add(metric.Date.Value, buildMetric);
                }
              }
              else if (!dictionary2.TryGetValue(metric.Name, out buildMetric))
              {
                buildMetric = BuildMetricExtensions.CreateBuildServerMetric(metric.Name, metric.Date);
                dictionary2.Add(metric.Name, buildMetric);
              }
              buildMetric.IntValue += metric.IntValue;
            }
            else
              buildMetricList.Add(metric);
          }
          foreach (Microsoft.TeamFoundation.Build2.Server.BuildMetric buildMetric in dictionary2.Values)
            buildMetricList.Add(buildMetric);
          foreach (Dictionary<DateTime, Microsoft.TeamFoundation.Build2.Server.BuildMetric> dictionary4 in dictionary1.Values)
          {
            foreach (Microsoft.TeamFoundation.Build2.Server.BuildMetric buildMetric in dictionary4.Values)
              buildMetricList.Add(buildMetric);
          }
        }
      }
      return buildMetricList;
    }

    private static Microsoft.TeamFoundation.Build2.Server.BuildMetric CreateBuildServerMetric(
      string name,
      DateTime? date)
    {
      return new Microsoft.TeamFoundation.Build2.Server.BuildMetric()
      {
        Name = name,
        Date = date,
        Scope = "refs/pull/*",
        IntValue = 0
      };
    }
  }
}
