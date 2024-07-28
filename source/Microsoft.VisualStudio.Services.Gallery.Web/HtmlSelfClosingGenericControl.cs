// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Web.HtmlSelfClosingGenericControl
// Assembly: Microsoft.VisualStudio.Services.Gallery.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 17D36576-2EF3-4ABC-94BA-AF7891D15A3A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Web.dll

using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace Microsoft.VisualStudio.Services.Gallery.Web
{
  public class HtmlSelfClosingGenericControl : HtmlGenericControl
  {
    public HtmlSelfClosingGenericControl()
    {
    }

    public HtmlSelfClosingGenericControl(string tag)
      : base(tag)
    {
    }

    protected override void Render(HtmlTextWriter writer)
    {
      writer.Write("<" + this.TagName);
      this.Attributes.Render(writer);
      writer.Write(" />");
    }
  }
}
