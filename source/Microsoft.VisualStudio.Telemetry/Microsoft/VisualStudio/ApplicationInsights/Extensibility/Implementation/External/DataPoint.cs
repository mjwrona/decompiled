// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.External.DataPoint
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System.CodeDom.Compiler;

namespace Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.External
{
  [GeneratedCode("gbc", "3.02")]
  internal class DataPoint
  {
    public string name { get; set; }

    public DataPointType kind { get; set; }

    public double value { get; set; }

    public int? count { get; set; }

    public double? min { get; set; }

    public double? max { get; set; }

    public double? stdDev { get; set; }

    public DataPoint()
      : this("AI.DataPoint", nameof (DataPoint))
    {
    }

    protected DataPoint(string fullName, string name)
    {
      this.name = string.Empty;
      this.kind = DataPointType.Measurement;
    }
  }
}
