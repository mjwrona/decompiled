// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Controls.PivotViews
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Controls
{
  public class PivotViews : ControlBase
  {
    public PivotViews(string contributionId = null, bool autoEnhance = true)
      : base("ul")
    {
      if (autoEnhance)
        this.EnhancementCssClass = "enhance";
      this.ContributionId = contributionId;
    }

    public PivotViews(IEnumerable<PivotView> views, string contributionId = null, bool autoEnhance = true)
      : this(contributionId, autoEnhance)
    {
      this.Views = views;
    }

    public PivotViews(IEnumerable<string> views, string contributrionId = null, bool autoEnhance = true)
      : this(autoEnhance: autoEnhance)
    {
      if (views == null)
        return;
      this.Views = (IEnumerable<PivotView>) views.Select<string, PivotView>((Func<string, PivotView>) (item => new PivotView(item))).ToArray<PivotView>();
    }

    public IEnumerable<PivotView> Views { get; set; }

    public string ContributionId { get; set; }

    protected override void WriteHtmlContents(
      HtmlHelper htmlHelper,
      TagBuilder tag,
      StringBuilder contents)
    {
      base.WriteHtmlContents(htmlHelper, tag, contents);
      tag.AddCssClass("pivot-view");
      tag.Attribute("role", "tablist");
      if (this.ContributionId != null)
        tag.Data("contributionId", (object) this.ContributionId);
      if (this.Views == null)
        return;
      string str = this.Views.Count<PivotView>().ToString();
      PivotView pivotView = this.Views.FirstOrDefault<PivotView>((Func<PivotView, bool>) (view => view.Selected && !view.Disabled));
      if (!this.Views.Any<PivotView>((Func<PivotView, bool>) (view => view.Selected)))
        tag.AddCssClass("empty");
      bool flag = true;
      int num = 0;
      foreach (PivotView view in this.Views)
      {
        TagBuilder tagBuilder1 = new TagBuilder("li");
        tagBuilder1.Attribute("role", "presentation");
        if (view.Disabled)
        {
          tagBuilder1.MergeAttribute("disabled", "disabled");
          tagBuilder1.AddCssClass("disabled");
          tagBuilder1.Attribute("aria-disabled", "true");
        }
        else if (pivotView == null || view.Selected)
        {
          pivotView = view;
          tagBuilder1.AddCssClass("selected");
        }
        if (view.Hidden)
          tagBuilder1.AddCssClass("invisible");
        string title = view.Title;
        if (!string.IsNullOrEmpty(title))
          tagBuilder1.MergeAttribute("title", title);
        if (!string.IsNullOrEmpty(view.Id))
          tagBuilder1.Data("id", (object) view.Id);
        ++num;
        TagBuilder tagBuilder2 = new TagBuilder("a");
        tagBuilder2.MergeAttribute("role", "tab");
        tagBuilder2.MergeAttribute("aria-posinset", num.ToString());
        tagBuilder2.MergeAttribute("aria-setsize", str);
        if (flag && !view.Disabled && !view.Hidden)
        {
          tagBuilder2.Attribute("tabindex", "0");
          flag = false;
        }
        else
          tagBuilder2.Attribute("tabindex", "-1");
        if (string.IsNullOrEmpty(view.Link))
          tagBuilder2.MergeAttribute("href", "#");
        else
          tagBuilder2.MergeAttribute("href", view.Link);
        if (view.Encoded)
          tagBuilder2.InnerHtml = view.Text;
        else
          tagBuilder2.SetInnerText(view.Text);
        tagBuilder1.InnerHtml = tagBuilder2.ToString();
        contents.AppendLine(tagBuilder1.ToString());
      }
    }
  }
}
