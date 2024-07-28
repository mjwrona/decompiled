// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Controls.PivotFilter
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
  public class PivotFilter : ControlBase
  {
    public PivotFilter()
      : base("div")
    {
      this.Behavior = PivotFilterBehavior.Select;
      this.EnhancementCssClass = "enhance";
    }

    public PivotFilter(string text, IEnumerable<string> items)
      : this()
    {
      this.Text = text;
      if (items == null)
        return;
      this.Items = (IEnumerable<PivotFilterItem>) items.Select<string, PivotFilterItem>((Func<string, PivotFilterItem>) (item => new PivotFilterItem(item))).ToArray<PivotFilterItem>();
    }

    public PivotFilter(string text, IEnumerable<PivotFilterItem> items)
      : this()
    {
      this.Text = text;
      this.Items = items;
    }

    public string Text { get; set; }

    public string Title { get; set; }

    public bool Encoded { get; set; }

    public PivotFilterBehavior Behavior { get; set; }

    public IEnumerable<PivotFilterItem> Items { get; set; }

    protected override void WriteHtmlContents(
      HtmlHelper htmlHelper,
      TagBuilder tag,
      StringBuilder contents)
    {
      base.WriteHtmlContents(htmlHelper, tag, contents);
      tag.AddCssClass("pivot-filter");
      switch (this.Behavior)
      {
        case PivotFilterBehavior.Radio:
          tag.AddCssClass("radio");
          break;
        case PivotFilterBehavior.Checkbox:
          tag.AddCssClass("checkbox");
          break;
        case PivotFilterBehavior.Dropdown:
          tag.AddCssClass("dropdown");
          break;
        default:
          tag.AddCssClass("select");
          break;
      }
      if (!string.IsNullOrEmpty(this.Title))
        tag.MergeAttribute("title", this.Title);
      if (!string.IsNullOrEmpty(this.Text))
      {
        TagBuilder tagBuilder = new TagBuilder("span");
        tagBuilder.AddCssClass("title");
        if (this.Encoded)
          tagBuilder.InnerHtml = this.Text;
        else
          tagBuilder.SetInnerText(this.Text);
        contents.AppendLine(tagBuilder.ToString());
      }
      TagBuilder tagBuilder1 = new TagBuilder("ul");
      tagBuilder1.AddCssClass("pivot-filter-items");
      StringBuilder stringBuilder = new StringBuilder();
      if (this.Items != null)
      {
        PivotFilterItem pivotFilterItem = (PivotFilterItem) null;
        switch (this.Behavior)
        {
          case PivotFilterBehavior.Radio:
          case PivotFilterBehavior.Checkbox:
            using (IEnumerator<PivotFilterItem> enumerator = this.Items.GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                PivotFilterItem current = enumerator.Current;
                TagBuilder tagBuilder2 = new TagBuilder("li");
                tagBuilder2.AddCssClass("pivot-filter-item");
                if (!string.IsNullOrEmpty(current.Title))
                  tagBuilder2.MergeAttribute("title", current.Title);
                if (current.Value != null)
                  tagBuilder2.Data("value", current.Value);
                if (current.Disabled)
                {
                  tagBuilder2.MergeAttribute("disabled", "disabled");
                  tagBuilder2.AddCssClass("disabled");
                }
                else
                {
                  switch (this.Behavior)
                  {
                    case PivotFilterBehavior.Radio:
                    case PivotFilterBehavior.Checkbox:
                      if (current.Selected)
                      {
                        tagBuilder2.AddCssClass("selected");
                        break;
                      }
                      break;
                    default:
                      if (pivotFilterItem == null || current.Selected)
                      {
                        pivotFilterItem = current;
                        tagBuilder2.AddCssClass("selected");
                        break;
                      }
                      break;
                  }
                }
                TagBuilder tagBuilder3 = new TagBuilder("span");
                tagBuilder3.MergeAttribute("role", "button");
                tagBuilder3.MergeAttribute("tabIndex", "0");
                tagBuilder3.AddClass("anchor");
                if (current.Encoded)
                {
                  tagBuilder2.Data("encoded", (object) true);
                  tagBuilder3.InnerHtml = current.Text;
                }
                else
                  tagBuilder3.SetInnerText(current.Text);
                tagBuilder2.InnerHtml = tagBuilder3.ToString();
                stringBuilder.AppendLine(tagBuilder2.ToString());
              }
              break;
            }
          default:
            pivotFilterItem = this.Items.FirstOrDefault<PivotFilterItem>((Func<PivotFilterItem, bool>) (item => item.Selected && !item.Disabled));
            goto case PivotFilterBehavior.Radio;
        }
      }
      tagBuilder1.InnerHtml = stringBuilder.ToString();
      contents.AppendLine(tagBuilder1.ToString());
    }
  }
}
