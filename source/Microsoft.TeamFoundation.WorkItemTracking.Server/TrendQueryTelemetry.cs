// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.TrendQueryTelemetry
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class TrendQueryTelemetry : QueryTelemetry
  {
    private const string c_thresholdRegistryPath = "/Service/WorkItemTracking/Settings/TraceTrendQueryAboveThreshold";
    private const int c_defaultThresholdTime = 0;

    public TrendQueryTelemetry(IVssRequestContext requestContext, string feature)
      : base(requestContext, feature, "/Service/WorkItemTracking/Settings/TraceTrendQueryAboveThreshold", 0)
    {
    }

    public new static string Feature => "TrendQuery";

    public override void AddData(params object[] param)
    {
      base.AddData(param.Length > 8 ? ((IEnumerable<object>) param).Take<object>(8).ToArray<object>() : param);
      if (param.Length > 1 && param[1] is AsOfDateTimesQueryResult timesQueryResult)
        this.ClientTraceData.Add("ResultCount", (object) timesQueryResult.WorkItemResults.Count<AsOfQueryResultEntry>());
      if (param.Length <= 9 || !(param[9] is IEnumerable<DateTime> source))
        return;
      this.ClientTraceData.Add("TrendAsOfDates", (object) string.Join(",", source.Select<DateTime, string>((Func<DateTime, string>) (t => t.ToString("u")))));
    }
  }
}
