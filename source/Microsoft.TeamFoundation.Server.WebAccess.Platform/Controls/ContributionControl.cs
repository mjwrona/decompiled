// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Controls.ContributionControl
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Server.WebAccess.Contributions.HtmlContent;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Controls
{
  public class ContributionControl : IContributionControl
  {
    public string TagName { get; private set; }

    public string CssClass { get; protected set; }

    public ContributionControl()
      : this((string) null)
    {
    }

    public ContributionControl(string cssClass)
      : this("div", cssClass)
    {
    }

    public ContributionControl(string tagName, string cssClass)
    {
      this.TagName = tagName;
      this.CssClass = cssClass;
    }

    public IHtmlString ToHtml(
      ContributionHtmlProviderContext htmlProviderContext,
      ContributionNode contributionNode,
      bool internalContentHost = true)
    {
      IHtmlString htmlString = (IHtmlString) this.RenderContentsInternal(htmlProviderContext, contributionNode).ToHtmlString();
      return internalContentHost ? new InternalContentHost().ToHtml(htmlString, contributionNode) : htmlString;
    }

    private TagBuilder RenderContentsInternal(
      ContributionHtmlProviderContext htmlProviderContext,
      ContributionNode contributionNode)
    {
      TagBuilder tagBuilder = new TagBuilder("div");
      if (!string.IsNullOrEmpty(this.CssClass))
        tagBuilder.AddClass(this.CssClass);
      StringBuilder stringBuilder = new StringBuilder();
      this.RenderContents(htmlProviderContext, contributionNode, tagBuilder);
      return tagBuilder;
    }

    protected virtual void RenderContents(
      ContributionHtmlProviderContext htmlProviderContext,
      ContributionNode contributionNode,
      TagBuilder tag)
    {
    }
  }
}
