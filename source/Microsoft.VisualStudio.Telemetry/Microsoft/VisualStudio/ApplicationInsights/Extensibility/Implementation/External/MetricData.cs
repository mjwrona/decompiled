// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.External.MetricData
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.External
{
  [GeneratedCode("gbc", "3.02")]
  internal class MetricData
  {
    public int ver { get; set; }

    public IList<DataPoint> metrics { get; set; }

    public IDictionary<string, string> properties { get; set; }

    public MetricData()
      : this("AI.MetricData", nameof (MetricData))
    {
    }

    protected MetricData(string fullName, string name)
    {
      this.ver = 2;
      this.metrics = (IList<DataPoint>) new List<DataPoint>();
      this.properties = (IDictionary<string, string>) new SortedDictionary<string, string>((IComparer<string>) StringComparer.Ordinal);
    }
  }
}
