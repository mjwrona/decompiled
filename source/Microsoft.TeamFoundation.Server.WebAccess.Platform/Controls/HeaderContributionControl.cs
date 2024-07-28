// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Controls.HeaderContributionControl
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Server.WebAccess.Contributions.HtmlContent;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Controls
{
  public class HeaderContributionControl : ContributionControl
  {
    public HeaderContributionControlConstants Constants { get; private set; }

    public HeaderContributionControlSections SectionsToRender { get; private set; }

    public HeaderContributionControl(
      HeaderContributionControlConstants constants,
      HeaderContributionControlSections sectionsToRender = HeaderContributionControlSections.All)
      : base((string) null)
    {
      this.Constants = constants ?? new HeaderContributionControlConstants();
      this.CssClass = this.Constants.CoreCssClass;
      this.SectionsToRender = sectionsToRender;
    }

    protected override void RenderContents(
      ContributionHtmlProviderContext htmlProviderContext,
      ContributionNode contributionNode,
      TagBuilder tag)
    {
      tag.MergeAttribute("aria-busy", "true");
      tag.Append((object) htmlProviderContext.HtmlHelper.JsonIsland((object) new
      {
        supportsContribution = true,
        contributionId = contributionNode.Id,
        elementContributionType = "ms.vss-web.header-element"
      }, (object) new{ @class = "options" }).ToHtmlString());
      this.AppendTable(tag, this.Constants.L1CssClass, this.Constants.L1AriaLabelText, (Action<TagBuilder>) (trTag =>
      {
        trTag.AddClass("header-row");
        this.RenderSection(htmlProviderContext, contributionNode, trTag, HeaderContributionControlSections.Left, "left", "left-section");
        this.RenderSection(htmlProviderContext, contributionNode, trTag, HeaderContributionControlSections.Center, "center", "center-section");
        this.RenderSection(htmlProviderContext, contributionNode, trTag, HeaderContributionControlSections.Right, "right", "right-section");
      }));
      if (!this.SectionsToRender.HasFlag((Enum) HeaderContributionControlSections.Justified))
        return;
      this.AppendTable(tag, this.Constants.L2CssClass, this.Constants.L2AriaLabelText, (Action<TagBuilder>) (trTag =>
      {
        trTag.AddClass("header-row");
        trTag.AppendTag("td", (Action<TagBuilder>) (tdTag =>
        {
          tdTag.AddClass("header-td").AddClass("justified-section").MergeAttribute("colspan", "3");
          this.RenderContributedControl(htmlProviderContext, contributionNode, tdTag, "justified");
        }));
      }));
    }

    private void AppendTable(
      TagBuilder tag,
      string cssClass,
      string ariaLabelText,
      Action<TagBuilder> action)
    {
      tag.AppendTag("table", (Action<TagBuilder>) (tblTag =>
      {
        tblTag.AddClass("header-table");
        if (!string.IsNullOrEmpty(cssClass))
          tblTag.AddClass(cssClass);
        tblTag.MergeAttribute("cellspacing", "0");
        tblTag.MergeAttribute("cellpadding", "0");
        tblTag.MergeAttribute("role", "navigation");
        if (!string.IsNullOrEmpty(ariaLabelText))
          tblTag.MergeAttribute("aria-label", ariaLabelText);
        tblTag.AppendTag("tbody", (Action<TagBuilder>) (tbodyTag => tbodyTag.AppendTag("tr", action)));
      }));
    }

    private void RenderSection(
      ContributionHtmlProviderContext htmlProviderContext,
      ContributionNode contributionNode,
      TagBuilder tag,
      HeaderContributionControlSections section,
      string sectionName,
      string sectionClass)
    {
      if (!this.SectionsToRender.HasFlag((Enum) section))
        return;
      tag.AppendTag("td", (Action<TagBuilder>) (tdTag =>
      {
        tdTag.AddClass("header-td").AddClass(sectionClass);
        this.RenderContributedControl(htmlProviderContext, contributionNode, tdTag, sectionName);
      }));
    }

    private void RenderContributedControl(
      ContributionHtmlProviderContext htmlProviderContext,
      ContributionNode contributionNode,
      TagBuilder tag,
      string align)
    {
      foreach (ContributionNode targetContribution in (IEnumerable<ContributionNode>) contributionNode.Children.Where<ContributionNode>((Func<ContributionNode, bool>) (c => c.Contribution.IsOfType("ms.vss-web.header-element") && string.Equals(c.Contribution.GetProperty<string>(nameof (align)), align, StringComparison.OrdinalIgnoreCase))).OrderBy<ContributionNode, int>((Func<ContributionNode, int>) (c => c.Contribution.GetProperty<int>("order", int.MaxValue))))
        tag.Append((object) HtmlContentProviderHelpers.GetRenderedHtml(htmlProviderContext, targetContribution));
    }
  }
}
