// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Controls.PivotView
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

namespace Microsoft.TeamFoundation.Server.WebAccess.Controls
{
  public class PivotView
  {
    public PivotView()
    {
    }

    public PivotView(string text) => this.Text = text;

    public string Text { get; set; }

    public string Title { get; set; }

    public string Link { get; set; }

    public string Id { get; set; }

    public bool Encoded { get; set; }

    public bool Selected { get; set; }

    public bool Disabled { get; set; }

    public bool Hidden { get; set; }
  }
}
