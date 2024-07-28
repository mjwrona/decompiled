// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.HtmlContentProviderHelpers
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Contributions.HtmlContent;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public static class HtmlContentProviderHelpers
  {
    public static IHtmlString GetRenderedHtml(
      ContributionHtmlProviderContext htmlProviderContext,
      ContributionNode targetContribution)
    {
      ContributionHtmlProviderResult htmlProviderResult = (ContributionHtmlProviderResult) null;
      ContributionNode htmlProviderContribution = targetContribution.GetChildrenOfType("ms.vss-web.html-provider").FirstOrDefault<ContributionNode>();
      if (htmlProviderContribution != null)
      {
        IVssRequestContext tfsRequestContext = htmlProviderContext.HtmlHelper.ViewContext.WebContext().TfsRequestContext;
        htmlProviderResult = tfsRequestContext.GetService<IContributionHtmlProviderService>().GetHtml(tfsRequestContext, htmlProviderContext, htmlProviderContribution, targetContribution);
      }
      return htmlProviderResult == null ? (IHtmlString) MvcHtmlString.Empty : htmlProviderResult.Html;
    }

    public static IEnumerable<ContributionNode> GetChildrenOfType(
      this ContributionNode contribution,
      string contributionType)
    {
      return contribution != null && contribution.Children != null ? contribution.Children.Where<ContributionNode>((Func<ContributionNode, bool>) (c => c.Contribution.IsOfType(contributionType))) : Enumerable.Empty<ContributionNode>();
    }
  }
}
