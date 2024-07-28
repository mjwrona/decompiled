// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.External.AjaxCallData
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System.CodeDom.Compiler;

namespace Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.External
{
  [GeneratedCode("gbc", "3.02")]
  internal class AjaxCallData : PageViewData
  {
    public string ajaxUrl { get; set; }

    public double requestSize { get; set; }

    public double responseSize { get; set; }

    public string timeToFirstByte { get; set; }

    public string timeToLastByte { get; set; }

    public string callbackDuration { get; set; }

    public string responseCode { get; set; }

    public bool success { get; set; }

    public AjaxCallData()
      : this("AI.AjaxCallData", nameof (AjaxCallData))
    {
    }

    protected AjaxCallData(string fullName, string name)
    {
      this.ajaxUrl = string.Empty;
      this.timeToFirstByte = string.Empty;
      this.timeToLastByte = string.Empty;
      this.callbackDuration = string.Empty;
      this.responseCode = string.Empty;
    }
  }
}
