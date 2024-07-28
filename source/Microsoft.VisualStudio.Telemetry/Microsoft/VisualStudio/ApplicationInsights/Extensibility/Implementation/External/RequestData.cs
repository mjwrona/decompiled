// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.External.RequestData
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.External
{
  [GeneratedCode("gbc", "3.02")]
  internal class RequestData
  {
    public int ver { get; set; }

    public string id { get; set; }

    public string name { get; set; }

    public string startTime { get; set; }

    public string duration { get; set; }

    public string responseCode { get; set; }

    public bool success { get; set; }

    public string httpMethod { get; set; }

    public string url { get; set; }

    public IDictionary<string, string> properties { get; set; }

    public IDictionary<string, double> measurements { get; set; }

    public RequestData()
      : this("AI.RequestData", nameof (RequestData))
    {
    }

    protected RequestData(string fullName, string name)
    {
      this.ver = 2;
      this.id = string.Empty;
      this.name = string.Empty;
      this.startTime = string.Empty;
      this.duration = string.Empty;
      this.responseCode = string.Empty;
      this.httpMethod = string.Empty;
      this.url = string.Empty;
      this.properties = (IDictionary<string, string>) new SortedDictionary<string, string>((IComparer<string>) StringComparer.Ordinal);
      this.measurements = (IDictionary<string, double>) new SortedDictionary<string, double>((IComparer<string>) StringComparer.Ordinal);
    }
  }
}
