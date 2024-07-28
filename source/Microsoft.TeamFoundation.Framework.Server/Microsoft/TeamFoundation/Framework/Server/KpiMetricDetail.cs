// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.KpiMetricDetail
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class KpiMetricDetail
  {
    internal KpiMetricDetail(string area, string scope, KpiMetric metric)
    {
      this.Area = area;
      this.Scope = scope;
      this.State = KpiState.None;
      this.Metric = metric;
    }

    public KpiMetric Metric { get; private set; }

    public string Area { get; set; }

    public string Scope { get; set; }

    public string DisplayName { get; set; }

    public string Description { get; set; }

    public KpiState State { get; set; }

    internal Dictionary<string, object> ToJson() => new Dictionary<string, object>()
    {
      ["name"] = (object) this.Metric.Name,
      ["displayName"] = (object) this.DisplayName,
      ["description"] = (object) (this.Description ?? string.Empty),
      ["value"] = (object) this.Metric.Value,
      ["state"] = (object) this.State,
      ["timeStamp"] = (object) this.Metric.TimeStamp
    };

    internal string GenerateMessage() => FrameworkResources.KpiMetricMessage((object) this.Area, (object) this.Scope, (object) this.Metric.Name, (object) this.Description, (object) this.Metric.Value, (object) this.State, (object) this.Metric.TimeStamp.ToString("o"));
  }
}
