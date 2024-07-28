// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WebGlobalMessage
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public class WebGlobalMessage
  {
    public string Message { get; set; }

    public WebMessageLevel MessageLevel { get; set; }

    public bool HtmlFormat { get; set; }

    public bool Closeable { get; set; }

    public string CssClass { get; set; }

    public string IconClass
    {
      get
      {
        switch (this.MessageLevel)
        {
          case WebMessageLevel.Warning:
            return "bowtie-status-warning";
          case WebMessageLevel.Error:
            return "bowtie-status-error";
          default:
            return "bowtie-status-info";
        }
      }
    }
  }
}
