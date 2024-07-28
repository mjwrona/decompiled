// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildMetricExtensions
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public static class BuildMetricExtensions
  {
    public static List<BuildMetric> MergePullRequestMetrics(
      this IEnumerable<BuildMetric> metrics,
      IVssRequestContext requestContext)
    {
      List<BuildMetric> buildMetricList = new List<BuildMetric>();
      if (metrics != null)
      {
        using (PerformanceTimer.StartMeasure(requestContext, "BuildMetricExtensions.MergePullRequestMetrics"))
        {
          Dictionary<string, Dictionary<DateTime, BuildMetric>> dictionary1 = new Dictionary<string, Dictionary<DateTime, BuildMetric>>((IEqualityComparer<string>) StringComparer.Ordinal);
          Dictionary<string, BuildMetric> dictionary2 = new Dictionary<string, BuildMetric>((IEqualityComparer<string>) StringComparer.Ordinal);
          foreach (BuildMetric metric in metrics)
          {
            if (metric.Scope != null && metric.Scope.StartsWith("refs/pull/", StringComparison.Ordinal))
            {
              BuildMetric buildMetric = (BuildMetric) null;
              if (metric.Date.HasValue)
              {
                Dictionary<DateTime, BuildMetric> dictionary3 = (Dictionary<DateTime, BuildMetric>) null;
                if (!dictionary1.TryGetValue(metric.Name, out dictionary3))
                {
                  dictionary3 = new Dictionary<DateTime, BuildMetric>();
                  dictionary1.Add(metric.Name, dictionary3);
                }
                if (!dictionary3.TryGetValue(metric.Date.Value, out buildMetric))
                {
                  buildMetric = BuildMetricExtensions.CreateMetric(metric.Name, metric.Date);
                  dictionary3.Add(metric.Date.Value, buildMetric);
                }
              }
              else if (!dictionary2.TryGetValue(metric.Name, out buildMetric))
              {
                buildMetric = BuildMetricExtensions.CreateMetric(metric.Name, metric.Date);
                dictionary2.Add(metric.Name, buildMetric);
              }
              buildMetric.IntValue += metric.IntValue;
            }
            else
              buildMetricList.Add(metric);
          }
          foreach (BuildMetric buildMetric in dictionary2.Values)
            buildMetricList.Add(buildMetric);
          foreach (Dictionary<DateTime, BuildMetric> dictionary4 in dictionary1.Values)
          {
            foreach (BuildMetric buildMetric in dictionary4.Values)
              buildMetricList.Add(buildMetric);
          }
        }
      }
      return buildMetricList;
    }

    private static BuildMetric CreateMetric(string name, DateTime? date) => new BuildMetric()
    {
      Name = name,
      Date = date,
      Scope = "refs/pull/*",
      IntValue = 0
    };
  }
}
