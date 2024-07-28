// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.External.PerformanceCounterData
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System.CodeDom.Compiler;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.External
{
  [GeneratedCode("gbc", "3.02")]
  internal class PerformanceCounterData
  {
    public int ver { get; set; }

    public string categoryName { get; set; }

    public string counterName { get; set; }

    public string instanceName { get; set; }

    public DataPointType kind { get; set; }

    public int? count { get; set; }

    public double? min { get; set; }

    public double? max { get; set; }

    public double? stdDev { get; set; }

    public double value { get; set; }

    public IDictionary<string, string> properties { get; set; }

    public PerformanceCounterData()
      : this("AI.PerformanceCounterData", nameof (PerformanceCounterData))
    {
    }

    protected PerformanceCounterData(string fullName, string name)
    {
      this.ver = 2;
      this.categoryName = string.Empty;
      this.counterName = string.Empty;
      this.instanceName = string.Empty;
      this.kind = DataPointType.Aggregation;
      this.properties = (IDictionary<string, string>) new Dictionary<string, string>();
    }
  }
}
