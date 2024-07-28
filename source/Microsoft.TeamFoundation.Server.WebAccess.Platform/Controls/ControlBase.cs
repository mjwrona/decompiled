// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Controls.ControlBase
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;

namespace Microsoft.TeamFoundation.Server.WebAccess.Controls
{
  public abstract class ControlBase
  {
    protected ControlBase(string tagName) => this.TagName = tagName;

    public string ControlId { get; set; }

    public string TagName { get; set; }

    public object HtmlAttributes { get; set; }

    public string CoreClass { get; protected set; }

    public string EnhancementCssClass { get; set; }

    public string CssClass { get; set; }

    public virtual MvcHtmlString ToHtml(HtmlHelper htmlHelper) => this.ToHtml(htmlHelper, (IDictionary<string, object>) null);

    public virtual MvcHtmlString ToHtml(
      HtmlHelper htmlHelper,
      IDictionary<string, object> htmlAttributes)
    {
      TagBuilder tag = new TagBuilder(this.TagName);
      this.PopulateRootTag(tag, htmlAttributes);
      StringBuilder contents = new StringBuilder();
      this.WriteHtmlContents(htmlHelper, tag, contents);
      tag.InnerHtml = contents.ToString();
      return MvcHtmlString.Create(tag.ToString());
    }

    protected void PopulateRootTag(TagBuilder tag, IDictionary<string, object> htmlAttributes)
    {
      if (this.HtmlAttributes != null)
        tag.MergeAttributes<string, object>((IDictionary<string, object>) new RouteValueDictionary(this.HtmlAttributes));
      if (htmlAttributes != null)
        tag.MergeAttributes<string, object>(htmlAttributes);
      if (!string.IsNullOrEmpty(this.ControlId))
        tag.GenerateId(this.ControlId);
      if (!string.IsNullOrEmpty(this.CoreClass))
        tag.AddCssClass(this.CoreClass);
      if (!string.IsNullOrEmpty(this.EnhancementCssClass))
        tag.AddCssClass(this.EnhancementCssClass);
      if (string.IsNullOrEmpty(this.CssClass))
        return;
      tag.AddCssClass(this.CssClass);
    }

    protected virtual void WriteHtmlContents(
      HtmlHelper htmlHelper,
      TagBuilder tag,
      StringBuilder contents)
    {
    }
  }
}
