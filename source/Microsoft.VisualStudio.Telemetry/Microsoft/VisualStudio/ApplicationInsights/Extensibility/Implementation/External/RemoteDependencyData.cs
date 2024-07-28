// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.External.RemoteDependencyData
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.External
{
  [GeneratedCode("gbc", "3.02")]
  internal class RemoteDependencyData
  {
    public int ver { get; set; }

    public string name { get; set; }

    public DataPointType kind { get; set; }

    public double value { get; set; }

    public int? count { get; set; }

    public double? min { get; set; }

    public double? max { get; set; }

    public double? stdDev { get; set; }

    public DependencyKind dependencyKind { get; set; }

    public bool? success { get; set; }

    public bool? async { get; set; }

    public DependencySourceType dependencySource { get; set; }

    public string commandName { get; set; }

    public string dependencyTypeName { get; set; }

    public IDictionary<string, string> properties { get; set; }

    public RemoteDependencyData()
      : this("AI.RemoteDependencyData", nameof (RemoteDependencyData))
    {
    }

    protected RemoteDependencyData(string fullName, string name)
    {
      this.ver = 2;
      this.name = string.Empty;
      this.kind = DataPointType.Measurement;
      this.dependencyKind = DependencyKind.Other;
      this.success = new bool?(true);
      this.dependencySource = DependencySourceType.Undefined;
      this.commandName = string.Empty;
      this.dependencyTypeName = string.Empty;
      this.properties = (IDictionary<string, string>) new SortedDictionary<string, string>((IComparer<string>) StringComparer.Ordinal);
    }
  }
}
