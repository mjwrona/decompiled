// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.External.PageViewData
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System.CodeDom.Compiler;

namespace Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.External
{
  [GeneratedCode("gbc", "3.02")]
  internal class PageViewData : EventData
  {
    public string url { get; set; }

    public string duration { get; set; }

    public PageViewData()
      : this("AI.PageViewData", nameof (PageViewData))
    {
    }

    protected PageViewData(string fullName, string name)
    {
      this.url = string.Empty;
      this.duration = string.Empty;
    }
  }
}
