// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.External.PageViewPerfData
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System.CodeDom.Compiler;

namespace Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.External
{
  [GeneratedCode("gbc", "3.02")]
  internal class PageViewPerfData : PageViewData
  {
    public string perfTotal { get; set; }

    public string networkConnect { get; set; }

    public string sentRequest { get; set; }

    public string receivedResponse { get; set; }

    public string domProcessing { get; set; }

    public PageViewPerfData()
      : this("AI.PageViewPerfData", nameof (PageViewPerfData))
    {
    }

    protected PageViewPerfData(string fullName, string name)
    {
      this.perfTotal = string.Empty;
      this.networkConnect = string.Empty;
      this.sentRequest = string.Empty;
      this.receivedResponse = string.Empty;
      this.domProcessing = string.Empty;
    }
  }
}
