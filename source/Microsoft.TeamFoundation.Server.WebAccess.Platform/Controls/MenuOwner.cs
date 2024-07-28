// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Controls.MenuOwner
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Controls
{
  public abstract class MenuOwner : Menu
  {
    protected MenuOwner()
    {
      this.ShowIcons = true;
      this.EnhancementCssClass = "enhance";
      this.RenderGrandchildren = false;
    }

    public bool ShowIcons { get; set; }

    public string PopupAlign { get; set; }

    protected override void WriteHtmlContents(
      HtmlHelper htmlHelper,
      TagBuilder tag,
      StringBuilder contents)
    {
      base.WriteHtmlContents(htmlHelper, tag, contents);
      object obj = (object) null;
      if (this.HasChildren)
        obj = (object) this.ChildItems.Select<MenuItem, JsObject>((Func<MenuItem, JsObject>) (item => item.ToJson()));
      contents.AppendLine(htmlHelper.JsonIsland((object) new
      {
        items = obj,
        popupAlign = this.PopupAlign,
        showIcon = this.ShowIcons
      }, (object) new{ @class = "options" }).ToHtmlString());
    }
  }
}
