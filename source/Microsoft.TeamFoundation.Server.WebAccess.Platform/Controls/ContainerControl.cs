// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Controls.ContainerControl
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Controls
{
  public class ContainerControl : ControlBase
  {
    public IHtmlString InternalContent { get; set; }

    public ContainerControl(string coreClass, IHtmlString internalContent)
      : base("div")
    {
      this.CoreClass = coreClass;
      this.InternalContent = internalContent;
    }

    protected override void WriteHtmlContents(
      HtmlHelper htmlHelper,
      TagBuilder tag,
      StringBuilder contents)
    {
      base.WriteHtmlContents(htmlHelper, tag, contents);
      if (this.InternalContent == null)
        return;
      contents.AppendLine(this.InternalContent.ToHtmlString());
    }
  }
}
